#if UNITY_EDITOR
//#define SHOW_LOG
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubility
{
		public class KObject
		{
				
				static int count;
				
				protected bool _isRunning;

				public bool isRunning {
						get {
								return _isRunning;
						}
				}

				public KObject ()
				{
						count++;

				}

				public static void Dump ()
				{
						LogMgr.Log ("Object count left =" + count.ToString());
				}

				protected virtual void OnCreate ()
				{
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnCreate");
						#endif
				}

				protected virtual void OnPause ()
				{
						_isRunning = false;
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnPause");
						#endif
				}

				protected virtual void OnResume ()
				{
						_isRunning = true;
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnResume");
						#endif
				}

				protected virtual void OnDestroy ()
				{
						_isRunning = false;
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnDestroy");
						#endif

						count--;

				}

				protected virtual void OnEnter ()
				{
						_isRunning = true;
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnStart");
						#endif
				}

				protected virtual void OnExit ()
				{
						_isRunning = false;
						#if SHOW_LOG
						LogMgr.Log (this.ToString () + " OnExit");
						#endif
				}



				public class KObjectManager:SingleTon<KObjectManager>
				{

						public void CallKobjectOnCreate (KObject kobj)
						{
								if (kobj != null)
										kobj.OnCreate ();
						}

						public void CallKobjectOnPause (KObject kobj)
						{
								if (kobj != null)
										kobj.OnPause ();
						}

						public void CallKobjectOnResume (KObject kobj)
						{
								if (kobj != null)
										kobj.OnResume ();
						}

						public void CallKobjectOnDestroy (KObject kobj)
						{
								if (kobj != null)
										kobj.OnDestroy ();
						}

						public void CallKobjectOnEnter (KObject kobj)
						{
								if (kobj != null)
										kobj.OnEnter ();
						}

						public void CallKobjectOnExit (KObject kobj)
						{
								if (kobj != null)
										kobj.OnExit ();
						}
				}

		}
}



