using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

namespace Kubility
{

    public sealed class AsyncSocket
    {
		static List<AsyncSocket> list = new List<AsyncSocket>();

        readonly object m_lock = new object();
        short times;
        Socket m_socket;

        SocketArgsStats m_state;

		SocketInitData m_InitData;

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

		class SocketInitData
		{
			public AddressFamily addressFamily;
			public SocketType socketType;
			public ProtocolType protocolTtype;
			public EndPoint RemoteEndPoint;

			public SocketInitData(Socket socket,IPEndPoint ip)
			{
				Reset(socket,ip);
			}

			public void Reset(Socket socket,IPEndPoint ip)
			{
				this.addressFamily= socket.AddressFamily;
				this.socketType = socket.SocketType;
				this.protocolTtype = socket.ProtocolType;
				this.RemoteEndPoint = ip;
			}
		}

        public static AsyncSocket Create(Socket _socket, IPEndPoint ip)
        {
			AsyncSocket socket = null;
			if(list.Count >0)
			{
				socket = list.Find(p=> p.m_socket == _socket);
			}

			if(socket == null)
			{
				socket = new AsyncSocket();
				socket.m_socket = _socket;
				socket.times = 0;
				list.Add(socket);
			}
			socket.m_InitData = new SocketInitData(_socket,ip);

            return socket;

        }

		public void Clear()
		{
			foreach(var sub in list)
			{
				sub.CloseConnect();
			}
			list.Clear();
		}

        public EndPoint GetRemoteIP()
        {
			return m_InitData.RemoteEndPoint;
        }

        public SocketArgsStats GetSocketState()
        {
            return m_state;
        }

        public void SetStateFree()
        {
            lock (m_lock)
            {
                m_state = SocketArgsStats.FREE;
            }

        }

		public SocketArgsStats GetState()
		{
			return m_state;
		}

        bool isAviliable()
        {
            return m_socket != null && m_state != SocketArgsStats.ERROR && m_state != SocketArgsStats.UNCONNECT;
        }

        public bool SendPacketsAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    ret = m_socket.SendPacketsAsync(args);
                    m_state = SocketArgsStats.SEND;

                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }


                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null Or ERROR");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

		public bool TryConnect(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
		{
			try
			{
				bool ret = false;
				if (isAviliable())
				{
					m_state = SocketArgsStats.CONNECTING;
					ret = m_socket.ConnectAsync(args);
					
					if (!ret)
					{
						m_state = SocketArgsStats.FREE;
						ev(args);
					}
					return true;
				}
				else
				{
					Reconnect();
					return false;
				}
			}
			catch (SocketException ex)
			{
				LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
				Reconnect();
				return false;
			}
		}

        public bool SendToAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    m_state = SocketArgsStats.SEND;
                    ret = m_socket.SendToAsync(args);

                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null Or ERROR");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool SendAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {

                    ret = m_socket.SendAsync(args);
                    m_state = SocketArgsStats.SEND;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null Or ERROR");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool AcceptAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    ret = m_socket.AcceptAsync(args);
                    m_state = SocketArgsStats.ACCEPT;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null Or ERROR");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool ConnectAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    args.RemoteEndPoint = m_InitData.RemoteEndPoint;
                    ret = m_socket.ConnectAsync(args);
                    m_state = SocketArgsStats.CONNECTING;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null Or Error");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool DisconnectAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    ret = m_socket.DisconnectAsync(args);
                    m_state = SocketArgsStats.UNCONNECT;
                    if (!ret)
                    {

                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null or Error");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;

                return false;
            }

        }

        public bool ReceiveAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {

                    ret = m_socket.ReceiveAsync(args);
                    m_state = SocketArgsStats.RECEIVE;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null or Error");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool ReceiveFromAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    ret = m_socket.ReceiveFromAsync(args);
                    m_state = SocketArgsStats.RECEIVE;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null or Error");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public bool ReceiveMessageFromAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> ev)
        {
            try
            {
                bool ret = false;
                if (isAviliable())
                {
                    ret = m_socket.ReceiveMessageFromAsync(args);
                    m_state = SocketArgsStats.RECEIVE;
                    if (!ret)
                    {
                        m_state = SocketArgsStats.FREE;
                        ev(args);
                    }
                    return true;
                }
                else
                {
                    LogMgr.LogError("  Socket Is Null or Error");
					m_state = SocketArgsStats.ERROR;
                    return false;
                }
            }
            catch (SocketException ex)
            {
                LogMgr.LogError(ex);
				m_state = SocketArgsStats.ERROR;
                Reconnect();
                return false;
            }

        }

        public void Reconnect(Action<bool> callback = null)
        {

            try
            {
				LogMgr.Log("cur Connected = "+m_socket.Connected );
				if (times > Config.mIns.Retry_Times)
				{
					LogMgr.LogError("Reconnect Failed " );
					CloseConnect();
					if(callback != null)
					{
						callback(false);
					}
				}
				else if(m_socket != null && !m_socket.Connected && m_InitData.RemoteEndPoint != null)
				{
					m_socket = new Socket(m_InitData.addressFamily,m_InitData.socketType,m_InitData.protocolTtype);
					
					m_socket.BeginConnect(m_InitData.RemoteEndPoint, (IAsyncResult iar)=>
					{
						Socket handler = (Socket)iar.AsyncState;
						try
						{
							handler.EndConnect(iar);
							times = 0;
							m_state = SocketArgsStats.CONNECTING;
							if(callback != null)
							{
								callback(true);
							}
						}
						catch (Exception e)
						{
							times++;
							Reconnect();
							LogMgr.LogError(e);
						}
					}, m_socket);
				}
				else
				{
					if(callback != null)
					{
						callback(false);
					}
				}
				
			}
            catch (Exception ex)
            {
                times++;

                if (times > Config.mIns.Retry_Times)
                {
                    LogMgr.LogError("Reconnect Failed " + ex);
                    CloseConnect();
                }
                else
                    Reconnect();
            }
        }
//
//        void EndConnect(IAsyncResult iar)
//        {
//            Socket handler = (Socket)iar.AsyncState;
//            try
//            {
//                handler.EndConnect(iar);
//                times = 0;
//				m_state = SocketArgsStats.CONNECTING;
//            }
//            catch (Exception e)
//            {
//                times++;
//				Reconnect();
//                LogMgr.LogError(e);
//            }
//
//        }

        public void CloseConnect()
        {
            try
            {
                if (m_socket.Connected)
                {
					m_state = SocketArgsStats.UNCONNECT;
                    m_socket.Shutdown(SocketShutdown.Both);

                    m_socket.Close();

                }
            }
            catch (Exception ex)
            {
                LogMgr.LogError("CloseConnect " + ex);
                CloseConnect();
            }
			finally
			{
				m_state = SocketArgsStats.UNCONNECT;
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


