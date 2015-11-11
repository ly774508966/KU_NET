using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Kubility.Editor
{
	public class MonoDelegateViewMenu
	{
		bool foldout =false;
		bool isPrefab =false;

		public float ShowEditorContent(MonoDelegateView view,Rect rect )
		{
			float delta = 60+(foldout?80:0);
//			Rect prect = new Rect(rect.x,rect.y,rect.width, rect.height+delta);
			foldout = EditorGUILayout.Foldout(foldout,"Base Vars");

			if(foldout)
			{
				view.enabled =EditorGUILayout.Toggle("enabled",view.enabled);
				bool value =EditorGUILayout.Toggle("Actived",view.gameObject.activeSelf);
				if(value != view.gameObject.activeSelf)
				{
					view.gameObject.SetActive(value);
				}


				PrefabType prefabtype =  PrefabUtility.GetPrefabType(view.gameObject);
				EditorGUILayout.EnumPopup("PrefabType: ",prefabtype );
				isPrefab = prefabtype != PrefabType.None;

				EditorGUILayout.Toggle("isPrefab",isPrefab);
			}

//			GUILayout.BeginHorizontal();
			view.m_view = (BaseView)EditorGUILayout.ObjectField(view.m_view,typeof(BaseView),true);
//
			if(GUILayout.Button("show", GUILayout.Width(80)))
			{

			}
//			GUILayout.EndArea();
			return delta;
		}
	}
}


