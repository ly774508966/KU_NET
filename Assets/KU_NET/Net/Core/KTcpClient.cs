using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Kubility
{	
	/// <summary>
	/// Tcp client.
	/// </summary>
	public class KTcpClient :AbstractNetUnit
	{

		sealed class KTcpClientDelegateCls:SingleTon<KTcpClientDelegateCls>
		{
			public bool quit =false;
			KThread m_SendThread;
			KThread m_ReceiveThread;

			ManualResetEvent mlock = new ManualResetEvent(false);


			internal void StartThreadDelegate(VoidDelegate send,VoidDelegate receive)
			{
				if(m_SendThread == null)
				{
					m_SendThread =  KThread.StartTask(send);
				}
				
				if(m_ReceiveThread == null)
				{
					m_ReceiveThread =KThread.StartTask(receive);
				}

				Resume();
			}

			internal void Pause()
			{
				mlock.Reset();
			}

			internal void Resume()
			{
				mlock.Set();
			}

			internal void Wait()
			{
				mlock.WaitOne();
			}

			internal void Close()
			{
				quit =true;
			}

		
		}

		AsyncSocket _socket;

		IPEndPoint ipend ;

		#region public Interface
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ipaddress">Ipaddress.</param>
		/// <param name="port">Port.</param>
		public bool Init(string ipaddress,int port)
		{

			IPAddress  ip ;
			bool ret = IPAddress.TryParse(ipaddress ,out ip);
			if(!ret)
			{
				LogMgr.LogError("ip parse error");
				return false;
			}

			ipend= new IPEndPoint(ip,port);

			_socket =AsyncSocket.Create( CreateTcpConnect(),ipend);

			SocketAsyncEventArgsPool.mIns.CreateSocketArgs(ipend,_socket,AcceptEventArg_OnConnectCompleted);

			startConnect(_socket,null);
			return true;
		}

		
		public void Send(BaseMessage message)
		{
			if(message != null)
			{
				MessageManager.mIns.GetSendQueue().Push_Back(message,this._socket,MessageManager.MessageOperation.Operation_send);
			}
			else
			{
				LogMgr.LogError("BaseMessage Send is Null");
			}

		}
		
		public void Send<T>(BaseMessage message,Action<T> callback)
		{
			if(message != null)
			{
				MessageManager.mIns.GetSendQueue().Push_Back(message,_socket,MessageManager.MessageOperation.Operation_send);
				message.Wait_Deserialize<T>(callback);
			}
			else
			{
				LogMgr.LogError("BaseMessage Send<T> is Null");
			}

		}

		public void Reconnect()
		{
			_socket.Reconnect((bool value)=>
			{
				if(value)
				{

					MessageManager.mIns.GetReceiveQueue().Push_Back(null,this._socket,MessageManager.MessageOperation.Operation_receive);
					
					KTcpClientDelegateCls.mIns.Resume();
				}
			});
		}

		public void CloseConnect()
		{
			_socket.CloseConnect();
		}
		
		public void Close(bool all =true)
		{
			if(all)
			{
				KTcpClientDelegateCls.mIns.Close();
			}
			SocketAsyncEventArgsPool.mIns.Close();
		}

		#endregion

		void ThreadSendMessage()
		{
			while(!KTcpClientDelegateCls.mIns.quit )
			{
				
				if(MessageManager.mIns.GetSendQueue().Size >0)
				{
					BaseMessage message;
					AsyncSocket msocket;
					if(MessageManager.mIns.GetSendQueue().Pop_First(out message,out msocket))
					{
						
						SocketAsyncEventArgsPool.mIns.Pop_FreeForSend(msocket, delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
						{
							if(!retCode)
							{
								_socketEvargs.RemoteEndPoint = _socket.GetRemoteIP();
								
								_socketEvargs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
							}
//							LogMgr.Log("sender >>");

							
							byte[] bys =message.Serialize();
							_socketEvargs.SetBuffer(bys,0,bys.Length);
							msocket.SendAsync(_socketEvargs,SendCallback);
							
						});
						
					}
					
				}
			}
		}
		
		void ThreadReceiveMessage()
		{
			while(!KTcpClientDelegateCls.mIns.quit )
			{
				
				BaseMessage message;
				AsyncSocket msocket;

				if(MessageManager.mIns.GetReceiveQueue().Pop_First(out message,out msocket))
				{

					SocketAsyncEventArgsPool.mIns.Pop_FreeForReceive(msocket, delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
					{
						
						if(!retCode)
						{
							_socketEvargs.RemoteEndPoint = msocket.GetRemoteIP();
							_socketEvargs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
						}

//						LogMgr.Log("Pop_FreeForReceive  laststate = "+ msocket.GetSocketState() +" retCode ="+retCode );
						
						msocket.ReceiveAsync(_socketEvargs,ReveiveCallBack);
						
						
					});
				}
				else
				{

					KTcpClientDelegateCls.mIns.Pause();
					KTcpClientDelegateCls.mIns.Wait();
				}
			}
		}

		void startConnect(AsyncSocket psocket, SocketAsyncEventArgs ev)
		{
			if(ev == null)
			{
				ev = new SocketAsyncEventArgs();
				ev.Completed +=new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
			}
			else
			{
				ev.AcceptSocket = null;
			}

			psocket.ConnectAsync(ev,ConnectCallback);
		}



		void ConnectCallback(SocketAsyncEventArgs args)
		{
			MessageManager.mIns.GetReceiveQueue().Push_Back(null,this._socket,MessageManager.MessageOperation.Operation_receive);
			KTcpClientDelegateCls.mIns.StartThreadDelegate(ThreadSendMessage,ThreadReceiveMessage);

		}

		void AcceptEventArg_OnConnectCompleted(object obj,SocketAsyncEventArgs ev)
		{
			Socket socket = obj as Socket;

			if(socket == null && !socket.Connected )
			{
				AsyncSocket.Create(socket,this.ipend).TryConnect(ev,ConnectCallback);
				return;
			}

			_socket.SetStateFree();
			if(ev == null)
			{
				ev = new SocketAsyncEventArgs();
				ev.Completed +=new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
			}
			else
			{
				ev.AcceptSocket = null;
			}

//			LogUtils.LogFullInfo("LastOperation = "+ ev.LastOperation.ToString());
			if(ev.LastOperation == SocketAsyncOperation.Connect)
			{
				ConnectCallback(ev);

			}
			else if(ev.LastOperation == SocketAsyncOperation.Receive)
			{
				MessageManager.mIns.GetReceiveQueue().Push_Back(null,this._socket,MessageManager.MessageOperation.Operation_receive);
				
				KTcpClientDelegateCls.mIns.Resume();
				if(ev.BytesTransferred >0 )
				{
					ReveiveCallBack(ev);
				}
			}
			else if(ev.LastOperation == SocketAsyncOperation.Send)
			{

				if(ev.BytesTransferred >0 )
				{
					SendCallback(ev);
					
				}
			}

		}

		void SendCallback(SocketAsyncEventArgs ev)
		{
			LogMgr.Log("Client  SendCallback  buffer Size = " + ev.Buffer.Length);
		}

		void ReveiveCallBack(SocketAsyncEventArgs ev)
		{
			if(ev.BytesTransferred >0)
			{
				LogMgr.Log("ReveiveCallBack bytes size = "+ ev.BytesTransferred  +" ev.SocketError = "+ ev.SocketError);
				if(ev.SocketError == SocketError.Success)
				{

					ProcessReceive(ev.Buffer,ev.BytesTransferred);


				}
			}



		}

		void ProcessReceive(byte[] Bytes,int rlen)
		{
			byte[] readData = new byte[rlen];
			System.Array.Copy(Bytes,0,readData,0,rlen);
			MessageManager.mIns.PushToReceiveBuffer(readData);

		}

		bool SocketAviliable()
		{
			return _socket !=null && _socket.GetSocketState() != AsyncSocket.SocketArgsStats.UNCONNECT && _socket.GetSocketState() != AsyncSocket.SocketArgsStats.READY;
		}


	}

}
