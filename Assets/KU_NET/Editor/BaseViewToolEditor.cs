using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

namespace Kubility
{

	public enum ToolMenu
	{
		Normal,
		BaseView,
		Trans,
	}

	public class BaseViewToolEditor : EditorWindow 
	{
		[MenuItem("Tools/BaseViewTool")]
		public static void OpenBaseViewEditor()
		{
			BaseViewToolEditor window = EditorWindow.GetWindow<BaseViewToolEditor>();
			window.autoRepaintOnSceneChange = true;
			window.minSize = new Vector2(800,600);
			window.name = "BaseViewToolEditor";
			window.titleContent = new GUIContent("BaseViewTool");
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
		string[] leftBtnsTitle = new string[]{
			"BaseView",
			"Trans",
			"Delegate"
		};
		int leftBtnsSelected =0;

		string[] SearchParams =new string[]{
			"StartsWith",
			"EndsWith",
			"Contains"
		};

		#endregion

		Vector3 mCurrentMousePosition;
		Rect mGraphRect ;
		Vector2 GrapEachSize = new Vector2(100,100);

		string SerializePath = Application.temporaryCachePath +"/KUI/";
		string search;

		Vector2 scrollPosition;
		Vector2 GraphRightPos;
		Vector2 GraphLeftPos;


		Rect oldPosition;

		int frames =0;
		float alltime =0f;
		float fps =0f;

		float starttime =0f;
		float deltatime =1f;
		float clickTime =0f;

		Rect windowRect = new Rect (100 + 100, 100, 100, 100);
		Rect windowRect2 = new Rect (100, 100, 100, 100);
		public static float Toolheight = 20f;
		const float default_wid = 200f;
		const float default_height = 600f;


		Vector3[] lines1;
		Vector3[] lines2;

		Color lineCol = new Color(0.5f,0.5f,0.2f);

		Texture2D BackGround;

		GenericMenu RightClickMenu = new GenericMenu();

		List<BaseViewEditorItem> BaseViewList = new List<BaseViewEditorItem>();
		List<TransEditorItem> TransList = new List<TransEditorItem>();
		List<DelegateEditorItem>  DelegateList = new List<DelegateEditorItem>();


		void Init()
		{
			oldPosition = position;
			mGraphRect = new Rect(default_wid,Toolheight,default_height,default_height -Toolheight*2);
			ResizeScreen(position);

		}

		void OnGUI()
		{
			this.mCurrentMousePosition = Event.current.mousePosition;
			
			GUILayout.BeginArea(new Rect(3,3,200,mGraphRect.height));
			GUILayout.BeginVertical();
			leftBtnsSelected = GUILayout.Toolbar(leftBtnsSelected,leftBtnsTitle,EditorStyles.toolbarButton,GUILayout.Width(180));
			
			GUITools.CreateSearChText(ref search,160);
			OpenLeftBtns(leftBtnsSelected);

			//ScrollView
			GUILayout.BeginScrollView(scrollPosition);
			
			GUILayout.EndScrollView();
			
			GUILayout.EndVertical();
			GUILayout.EndArea();

			DrawDraphTop();
			DrawDraphArea();
			DrawDraphBottom();
			HandleEvents();
		}

		
		void Update()
		{
			
			++frames;
			float delta = (Time.realtimeSinceStartup - starttime);
			alltime +=  Time.timeScale /delta ;
			starttime = Time.realtimeSinceStartup;
			deltatime -= delta;
			
			if(deltatime <0f && frames >0)
			{
				fps = alltime /frames;
				alltime =0;
				frames=0;
				deltatime =1f;
				
			}
			
			
			if(oldPosition != position)
			{
				ResizeScreen(position);
			}
			
			oldPosition = position;


			
		}

		void HandleEvents()
		{
			if(Event.current != null && Event.current.isMouse && Event.current.button ==1)
			{

				if(mGraphRect.Contains(Event.current.mousePosition))
				{
					RightClickEvent();
				}


			}
		}

		void ResizeScreen(Rect scrennsize)
		{
//			LogMgr.LogError("New size = "+ scrennsize);
			if( Mathf.Abs(oldPosition.width - scrennsize.width) > 0.01
			   || Mathf.Abs(oldPosition.height- scrennsize.height) >25)//fix
			{
//				LogMgr.LogError("change");
				mGraphRect.y = Toolheight;
				mGraphRect.size = new Vector2(Mathf.Max(0, scrennsize.size.x -Toolheight),Mathf.Max(0,scrennsize.size.y -Toolheight*2));
				
				SetTexture((int)mGraphRect.width,(int)mGraphRect.height);
				SetLinesPos();
			}

		}


		void RightClickEvent()
		{
			LogMgr.Log("right click");

			RightClickMenu.AddDisabledItem(new GUIContent("disableitem" ));
			RightClickMenu.AddItem(new GUIContent("item1" ),false,Callback,"item1");
			RightClickMenu.AddSeparator("SubMenu/");
			RightClickMenu.AddItem(new GUIContent("SubMenu/item2" ),false,Callback,"item 2");
			RightClickMenu.ShowAsContext();
		}

		public void Callback (object obj) {
			Debug.Log ("Selected: " + obj);
		}


		void WindowFunction (int windowID) 
		{

			GUILayout.TextField("test  "+ windowID);

			if(Event.current != null && Event.current.isMouse && Event.current.button == 0)
			{
				GUI.DragWindow();
			}
			else if(Event.current != null && Event.current.isMouse && Event.current.button == 1)
			{
				RightClickEvent();
			}

		}

		bool listtoggle;
		bool foldout ;
		private void DrawDraphTop()
		{

			GUILayout.BeginArea(new Rect(200,0,mGraphRect.width-200+Toolheight,Toolheight));
			GUILayout.BeginHorizontal();

			listtoggle = GUILayout.Toggle(listtoggle,"","ListToggle");

			GUIStyle addStyle = GUI.skin.FindStyle("OL Plus");
			GUIStyle MinusStyle = GUI.skin.FindStyle("OL Minus");
			GUIStyle FoldOut = GUI.skin.FindStyle("ProjectBrowserSubAssetExpandBtn");


			if(GUILayout.Button("+",EditorStyles.toolbarButton))
			{

			}

			if(GUILayout.Button("-",EditorStyles.toolbarButton))
			{
				
			}

			if(GUILayout.Button("",addStyle))
			{
				
			}

			if(GUILayout.Button("",MinusStyle))
			{
				
			}

			if(GUILayout.Button("--",EditorStyles.toolbarButton))
			{
				
			}

//
//
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawDraphBottom()
		{
			GUILayout.BeginArea(new Rect(200,20+ mGraphRect.height,mGraphRect.width-200+Toolheight,Toolheight));
			GUILayout.BeginHorizontal();
			GUIStyle addStyle = GUI.skin.FindStyle("OL Plus");
			GUIStyle MinusStyle = GUI.skin.FindStyle("OL Minus");
			GUIStyle FoldOut = GUI.skin.FindStyle("ProjectBrowserSubAssetExpandBtn");
			
			
			if(GUILayout.Button("+",EditorStyles.toolbarButton))
			{
				
			}
			
			if(GUILayout.Button("-",EditorStyles.toolbarButton))
			{
				
			}
			
			if(GUILayout.Button("",addStyle))
			{
				
			}
			
			if(GUILayout.Button("",MinusStyle))
			{
				
			}
			
			if(GUILayout.Button("--",EditorStyles.toolbarButton))
			{
				
			}
			
			//
			//
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawDraphArea()
		{
			EditorZoomArea.Begin(mGraphRect,1f);
			Handles.BeginGUI();
			if(BackGround != null)
				GUI.DrawTexture(new Rect(0,0,BackGround.width,BackGround.height),BackGround);

			GUILayout.Label("FPS : "+string.Format("{0:F2}",fps));

			Handles.color = lineCol;

			Handles.DrawLine(new Vector3(0,0,0),new Vector3(mGraphRect.width,0,0));
			Handles.DrawLine(new Vector3(0,0,0),new Vector3(0,mGraphRect.height-1,0));
			Handles.DrawLine(new Vector3(mGraphRect.width,0,0),new Vector3(mGraphRect.width,mGraphRect.height-1,0));
			Handles.DrawLine(new Vector3(0,mGraphRect.height-1,0),new Vector3(mGraphRect.width,mGraphRect.height-1,0));

			Handles.DrawAAPolyLine(2,lines1);
			Handles.DrawAAPolyLine(2,lines2);

			Handles.DrawBezier(windowRect.center, windowRect2.center, new Vector2(windowRect.xMax + 50f,windowRect.center.y), new Vector2(windowRect2.xMin - 50f,windowRect2.center.y),Color.red,null,5f);
			Handles.EndGUI();

			BeginWindows();
			windowRect = GUI.Window (0, windowRect, WindowFunction, "Box1");
			windowRect2 = GUI.Window (1, windowRect2, WindowFunction, "Box2");
			
			EndWindows();

			EditorZoomArea.End();

		}

		private bool GetMousePositionInGraph()
		{

			if (!this.mGraphRect.Contains(mCurrentMousePosition))
			{
				return false;
			}

			return true;
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
				ShowBaseViews();
			}
			else if(select == 1)
			{

			}
			else if(select == 2)
			{

			}
		}


		bool CreateViewFoldOut ;
		bool CreateViewFoldOut1 ;
		bool CreateViewFoldOut2 ;
		bool value;
		void ShowBaseViews()
		{
			GUILayout.BeginVertical();
			CreateViewFoldOut =GUITools.FoldOut(new Rect(0,40,200,20), CreateViewFoldOut,"Create");

			if(CreateViewFoldOut)
			{
				CreateViewFoldOut1 =GUITools.FoldOut(new Rect(0,60,200,20), CreateViewFoldOut1,"Create/Create");
				if(CreateViewFoldOut1)
				{
					CreateViewFoldOut2 =GUITools.FoldOut(new Rect(0,80,200,20), CreateViewFoldOut2,"Create/Create/create");
					CreateViewFoldOut2 =GUITools.FoldOut(new Rect(0,100,200,20), CreateViewFoldOut2,"Create/Create/test");
				}
			}



			GUILayout.EndVertical();
		}

		void SetTexture(int wid,int height)
		{
			wid +=100;//reserve
			Texture2D tex = new Texture2D(wid ,height,TextureFormat.RGBA32,false,true);
			Color[] cols = new Color[wid * height];
			for(int i=0; i < cols.Length; ++i)
			{
				cols[i]= Color.black;
			}
			tex.SetPixels(cols);
			tex.Apply();
			
			BackGround = tex;
		}
		
		void SetLinesPos()
		{
			int tw = (int)mGraphRect.width;
			int th = (int)mGraphRect.height;
			
			int w =  20 ;
			int h =  20;
			
			int sw = tw /w *2; // 4 + (w-1) *2;
			int sh = th /h *2; //4 + (h-1) *2;
			
			lines1 = new Vector3[sw ];
			lines2 = new Vector3[sh ];
			
			
			for(int i=0; i < sw; ++i)
			{
				int k = i %4;
				if( k==0)
				{
					lines1[i] = new Vector3 ((2 *w) *(i/4),0,0);
				}
				else if(k ==1)
				{
					lines1[i] = new Vector3 ((2 *w) *(i/4),th,0);
				}
				else if( k== 2)
				{
					lines1[i] = new Vector3 ((2 *w) *(i/4) + w,th,0);
				}
				else if(k ==3)
				{
					lines1[i] = new Vector3 ((2 *w) *(i/4) +w,0,0);
				}
			}
			
			for(int i=0; i < sh; ++i)
			{
				int k = i %4;
				if( k==0)
				{
					lines2[i] = new Vector3 (tw,(2 *h) *(i/4),0);
				}
				else if(k ==1)
				{
					lines2[i] = new Vector3 (0,(2 *h) *(i/4),0);
				}
				else if( k== 2)
				{
					lines2[i] = new Vector3 (0,(2 *h) *(i/4)+h,0);
				}
				else if(k ==3)
				{
					lines2[i] = new Vector3 (tw,(2 *h) *(i/4)+h,0);
				}
			}
			
			
		}
		
		
	}
}


