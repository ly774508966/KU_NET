#if UNITY_EDITOR
//#define SHOW_LOG
//#define DEBUG
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubility
{

	public interface KObjectInterface
	{
		bool isRunning
		{
			get;
		}

		void OnCreate ();

		void OnPause ();

		void OnResume ();
		
		void OnDestroy ();
		
		void OnEnter ();
		
		void OnExit ();

	}

	public class KObject:KObjectInterface
	{
#if DEBUG 
		static List<object> objlist  = new List<object>();
#endif
		static int count;
				
		protected bool _isRunning;

		public bool isRunning {
			get {
				return _isRunning;
			}
		}

		protected KObject ()
		{
			count++;
#if DEBUG
			objlist.Add(this);
#endif
		}

		public static void Dump ()
		{
			LogMgr.Log ("Object count left =" + count.ToString ());
#if	DEBUG

			for(int i=0; i < objlist.Count;++i)
			{
				KThread th = objlist[i] as KThread;
				if(th != null)
					LogMgr.Log(th.GetUID());
				else
					LogMgr.Log(objlist[i].GetType());
			}

#endif
		}

		public virtual void OnCreate ()
		{
			LogMgr.Log("cr >> "+ this.GetType());
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnCreate");
			#endif
		}

		public virtual void OnPause ()
		{
			_isRunning = false;
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnPause");
			#endif
		}

		public virtual void OnResume ()
		{
			_isRunning = true;
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnResume");
			#endif
		}

		public virtual void OnDestroy ()
		{
			_isRunning = false;
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnDestroy");
			#endif
#if DEBUG
			objlist.Remove(this);
#endif
			count--;

		}

		public virtual void OnEnter ()
		{
			_isRunning = true;
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnEnter");
			#endif
		}

		public virtual void OnExit ()
		{
			_isRunning = false;
			#if SHOW_LOG
			LogMgr.Log (this.ToString () + " OnExit");
			#endif
		}



		public class KObjectManager:SingleTon<KObjectManager>
		{

			public void CallKobjectOnCreate (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnCreate ();
			}

			public void CallKobjectOnPause (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnPause ();
			}

			public void CallKobjectOnResume (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnResume ();
			}

			public void CallKobjectOnDestroy (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnDestroy ();
			}

			public void CallKobjectOnEnter (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnEnter ();
			}

			public void CallKobjectOnExit (KObjectInterface kobj)
			{
				if (kobj != null)
					kobj.OnExit ();
			}
		}

	}
}



