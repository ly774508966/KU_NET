#define AUTO_ALIGN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kubility
{
	[Serializable]
	public class BaseView :ScriptableObject, KObjectInterface
	{
		private bool _Running;
		public bool isRunning
		{
			get
			{
				return _Running;
			}
			protected set
			{
				_Running = value;
			}
		}

		[HideInInspector]
		[SerializeField]
		private bool m_AutoPos;

		public bool AutoPos {
			get {
				return m_AutoPos;
			}
			set {
				m_AutoPos = value;
			}
		}

		[HideInInspector]
		[SerializeField]
		private Vector3 m_pos;

		public Vector3 Pos {
			get {
				return m_pos;
			}
			set {
				m_pos = value;
			}
		}

		[SerializeField]
		protected AbstractTrans
			_Trans;
				
		public AbstractTrans Trans
		{
			get {
				return _Trans;
			}
					
			set {
				_Trans = value;
			}
		}

		GameObject TargetGameObject;

		protected Dictionary<string ,VoidDelegate> customAntioms;
				#if AUTO_ALIGN
		protected Dictionary<UIBehaviour ,UIAlign> AutoBehaviour;
				#endif

		public static void Create<T> (MonoDelegateView  value ,AbstractTrans trans) where T :BaseView 
		{

			BaseView oldview = value.m_view;

			T view = (T)Activator.CreateInstance(typeof(T),new System.Object[]{trans});
			if(view.AutoBehaviour == null)
			{
				view.AutoBehaviour = new Dictionary<UIBehaviour, UIAlign>();
			}

			if(view.customAntioms == null)
			{
				view.customAntioms = new Dictionary<string, VoidDelegate>();
			}

			view.TargetGameObject = value.gameObject;

			if (oldview!= null && oldview.AutoPos) {
				view.AutoPos = oldview.AutoPos;
				view.Pos = oldview.Pos;
			}

			if (trans == null && oldview!= null && oldview._Trans != null) {
				view.Trans = oldview._Trans;
			} else if (trans != null) {
				view.Trans = trans;
			}
			value.m_view = view;
		}

		public BaseView()
		{

		}

		public BaseView (AbstractTrans  trans)
		{
			this._Trans = trans;
			this.customAntioms = new Dictionary<string, VoidDelegate> ();
			#if AUTO_ALIGN
			this.AutoBehaviour = new Dictionary<UIBehaviour, UIAlign> ();
			#endif
		}

		public void AddCustomAnimation (string Key, VoidDelegate ev)
		{
			if (customAntioms.ContainsKey (Key)) {
				LogMgr.LogWarning ("it contains this Animation");
			} else {
				customAntioms.Add (Key, ev);
			}
		}

		public void RemoveCustomAnimation (string Key, VoidDelegate ev)
		{
			if (customAntioms.ContainsKey (Key)) {
				customAntioms.Remove (Key);
			}

		}
		#if AUTO_ALIGN
		public void Push_AutoUIBehaviour (UIBehaviour behaviour, UIAlign align = UIAlign.CENTER)
		{
			if (AutoBehaviour.ContainsKey (behaviour))
				return;
						
			AutoBehaviour.Add (behaviour, align);
		}

		public void Remove_AutoUIBehaviour (UIBehaviour behaviour, UIAlign align = UIAlign.CENTER)
		{
			if (AutoBehaviour.ContainsKey (behaviour)) {
				AutoBehaviour.Remove (behaviour);
			}
		}

		#endif


		public virtual void Visit (object obj)
		{

		}

		public virtual void OnCreate ()
		{
			isRunning = true;
			if (Trans != null)
				Trans.OnCreateTrans ();

		}

		public virtual void OnPause ()
		{
			isRunning = false;
			if (Trans != null)
				Trans.OnPauseTrans ();
		}

		public virtual void OnResume ()
		{
			isRunning = true;
			if (Trans != null)
				Trans.OnResumeTrans ();
		}

		public virtual void OnDestroy ()
		{
			isRunning =false;
			if (Trans != null)
				Trans.OnDestroyTrans ();
		}

		public virtual void OnEnter ()
		{
			isRunning =true;
			if(AutoPos
			   && TargetGameObject != null
			   )
			{
				TargetGameObject.transform.position = Pos;
			}
#if KUGI
#if AUTO_ALIGN
			Dictionary<UIBehaviour,UIAlign>.Enumerator enumerator = AutoBehaviour.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				UIBehaviour behaviour = enumerator.Current.Key;
				UIAlign align = enumerator.Current.Value;
				behaviour.AutoAlignWithRectTrans (align);
			}

#endif

#endif

			if (Trans != null)
				Trans.OnEnterTrans ();


		}

		public virtual void OnExit ()
		{
			isRunning =false;
			if (Trans != null)
				Trans.OnExitTrans ();
		}
	}

	public class MonoDelegateView:MonoEventsBehaviour<MonoDelegateView>
	{

		public BaseView m_view;

		protected override void Awake ()
		{
			KObject.KObjectManager.mIns.CallKobjectOnCreate (m_view);
		}

		protected override void OnDestroy ()
		{
			KObject.KObjectManager.mIns.CallKobjectOnDestroy (m_view);
		}


	}
}


