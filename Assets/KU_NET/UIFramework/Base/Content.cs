using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
	/// <summary>
	/// Content.
	/// </summary>
	public struct Content :IEquatable<Content>
	{
		public UIType m_viewType;
		public string m_PrefabPath;
		public string[] m_SubPrefabPaths;

		public bool Equals (Content obj)
		{
			return m_viewType == obj.m_viewType && m_PrefabPath.Equals (obj.m_PrefabPath);
		}

	}

	public class ContentManager:SingleTon<ContentManager>
	{
		Stack<Content> m_stack = new Stack<Content> ();
		ContentFactoryInterface factory;

		public ContentFactoryInterface Facotry {
			get {
				return factory;
			}
			set {
				factory = value;
			}
		}

		public void Push (UIType Type)
		{
			if (factory == null)
				throw new CustomException ("pleasze init Factory first");

			Content con = factory.Create (Type);

			if (string.IsNullOrEmpty (con.m_PrefabPath)) {
				LogMgr.LogError ("Content TargetPath cant be null");
				return;
			} else if (m_stack.Count > 0) {
				Content curContent = m_stack.Peek ();
				BaseView curView = UIManager.mIns.TryGet (curContent.m_viewType, curContent.m_PrefabPath, con.m_SubPrefabPaths);
				KObject.KObjectManager.mIns.CallKobjectOnPause (curView);
			}

			m_stack.Push (con);
			BaseView NextView = UIManager.mIns.TryGet (con.m_viewType, con.m_PrefabPath, con.m_SubPrefabPaths);
			KObject.KObjectManager.mIns.CallKobjectOnEnter (NextView);
		}

		public void Push (Content con)
		{

			if (string.IsNullOrEmpty (con.m_PrefabPath)) {
				LogMgr.LogError ("Content TargetPath cant be null");
				return;
			} else if (m_stack.Count > 0) {
				Content curContent = m_stack.Peek ();
				BaseView curView = UIManager.mIns.TryGet (curContent.m_viewType, curContent.m_PrefabPath, con.m_SubPrefabPaths);
				KObject.KObjectManager.mIns.CallKobjectOnPause (curView);
			}

			m_stack.Push (con);
			BaseView NextView = UIManager.mIns.TryGet (con.m_viewType, con.m_PrefabPath, con.m_SubPrefabPaths);
			KObject.KObjectManager.mIns.CallKobjectOnEnter (NextView);
		}

		public void Pop ()
		{
			if (m_stack.Count > 0) {
				Content curContent = m_stack.Pop ();
				BaseView curView = UIManager.mIns.TryGet (curContent.m_viewType, curContent.m_PrefabPath);
				KObject.KObjectManager.mIns.CallKobjectOnExit (curView);
			}

			if (m_stack.Count > 0) {
				Content LastContent = m_stack.Peek ();
				BaseView LastView = UIManager.mIns.TryGet (LastContent.m_viewType, LastContent.m_PrefabPath);
				KObject.KObjectManager.mIns.CallKobjectOnResume (LastView);
			}
		}

		public Content? GetCurent ()
		{
			if (m_stack.Count > 0) {
				return m_stack.Peek ();
			}
			return null;
		}

	}
	
}


