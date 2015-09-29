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
		SocketAsyncEventArgsPool m_pool = new SocketAsyncEventArgsPool();
		public bool quit =false;

		AsyncSocket _socket;
		Thread m_SendThread;
		Thread m_ReceiveThread;

		ManualResetEvent mlock = new ManualResetEvent(false);

		#region init Interface
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

			IPEndPoint ipend = new IPEndPoint(ip,port);

			_socket =AsyncSocket.Create( CreateTcpConnect(),ipend);

			m_pool.CreateSocketArgs(ipend,_socket,AcceptEventArg_OnConnectCompleted);

			startConnect(_socket,null);
			return true;
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

		public void Close()
		{
			quit= true;
			m_SendThread.Abort();
			m_ReceiveThread.Abort();
			m_pool.Close();
		}

		void ConnectCallback(SocketAsyncEventArgs args)
		{

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

						m_pool.Pop_FreeForSend(delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
						{
							LogMgr.Log("Pop_FreeForSend  laststate = "+ _socket.GetSocketState());

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
					m_pool.Pop_FreeForReceive(delegate(SocketAsyncEventArgs _socketEvargs,bool retCode) 
					{

						if(!retCode)
						{
							_socketEvargs.RemoteEndPoint = _socket.GetRemoteIP();
							_socketEvargs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_OnConnectCompleted); 
						}
						LogMgr.Log("Pop_FreeForReceive  laststate = "+ _socket.GetSocketState() +" retCode ="+retCode
						           );
						//KTool.Dump(_socketEvargs);
						_socket.ReceiveAsync(_socketEvargs,ReveiveCallBack);


					});
					mlock.WaitOne();
				}
			}

			
		}
		
		public void Send(BaseMessage message)
		{
			MessageManager.mIns.GetSendQueue().Push_Back(message);
		}

		void AcceptEventArg_OnConnectCompleted(object obj,SocketAsyncEventArgs ev)
		{
//			if(!Application.isPlaying)
//				return;

			LogMgr.Log("complate ");
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

			LogMgr.LogError("SocketAsyncOperation  ="+ ev.LastOperation  +"  obj = "+ obj  +"ev.UserToken  =" + ev.UserToken);



			if(ev.LastOperation == SocketAsyncOperation.Connect)
			{
				if(m_SendThread == null)
				{
					LogMgr.Log("m_SendThread start");
					m_SendThread = new Thread(ThreadSendMessage);
					m_SendThread.Start();

				}

				if(m_ReceiveThread  == null)
				{

					LogMgr.Log("m_ReceiveThread start");
					m_ReceiveThread = new Thread(ThreadReceiveMessage);
					m_ReceiveThread.Start();
				}

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
				LogMgr.Log("ReveiveCallBack 1  = "+ ev.BytesTransferred  +" ev.SocketError = "+ ev.SocketError);
				if(ev.SocketError == SocketError.Success)
				{

					ProcessReceive(ev.Buffer,ev.BytesTransferred);


				}
			}

			mlock.Set();


		}

		void ProcessReceive(byte[] Bytes,int rlen)
		{
//			if(!Application.isPlaying)
//				return;

			byte[] readData = new byte[rlen];
			System.Array.Copy(Bytes,0,readData,0,rlen);

			BaseMessage.Deserialize(readData);

		}


		#endregion

		bool SocketAviliable()
		{
			return _socket !=null && _socket.GetSocketState() != AsyncSocket.SocketArgsStats.UNCONNECT && _socket.GetSocketState() != AsyncSocket.SocketArgsStats.READY;
		}


	}

}




