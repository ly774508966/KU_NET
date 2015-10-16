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
		public bool quit =false;

		AsyncSocket _socket;
		KThread m_SendThread;
		KThread m_ReceiveThread;
		IPEndPoint ipend ;

		ManualResetEvent mlock = new ManualResetEvent(false);

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
			MessageManager.mIns.GetSendQueue().Push_Back(message);
		}
		
		public void Send<T>(BaseMessage message,Action<T> callback)
		{
			MessageManager.mIns.GetSendQueue().Push_Back(message);
			message.Wait_Deserialize<T>(callback);
		}

		public void Reconnect()
		{
			_socket.Reconnect((bool value)=>
			{
//				LogMgr.LogError("Reconnect callback  "+ value );
				if(value)
				{
					mlock.Set();
				}
			});
		}
		
		public void Close()
		{
			quit= true;
			mlock.Set();
			SocketAsyncEventArgsPool.mIns.Close();
		}

		public AsyncSocket GetSocket()
		{
			return _socket;
		}

		#endregion

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
			if(m_SendThread == null)
			{
				m_SendThread =  KThread.StartTask(ThreadSendMessage);
			}
			
			if(m_ReceiveThread == null)
			{
				m_ReceiveThread =KThread.StartTask(ThreadReceiveMessage);
			}
			else
			{
				mlock.Set();
			}
		}
		
		void ThreadSendMessage()
		{
			while(!quit )
			{

				if(MessageManager.mIns.GetSendQueue().Size >0)
				{

					BaseMessage message;
					MessageManager.mIns.GetSendQueue().Pop_First(out message);
					if(message != null)
					{

						SocketAsyncEventArgsPool.mIns.Pop_FreeForSend(delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
						{
							//LogMgr.Log("Pop_FreeForSend  laststate = "+ _socket.GetSocketState());

							if(!retCode)
							{
								_socketEvargs.RemoteEndPoint = _socket.GetRemoteIP();
								_socketEvargs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
							}

							byte[] bys =message.Serialize();
							_socketEvargs.SetBuffer(bys,0,bys.Length);
							_socket.SendAsync(_socketEvargs,SendCallback);
	
						});

					}
					
				}
			}
		}

		void ThreadReceiveMessage()
		{
			while(!quit )
			{

				mlock.Reset();

				if(SocketAviliable())
				{
					SocketAsyncEventArgsPool.mIns.Pop_FreeForReceive(delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
					{

						if(!retCode)
						{
							_socketEvargs.RemoteEndPoint = _socket.GetRemoteIP();
							_socketEvargs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
						}
//						LogMgr.Log("Pop_FreeForReceive  laststate = "+ _socket.GetSocketState() +" retCode ="+retCode );

						_socket.ReceiveAsync(_socketEvargs,ReveiveCallBack);


					});
					mlock.WaitOne();
				}
			}

			
		}


		void AcceptEventArg_OnConnectCompleted(object obj,SocketAsyncEventArgs ev)
		{
			Socket socket = obj as Socket;

			if(socket == null && !socket.Connected && !quit)
			{
				AsyncSocket.Create(socket,this.ipend).TryConnect(ev,ConnectCallback);
				return;
			}
			else if(quit)
			{
				//may do something
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

//			LogMgr.LogError("LastOperation = "+ ev.LastOperation.ToString());
			if(ev.LastOperation == SocketAsyncOperation.Connect)
			{
				ConnectCallback(ev);

			}
			else if(ev.LastOperation == SocketAsyncOperation.Receive)
			{
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

			mlock.Set();

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




