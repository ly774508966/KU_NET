using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;

namespace Kubility
{

	public sealed class AsyncSocket  
	{
		readonly object m_lock = new object();
		short times ;
		Socket m_socket;

		SocketArgsStats m_state;

		EndPoint m_IP;

		public enum SocketArgsStats
		{

			READY,
			FREE,
			CONNECTING,
			UNCONNECT,
			SEND,
			RECEIVE,
			ACCEPT,
			ERROR,
		}



		public static AsyncSocket Create(Socket _socket,IPEndPoint ip)
		{
			AsyncSocket socket = new AsyncSocket();
			socket.m_socket = _socket;
			socket.m_IP =ip;
			socket.times = 0;
			return socket;

		}

		public EndPoint GetRemoteIP()
		{
			return m_IP;
		}

		public SocketArgsStats GetSocketState()
		{
			return m_state;
		}

		public void SetStateFree()
		{
			lock(m_lock)
			{
				m_state = SocketArgsStats.FREE;
			}

		}

		bool isAviliable()
		{
			return m_socket != null  &&m_state != SocketArgsStats.ERROR && m_state != SocketArgsStats.UNCONNECT;
		}

		public bool SendPacketsAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					ret = m_socket.SendPacketsAsync(args);
					m_state =  SocketArgsStats.SEND;

//					LogMgr.LogError("SendPacketsAsync  ret "+ ret);
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					
					
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null Or ERROR");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool SendToAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					m_state = SocketArgsStats.SEND;
					ret = m_socket.SendToAsync(args);

					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null Or ERROR");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool SendAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{

					ret = m_socket.SendAsync(args);
					m_state = SocketArgsStats.SEND;
//					LogMgr.LogError("SendAsync   =   "+ret);
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null Or ERROR");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool AcceptAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					ret = m_socket.AcceptAsync(args);
					m_state = SocketArgsStats.ACCEPT;
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null Or ERROR");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool ConnectAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					args.RemoteEndPoint = this.m_IP;
					ret = m_socket.ConnectAsync(args);
					m_state = SocketArgsStats.CONNECTING;
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null Or Error");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool DisconnectAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					ret = m_socket.DisconnectAsync(args);
					m_state = SocketArgsStats.UNCONNECT;
					if(!ret)
					{

						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null or Error");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);

				return false;
			}

		}

		public bool ReceiveAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{

					ret = m_socket.ReceiveAsync(args);
					m_state = SocketArgsStats.RECEIVE;
//					LogMgr.LogError("ReceiveAsync = "+ret);
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null or Error");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool ReceiveFromAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					ret = m_socket.ReceiveFromAsync(args);
					m_state = SocketArgsStats.RECEIVE;
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null or Error");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs args,Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret =false;
				if(isAviliable())
				{
					ret = m_socket.ReceiveMessageFromAsync(args);
					m_state = SocketArgsStats.RECEIVE;
					if(!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					LogMgr.LogError("  Socket Is Null or Error");
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				Reconnect();
				return false;
			}

		}

		public void Reconnect()
		{

			try
			{
				if(m_socket != null && m_IP != null)
				{
					m_socket.BeginConnect(m_IP,EndConnect,m_socket);
				}
			}
			catch(Exception ex)
			{
				times++;

				if(times > Config.mIns.Retry_Times)
				{
					LogMgr.LogError("Reconnect Failed " + ex);
					CloseConnect();
				}
				else
					Reconnect();
			}
		}

		void EndConnect(IAsyncResult iar)
		{
			Socket handler=(Socket)iar.AsyncState;
			try
			{
				handler.EndConnect(iar);
				times =0;
			}
			catch (Exception e)
			{
				times++;
				LogMgr.LogError(e);
			}

		}
		
		public void CloseConnect()
		{
			try
			{
				if(m_socket.Connected )
				{
					m_socket.Shutdown( SocketShutdown.Both);
					m_socket.Close();


				}
			}
			catch (Exception ex)
			{
				LogMgr.LogError("CloseConnect " + ex);
				CloseConnect();
			}
		}

		public bool Connected()
		{
			if (m_socket.Poll(-1, SelectMode.SelectRead))
			{
				int nRead = m_socket.Receive(new byte[256]);
				if (nRead == 0)
				{
					return false;
				}
				return true;
			}

			return false;
		}
	}
}


