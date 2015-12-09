//#define SHOW_LOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Kubility
{
    public sealed class KThread : KObject
    {
        Thread m_thread;

        public static bool RaiseAbortException = false;

		private object objLock = new object();

		AutoResetEvent _restEv ;
		AutoResetEvent restEv
		{
			get
			{
				lock(objLock)
				{
					return _restEv;
				}
			}
			set
			{
				_restEv = value;
			}
		}

        VoidDelegate VoidEv;

        delegate void SingleDelegate(object obj);

        SingleDelegate SEv;


        bool kill = false;
		bool isWorking =false;

        public override void OnCreate()
        {
            base.OnCreate();
#if SHOW_LOG
            LogMgr.Log("Thread OnCreate");
#endif

        }

        public override void OnPause()
        {
            base.OnPause();
#if SHOW_LOG
            LogMgr.Log("Thread OnPause");
#endif
        }

        public override void OnResume()
        {
            base.OnResume();
#if SHOW_LOG
            LogMgr.Log("Thread OnResume");
#endif
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
#if SHOW_LOG
            LogMgr.Log("Thread OnDestory");
#endif
//			LogMgr.Log("destroy  UID ="+ GetUID());
            this.VoidEv = null;
            this.SEv = null;

        }

        public override void OnEnter()
        {
            base.OnEnter();
#if SHOW_LOG
            LogMgr.Log("Thread OnStart");
#endif

        }

        public override void OnExit()
        {
            base.OnExit();
#if SHOW_LOG
            LogMgr.Log("Thread OnExit");
#endif

        }

        static int counter = 0;

        KThread()
        {
            if (m_thread == null)
            {
                m_thread = new Thread(new ThreadStart(ThreadEvents));
                m_thread.Name = counter.ToString();
				Interlocked.Increment(ref counter);
				restEv= new AutoResetEvent(false);
            }


        }

		public int GetUID()
		{
			if(m_thread != null)
			{
				int value ;
				if(int.TryParse (m_thread.Name, out value ))
				{
					return value;
				}
				return -1;
			}
			else
			{
				return -1;
			}
		}

        public static KThread StartTask(VoidDelegate vev, bool autoStart = true)
        {
            KThread th = null;

            KThreadPool.mIns.Push_Task(vev, autoStart, delegate(KThread obj)
            {
                th = obj;

            });

            return th;
        }


        void ThreadEvents()
        {

            try
            {
                while (!kill)
                {

                    restEv.WaitOne();

                    OnEnter();

                    if (VoidEv != null)
                    {
						isWorking =true;
                        VoidEv();
                    }

					isWorking =false;

					if (SEv != null && !kill)
                    {
                        SEv(this);
                    }

                    OnExit();
                }

                OnDestroy();
            }
            catch (Exception ex)
            {
#if SHOW_LOG
				if(ex is ThreadAbortException && RaiseAbortException)
				{
					LogMgr.LogError("ThreadAbortException Error >>= " + ex.ToString());
				}
				else
                	LogMgr.LogError("ThreadEvents Error >> = " + ex.ToString());
#endif

            }
            finally
            {
                if ( KThreadPool.mIns.Remove(this))
				{
					OnDestroy();
				}

				if(VoidEv != null)
				{
					KThread.StartTask(VoidEv);
				}

            }

        }

        #region Force

        public void ForceSuspend()
        {

            if (m_thread != null && isRunning)
            {
                OnPause();
                m_thread.Suspend();
            }
        }

        public void ForceResume()
        {

            if (m_thread != null && !isRunning)
            {
                OnResume();
                m_thread.Resume();
            }
        }

        #endregion


        #region Weak

        public void WeakStop()
        {
            if (m_thread != null)
            {
                OnPause();
                restEv.Reset();
            }
        }

        public void WeakResume()
        {
            if (m_thread != null && m_thread.ThreadState == ThreadState.WaitSleepJoin)
            {
                OnResume();
                restEv.Set();
            }
        }

        #endregion

        /// <summary>
        /// next loop will stop
        /// </summary>
        public void WillKill()
        {

            if (m_thread != null && m_thread.ThreadState != ThreadState.Unstarted)
            {
                this.kill = true;
                this.restEv.Set();
            }
            else if (m_thread.ThreadState == ThreadState.Unstarted)
            {
                Abort();
            }
        }


        public void Start()
        {
            if (m_thread != null)
            {
                KThreadPool.mIns.StartTask(KThreadPool.mIns.GetFirstTask());
            }
        }

        public void Abort()
        {
            if (m_thread != null)
            {
                VoidEv = null;
                SEv =  null;
				if(isWorking)
				{
					m_thread.Abort();
					OnDestroy();
				}
				else
				{
					kill =true;
					restEv.Set();
				}
                

            }
			else
			{
				OnDestroy();
			}
        }

        public void Join()
        {
            if (m_thread != null && !isRunning)
            {
                m_thread.Join();

            }
        }

        public void SetPriority(System.Threading.ThreadPriority priority)
        {
            if (m_thread != null)
            {
                m_thread.Priority = priority;
            }
        }

        public static void CloseAll()
        {
            KThreadPool.mIns.Close();
        }

        private class KThreadPool : SingleTon<KThreadPool>
        {

            LinkedList<KThread> WorkQueue = new LinkedList<KThread>();
			Stack<VoidDelegate> TaskQueue = new Stack<VoidDelegate>();
            LinkedList<KThread> WaitQueue = new LinkedList<KThread>();
            bool _stop = false;

            public KThreadPool()
            {
//                for (int i = 0; i < 5; ++i)
//                {
//                    var th = new KThread();
//                    Push_ToWaitQueue(th);
//                }
            }

            public void ResumeAll()
            {
                _stop = false;
                var enumerator = WorkQueue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KThread sub = (KThread)enumerator.Current;
                    sub.ForceResume();
                }
            }

            public void StopAll()
            {
                _stop = true;
                var enumerator = WorkQueue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KThread sub = (KThread)enumerator.Current;
                    sub.ForceSuspend();
                }

            }



            public void Close()
            {

                _stop = true;
//				lock(m_lock)
//				{
//					if(TaskQueue.Count >0)
//						TaskQueue.Clear();
//				}


				lock(m_lock)
				{
					while(WorkQueue.Count >0)
					{
						KThread sub = WorkQueue.First.Value;
						if (sub != null) sub.Abort();
						WorkQueue.Remove(sub);
					}
				}

//                var enumerator = WorkQueue.GetEnumerator();
//                while (enumerator.MoveNext())
//                {
//                    KThread sub = (KThread)enumerator.Current;
////                      sub.WillKill();
//                    //使用强制关闭
//                    sub.Abort();
//
//                }


                WorkQueue.Clear();
				lock(m_lock)
				{
					while(WaitQueue.Count >0)
					{
						KThread sub = WaitQueue.First.Value;
						if (sub != null) sub.Abort();
						WaitQueue.Remove(sub);
					}
				}


//                var wenumerator = WaitQueue.GetEnumerator();
//                while (wenumerator.MoveNext())
//                {
//                    KThread sub = (KThread)wenumerator.Current;
////                    sub.WillKill();
//                    //使用强制关闭
//                    sub.Abort();
//                }
                WaitQueue.Clear();

            }


            public void Push_ToWaitQueue(KThread th)
            {
                if (th != null)
                {
                    lock (m_lock)
                    {
                        WaitQueue.AddLast(th);
                    }


                }
            }

            public void Push_ToWorkQueue(KThread th)
            {
                if (th != null)
                {
                    lock (m_lock)
                    {
                        WorkQueue.AddLast(th);
                    }

                }
            }

            public bool Remove(KThread th)
            {
				bool ret1 =false;
				bool ret2 =false;
                lock (m_lock)
                {

					ret1 =WorkQueue.Remove(th);
					ret2 =WaitQueue.Remove(th);
//					th.OnDestroy();
                }
				return ret2 || ret1;
            }
            
            public void Push_Task(VoidDelegate vev, bool autoStart, Action<KThread> callback = null)
            {
                TaskQueue.Push(vev);

                if (callback != null)
                {
                    if (WaitQueue.Count > 0)
                        callback(WaitQueue.First.Value);
                    else
                    {
                        if (WaitQueue.Count + WorkQueue.Count <= Config.mIns.Thread_MaxSize)
                        {
                            var th = new KThread();
                            Push_ToWaitQueue(th);
                            
                            callback(WaitQueue.First.Value);
                        }
                    }

                }
                
                
                if (autoStart)
                    StartTask(GetFirstTask());
            }

            public VoidDelegate GetFirstTask()
            {
                if (TaskQueue.Count > 0)
                    return TaskQueue.Peek();
                else
                    return null;
            }

            public void StartTask(VoidDelegate vev)
            {
                if (_stop || vev == null)
                    return;

                TaskQueue.Pop();

                if (WaitQueue.Count > 0)
                {

                    KThread th = null;
                    lock (m_lock)
                    {
                        var en = WaitQueue.GetEnumerator();
                        while (en.MoveNext())
                        {
                            th = en.Current as KThread;
                            if (th.m_thread.ThreadState == ThreadState.Aborted)
                                WaitQueue.Remove(th);
                            else if (!th.isRunning)
                                break;
                        }

                        if (th == null)
                            return;

                        th.VoidEv = null;
                        th.SEv = null;

                        th.VoidEv += vev;
                        th.SEv = Check;

                        WorkQueue.AddLast(th);
                        WaitQueue.Remove(th);
                    }
      

                    if (th.m_thread.ThreadState == ThreadState.Unstarted)
                    {

						th.restEv.Set();
                        th.m_thread.Start();
                    }
                    else if (th.m_thread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        th.WeakResume();
                    }

                }
                else
                {
                    if (WaitQueue.Count + WorkQueue.Count <= Config.mIns.Thread_MaxSize)
                    {
                        var th = new KThread();
                        Push_ToWaitQueue(th);

                        StartTask(vev);
                    }

                }


            }

            void Check(object obj)
            {
                if (_stop)
                    return;

                KThread th;

                lock (m_lock)
                {
                    th = (KThread)obj;
                    WaitQueue.AddLast(th);
                    WorkQueue.Remove(th);
                }

                th.VoidEv = null;
                th.SEv = null;

                if (TaskQueue.Count > 0)
                {
                    StartTask(GetFirstTask());
                }

                th.WeakStop();

            }



        }

    }
}


