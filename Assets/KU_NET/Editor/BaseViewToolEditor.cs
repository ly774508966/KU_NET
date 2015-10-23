using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Kubility
{
	public class BaseViewToolEditor : EditorWindow 
	{
		[MenuItem("Tools/BaseViewTool")]
		public static void OpenBaseViewEditor()
		{
			BaseViewToolEditor window = EditorWindow.GetWindow<BaseViewToolEditor>();
			window.autoRepaintOnSceneChange = true;
			window.minSize = new Vector2(800,600);
			window.name = "BaseViewToolEditor";
			window.Focus();

			window.Init();
			window.Show();
		}

		#region vars
		SerializedObject serializedObject;
		SerializedProperty AutoPos;
		SerializedProperty Pos;
		SerializedProperty Trans;

		AbstractTrans AbsTrans;
		#endregion

		#region Editor
		string[] leftBtnsTitle = new string[]{"BaseView","Trans","Delegate"};
		int leftBtnsSelected =0;
		#endregion

		string SerializePath = Application.temporaryCachePath +"/KUI/";

		void Init()
		{

		}

		void OnGUI()
		{

			GUILayout.BeginArea (new Rect (Vector2.zero,new Vector2(200,600)));
			GUILayout.Space(10);
			leftBtnsSelected = GUILayout.Toolbar(leftBtnsSelected,leftBtnsTitle);
			OpenLeftBtns(leftBtnsSelected);

			GUILayout.EndArea ();
		}

		void PropertyField(SerializedProperty property)
		{
			if(property != null)
			{
				EditorGUILayout.PropertyField(property,true);
			}
		}

		void OpenLeftBtns(int select)
		{


			if(select == 0)
			{
//				EditorGUILayout.BeginVertical();
//
//				EditorGUILayout.EndVertical();
			}
			else if(select == 1)
			{

			}
			else if(select == 2)
			{

			}
		}
		
		
	}
}


