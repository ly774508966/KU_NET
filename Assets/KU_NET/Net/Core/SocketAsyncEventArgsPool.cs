using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Kubility
{
	public sealed class SocketAsyncEventArgsPool:SingleTon<SocketAsyncEventArgsPool>
    {
        /// <summary>
        /// list not best,improve in the further
        /// </summary>
        List<SocketEventArgsExtern> Pool;

        SocketAsyncEventArgsPoolTools m_tools;
        //		/// <summary>
        //		/// The send queue.
        //		/// </summary>
        //		LinkedList<SocketAsyncEventArgs> SendQueue = new LinkedList<SocketAsyncEventArgs>();
        //		/// <summary>
        //		/// The receive queue.
        //		/// </summary>
        //		LinkedList<SocketAsyncEventArgs> ReceiveQueue = new LinkedList<SocketAsyncEventArgs>();

        public SocketAsyncEventArgsPool()
        {
            Pool = new List<SocketEventArgsExtern>(Config.mIns.ARGS_MAX_NUM);
            m_tools = new SocketAsyncEventArgsPoolTools(this);
        }

        private class SocketEventArgsExtern
        {
            public SocketAsyncEventArgs m_ReceiveArgs;

            public SocketAsyncEventArgs m_SendArgs;

            public AsyncSocket m_connectSocket;

            public SocketEventArgsExtern()
            {
                this.m_ReceiveArgs = new SocketAsyncEventArgs();
                this.m_SendArgs = new SocketAsyncEventArgs();
                this.m_connectSocket = null;
            }

        }

        class PoolCMD
        {
            /// <summary>
            /// seconds
            /// </summary>
            public float delayTime;
            /// <summary>
            /// 1 = send  2 =recevied
            /// </summary>
            public short flag;
			public AsyncSocket target;
            public Action<SocketAsyncEventArgs, bool> ev;
        }


        sealed class SocketAsyncEventArgsPoolTools
        {
            byte quit = 0;
            ManualResetEvent m_lock = new ManualResetEvent(false);
            Stack<PoolCMD> cmdList = new Stack<PoolCMD>();

            public SocketAsyncEventArgsPoolTools(SocketAsyncEventArgsPool pool)
            {
                m_lock.Reset();
                KThread.StartTask(delegate()
                {
                    PoolThread(pool);
                });

            }
            /// <summary>
            /// dont want to call thread.abort
            /// </summary>
            public void Close()
            {
                quit = 1;
                Resume();
            }

            public void Pause()
            {
                m_lock.Reset();
            }

            public void Resume()
            {
                m_lock.Set();
            }

            public void PushCmd(PoolCMD cmd)
            {
                if (cmd != null)
                {
                    lock (cmdList)
                    {
                        cmdList.Push(cmd);
                       
                    }
					Resume();
                }
            }

            void PoolThread(object obj)
            {
                SocketAsyncEventArgsPool argsPool = (SocketAsyncEventArgsPool)obj;
                while (quit == 0)
                {
                    while (cmdList.Count > 0)
                    {
                        PoolCMD cmd = cmdList.Pop();

                        float DelayTime = cmd.delayTime;
                        bool found = false;
                        Action<SocketAsyncEventArgs, bool> cmdEv = cmd.ev;

                        if (cmdEv != null)
                        {

                            int tryTimes = 0;
                            while (!found && tryTimes < 4)
                            {

								SocketEventArgsExtern arg = argsPool.Pool.Find(p => p.m_connectSocket.Equals(cmd.target));

								if(arg != null)
								{
	
									AsyncSocket.SocketArgsStats state = cmd.target.GetSocketState();
									if (cmd != null && cmd.target != null 
									    && state != AsyncSocket.SocketArgsStats.UNCONNECT
									    && state != AsyncSocket.SocketArgsStats.ERROR
									    && state != AsyncSocket.SocketArgsStats.READY)
									{
										found = true;
	
										if (cmd.flag == 1)//send
										{
											cmdEv(arg.m_SendArgs, found);
										}
										else if (cmd.flag == 2)//receiced
										{
											cmdEv(arg.m_ReceiveArgs, found);
										}
										else if (cmd.flag ==3)
										{
											cmdEv(new SocketAsyncEventArgs(), false);
										}
										else
										{
											LogUtils.LogFullInfo("Cmd Error",LogType.ERROR);
											found = false;
										}
									}


								}

								tryTimes++;
							}

                            if (DelayTime > 0.001f)
                            {
                                Thread.Sleep((int)(DelayTime * 1000));
                            }

                            if (!found)
                            {
                                SocketAsyncEventArgs newargs = new SocketAsyncEventArgs();
                                if (cmd.flag == 1 || cmd.flag ==3)//send
                                {

                                }
                                else if (cmd.flag == 2)//receiced
                                {
                                    newargs.SetBuffer(new byte[Config.mIns.EACH_SOCKET_RECEIVE_SIZE], 0, Config.mIns.EACH_SOCKET_RECEIVE_SIZE);

                                }

                                cmdEv(newargs, found);
                            }

                        }
                        else
                        {
                            LogMgr.LogError("please set callback first ,otherwise it will never return anything");
                        }


                    }

                    Pause();
					m_lock.WaitOne();
                }

//                LogMgr.Log("PoolThread is Done  = " + quit.ToString());
            }
        }
        /// <summary>
        /// Creates the socket arguments. will Try Twice
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="socket">Socket.</param>
        /// <param name="ev">Ev.</param>
        public void CreateSocketArgs(IPEndPoint ip, AsyncSocket socket, Action<object, SocketAsyncEventArgs> ev)
        {

            if (Pool.Count < Config.mIns.ARGS_MAX_NUM)
            {
                SocketEventArgsExtern args = new SocketEventArgsExtern();
                args.m_ReceiveArgs.RemoteEndPoint = ip;
                args.m_ReceiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ev);
                args.m_ReceiveArgs.SetBuffer(new byte[Config.mIns.EACH_SOCKET_RECEIVE_SIZE], 0, Config.mIns.EACH_SOCKET_RECEIVE_SIZE);
                args.m_SendArgs.RemoteEndPoint = ip;
                args.m_SendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ev);
                args.m_connectSocket = socket;
                lock (Pool)
                {
                    Pool.Add(args);
                }

            }
            else
            {
                Pool.RemoveAll(x => x == null);
                if (Pool.Count < Config.mIns.ARGS_MAX_NUM)
                {
                    SocketEventArgsExtern args = new SocketEventArgsExtern();
                    args.m_ReceiveArgs.RemoteEndPoint = ip;
                    args.m_ReceiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ev);
                    args.m_ReceiveArgs.SetBuffer(new byte[Config.mIns.EACH_SOCKET_RECEIVE_SIZE], 0, Config.mIns.EACH_SOCKET_RECEIVE_SIZE);
                    args.m_SendArgs.RemoteEndPoint = ip;
                    args.m_SendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ev);
                    args.m_connectSocket = socket;
                    lock (Pool)
                    {
                        Pool.Add(args);
                    }
                }
                else
                    LogMgr.LogError("SocketAsyncEventArgsPool is out of Range .please update the config first.");
            }

        }

		public void Pop_FreeForOther(AsyncSocket _socket, Action<SocketAsyncEventArgs, bool> ev, float delayTime = 0f)
		{
			
			PoolCMD cmd = new PoolCMD();
			cmd.delayTime = delayTime;
			cmd.flag = 3;
			cmd.ev = ev;
			cmd.target = _socket;

			m_tools.PushCmd(cmd);
			
		}

		public void Pop_FreeForSend(AsyncSocket _socket,Action<SocketAsyncEventArgs, bool> ev, float delayTime = 0f)
        {

            PoolCMD cmd = new PoolCMD();
            cmd.delayTime = delayTime;
            cmd.flag = 1;
            cmd.ev = ev;
			cmd.target = _socket;

            m_tools.PushCmd(cmd);

        }

		public void Pop_FreeForReceive(AsyncSocket _socket,Action<SocketAsyncEventArgs, bool> ev, float delayTime = 0f)
        {
            PoolCMD cmd = new PoolCMD();
            cmd.delayTime = delayTime;
            cmd.flag = 2;
            cmd.ev = ev;
			cmd.target = _socket;
            m_tools.PushCmd(cmd);
        }

        public void Remove_SocketArgs(AsyncSocket _socket)
        {
            if (_socket != null)
            {
                Pool.RemoveAll(x => x == null || x.m_connectSocket == _socket);
            }


        }

        public void Close()
        {
            foreach (var sub in Pool)
            {
                sub.m_connectSocket.CloseConnect();
            }

            this.Pool.Clear();
            this.m_tools.Close();
        }


        public void Dump()
        {
            LogMgr.Log("----------START------------");

            var fields = typeof(SocketEventArgsExtern).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            var propertys = typeof(SocketEventArgsExtern).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            foreach (var sub in Pool)
            {
                KTool.Dump(sub, typeof(SocketEventArgsExtern), fields, propertys);
            }

            LogMgr.Log("----------END------------");
        }
    }
}



