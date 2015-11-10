using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace Kubility.Editor
{
	public class KGUILayout
	{
		enum UILayout
		{
			Auto,
			End,
			Group,
			Vertical,
			Horizontal,
			ScrollView,
		}

		enum EditorMode
		{
			EditorWindow,
			Editor,
			Other,
		}

		static Vector2 startPos;
		static float LeftWid;
		static float LeftHeight;

		static Stack<Rect> AreaStack = new Stack<Rect>();

		static EditorMode mode = EditorMode.EditorWindow;

		const float default_height = 20;
		const float default_wid = 20;

		public static void ListeningEditor(EditorWindow window)
		{
			mode = EditorMode.EditorWindow;
			BeginArea(window.position);


			
		}

		public static void ListeningEditor(UnityEditor.Editor editor)
		{

			mode = EditorMode.Editor;

		}

		#region area

		public static void BeginArea(Rect rect)
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
			{
				UIrect = rect,
				UIMode = UILayout.Group,
			});
		}

		public static void EndArea()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.End,
			});
		}

		public static void BeginVertical()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.Vertical,
			});
		}

		public static void EndVertical()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.Vertical,
			});
		}

		public static void BeginHorizontal()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.Horizontal,
			});
		}

		public static void EndHorizontal()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.End,
			});
		}

		public static void BeginScrollView(ref Vector2 pos )
		{
			KGUILayoutManager.mIns.PushDrawCmd<Vector2>(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.ScrollView,
			}
			,
			delegate(Vector2 obj) 
			{
				pos = obj;
			}
			);
		}
		
		public static void EndScrollView()
		{
			KGUILayoutManager.mIns.PushDrawCmd(
				new GUICMD()
				{
				UIrect = null,
				UIMode = UILayout.End,
			});
		}

		#endregion

		struct GUICMD :IEquatable<GUICMD>
		{

			public Rect? UIrect;
			public Action<bool> callback;
			public UILayout UIMode;
			public ValueType Params;
			public string Title;
			public string Content;

			public bool Equals (GUICMD other)
			{
				if(UIrect != other.UIrect) return false;
				if(callback != other.callback) return false;
				if(UIMode != other.UIMode) return false;
				if(Params != other.Params) return false;
				if(Title != other.Title) return false;
				if(Content != other.Content) return false;
				return true;
			}
		}

		class KGUILayoutManager :SingleTon<KGUILayoutManager>
		{
			List<GUICMD> DrawList = new List<GUICMD>();

			public void PushDrawCmd(GUICMD cmd)
			{

			}

			public void PushDrawCmd<T>(GUICMD cmd,Action<T> callback)
			{
				
			}

			public void PopDrawCmd()
			{

			}

			public void PopDrawCmd<T>(Action<T> callback)
			{
				
			}

			void Draw()
			{

			}
		}
  	}
}


