//#define SHOW_LOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Kubility
{
		public sealed class KThread :KObject
		{
				Thread m_thread;
		
				ManualResetEvent restEv = new ManualResetEvent (false);
		
				VoidDelegate VoidEv;

				delegate void SingleDelegate (object obj);

				SingleDelegate SEv;
		
		
				bool kill = false;

				protected override void OnCreate ()
				{
						base.OnCreate ();
						#if SHOW_LOG
						LogMgr.Log ("Thread OnCreate");
						#endif
			
				}

				protected override void OnPause ()
				{
						base.OnPause ();
						#if SHOW_LOG
						LogMgr.Log ("Thread OnPause");
						#endif
				}

				protected override void OnResume ()
				{
						base.OnResume ();
						#if SHOW_LOG
						LogMgr.Log ("Thread OnResume");
						#endif
				}

				protected override void OnDestroy ()
				{
						base.OnDestroy ();
						#if SHOW_LOG
						LogMgr.Log ("Thread OnDestory");
						#endif
			
						this.VoidEv = null;
						this.SEv = null;
						this.restEv.Reset ();
			
				}

				protected override void OnEnter ()
				{
						base.OnEnter ();
						#if SHOW_LOG
						LogMgr.Log ("Thread OnStart");
						#endif
						restEv.Set ();
				}

				static int counter = 0;

				KThread ()
				{
						if (m_thread == null) {
								m_thread = new Thread (new ThreadStart (ThreadEvents));
								m_thread.Name = counter.ToString ();
								counter++;
								restEv.Reset ();
						}
				}

				public static KThread StartTask (VoidDelegate vev, bool autoStart = true)
				{
						KThread th = null;
						KThreadPool.mIns.Push_Task (vev, autoStart, delegate(KThread obj) {
								th = obj;
				
						});
			
						return th;
				}

		
				void ThreadEvents ()
				{

						try {
								OnEnter ();
								while (!kill) {
										restEv.WaitOne ();

										if (VoidEv != null) {
												VoidEv ();
										}
					
										if (SEv != null) {
												SEv (this);
										}
								}
				
								OnDestroy ();
						} catch (Exception ex) {
								LogMgr.LogError ("ex  = " + ex.ToString ());
						}

				}

				#region Force

				public void ForceSuspend ()
				{
						if (m_thread != null && m_thread.ThreadState == ThreadState.Running) {
								OnPause ();
								m_thread.Suspend ();
						}
				}

				public void ForceResume ()
				{
						if (m_thread != null && m_thread.ThreadState == ThreadState.Suspended) {
								OnResume ();
								m_thread.Resume ();
						}
				}

				#endregion

		
				#region Weak

				public void WeakStop ()
				{
						if (m_thread != null) {
								OnPause ();
								restEv.Reset ();
						}
				}

				public void WeakResume ()
				{
						if (m_thread != null && m_thread.ThreadState == ThreadState.WaitSleepJoin) {
								OnResume ();
								restEv.Set ();
						}
				}

				#endregion

				/// <summary>
				/// next loop will stop
				/// </summary>
				public void WillKill ()
				{
						LogMgr.LogError ("enter");
						if (m_thread != null && m_thread.ThreadState != ThreadState.Unstarted) {
								this.kill = true;
								this.restEv.Set ();
						} else if (m_thread.ThreadState == ThreadState.Unstarted) {
								Abort ();
						}
				}

		
				public void Start ()
				{
						if (m_thread != null) {
								KThreadPool.mIns.StartTask (KThreadPool.mIns.GetFirstTask ());
						}
				}

				public void Abort ()
				{
						if (m_thread != null) {
								ForceSuspend ();
								m_thread.Abort ();
								OnDestroy ();
						}
				}

				public void Join ()
				{
						if (m_thread != null && !isRunning) {
								m_thread.Join ();
				
						}
				}

				public void SetPriority (System.Threading.ThreadPriority priority)
				{
						if (m_thread != null) {
								m_thread.Priority = priority;
						}
				}

				public static void CloseAll ()
				{
						KThreadPool.mIns.Close ();
				}

				private class KThreadPool:SingleTon<KThreadPool>
				{

						List<KThread> WorkQueue = new List<KThread> ();
						LinkedList<VoidDelegate> TaskQueue = new LinkedList<VoidDelegate> ();
						List<KThread> WaitQueue = new List<KThread> ();
						bool _stop = false;

						public KThreadPool ()
						{
								for (int i = 0; i < 5; ++i) {
										var th = new KThread ();
										Push_ToWaitQueue (th);
								}
						}

						public void ResumeAll ()
						{
								_stop = false;
								List<KThread>.Enumerator enumerator = WorkQueue.GetEnumerator ();
								while (enumerator.MoveNext ()) {
										KThread sub = (KThread)enumerator.Current;
										sub.ForceResume ();
								}
						}

						public void StopAll ()
						{
								_stop = true;
								List<KThread>.Enumerator enumerator = WorkQueue.GetEnumerator ();
								while (enumerator.MoveNext ()) {
										KThread sub = (KThread)enumerator.Current;
										sub.ForceSuspend ();
								}

						}



						public void Close ()
						{
								_stop = true;
								TaskQueue.Clear ();
								LogMgr.LogError ("total work = " + (WorkQueue.Count) + "  wait =" + WaitQueue.Count);
								List<KThread>.Enumerator enumerator = WorkQueue.GetEnumerator ();
								while (enumerator.MoveNext ()) {
										KThread sub = (KThread)enumerator.Current;
										sub.WillKill ();

								}
								LogMgr.LogError ("wait");

								WorkQueue.Clear ();
								List<KThread>.Enumerator wenumerator = WaitQueue.GetEnumerator ();
								while (wenumerator.MoveNext ()) {
										KThread sub = (KThread)wenumerator.Current;
										sub.WillKill ();

								}
								WaitQueue.Clear ();

						}

			
						public void Push_ToWaitQueue (KThread th)
						{
								if (th != null) {
										lock (m_lock) {
												WaitQueue.Add (th);
										}

					
								}
						}

						public void Push_ToWorkQueue (KThread th)
						{
								if (th != null) {
										lock (m_lock) {
												WorkQueue.Add (th);
										}
					
								}
						}

						public void Push_Task (VoidDelegate vev, bool autoStart, Action<KThread> callback = null)
						{
								TaskQueue.AddLast (vev);
				
								if (callback != null) {
										if (WaitQueue.Count > 0)
												callback (WaitQueue [WaitQueue.Count - 1]);
										else
												callback (null);
								}
				
				
								if (autoStart)
										StartTask (GetFirstTask ());
						}

						public VoidDelegate GetFirstTask ()
						{
								if (TaskQueue.Count > 0)
										return TaskQueue.First.Value;
								else
										return null;
						}

						public void StartTask (VoidDelegate vev)
						{
								if (_stop || vev == null)
										return;
								TaskQueue.RemoveFirst ();
				
								if (WaitQueue.Count > 0) {
										KThread th;
										lock (m_lock) {
												th = WaitQueue [WaitQueue.Count - 1];
												while (th.m_thread.ThreadState == ThreadState.Aborted && WaitQueue.Count > 0) {
														WaitQueue.Remove (th);
														th = WaitQueue [WaitQueue.Count - 1];
												}
												if (WaitQueue.Count == 0)
														return;

												WorkQueue.Add (th);
												WaitQueue.Remove (th);
										}
					
					
										th.VoidEv = null;
										th.SEv = null;
					
										th.VoidEv += vev;
										th.SEv = Check;
					
					
										if (th.m_thread.ThreadState == ThreadState.Unstarted) {
												th.m_thread.Start ();
										} else if (th.m_thread.ThreadState == ThreadState.WaitSleepJoin) {
												th.WeakResume ();
										}
					
								} else {
										if (WaitQueue.Count + WorkQueue.Count <= Config.mIns.Thread_MaxSize) {
												var th = new KThread ();
												Push_ToWaitQueue (th);

												StartTask (vev);
										}
					
								}
				
				
						}

						void Check (object obj)
						{
								if (_stop)
										return;
				
								KThread th;
				
								lock (m_lock) {
										th = (KThread)obj;
										WaitQueue.Add (th);
										WorkQueue.Remove (th);
								}
				
								th.VoidEv = null;
								th.SEv = null;
				
								if (TaskQueue.Count > 0) {
										StartTask (GetFirstTask ());
								}
				
								th.WeakStop ();
				
						}
			
			
			
				}
		
		}
}


