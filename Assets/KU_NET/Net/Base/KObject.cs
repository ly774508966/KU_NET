#if UNITY_EDITOR
//#define SHOW_LOG
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
//		static List<object> objlist  = new List<object>();
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
//			objlist.Add(this);

		}

		public static void Dump ()
		{
			LogMgr.Log ("Object count left =" + count.ToString ());

//			foreach(var sub in objlist)
//			{
//				LogMgr.Log(sub.GetType());
//			}
		}

		public virtual void OnCreate ()
		{
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



