using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

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
		Rect mGraphRect = new Rect(200,20,600,560);
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

		Rect windowRect = new Rect (100 + 100, 100, 100, 100);
		Rect windowRect2 = new Rect (100, 100, 100, 100);
		public static float height = 32.0f;
		public static float width = 32.0f;

		Vector3[] lines1;
		Vector3[] lines2;

		Color lineCol = new Color(0.5f,0.5f,0.2f);

		Texture2D BackGround;


		void Init()
		{
			oldPosition = position;

			ResizeScreen(position);

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




		void ResizeScreen(Rect scrennsize)
		{
			LogMgr.LogError("New size = "+ scrennsize);
			if( mGraphRect.size != scrennsize.size)
			{
				mGraphRect.size = new Vector2(Mathf.Max(0, scrennsize.size.x -20),Mathf.Max(0,scrennsize.size.y -40));
				
				SetTexture((int)mGraphRect.width,(int)mGraphRect.height);
				SetLinesPos();
			}


		}

		void OnGUI()
		{
			this.mCurrentMousePosition = Event.current.mousePosition;

			GUILayout.BeginArea(new Rect(3,3,200,mGraphRect.height));
			GUILayout.BeginVertical();
			leftBtnsSelected = GUILayout.Toolbar(leftBtnsSelected,leftBtnsTitle,EditorStyles.toolbarButton,GUILayout.Width(180));

			GUITools.CreateSearChText(ref search,160);
			OpenLeftBtns(leftBtnsSelected);

			//leftBtnsSelected = GUITools.CreateSearChTextField_GUI(new Vector2(10,22),leftBtnsSelected,SearchParams,ref search);

			GUILayout.BeginScrollView(scrollPosition);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();
			GUILayout.EndArea();
//			Repaint();

			GUILayout.BeginArea(new Rect(203,0,mGraphRect.width,20));
			GUILayout.Label("here will place some btns ");
			GUILayout.EndArea();

			DrawDraphArea();


		}

		void Update()
		{
			
			++frames;
			float delta = (Time.realtimeSinceStartup - starttime);
			alltime +=  Time.timeScale /delta ;
			starttime = Time.realtimeSinceStartup;
			deltatime -= delta;
			
			if(deltatime <0f)
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


		void WindowFunction (int windowID) 
		{

			GUILayout.TextField("test  "+ windowID);

			GUI.DragWindow();
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
//			Vector3 pos = new Vector3(0,0,0);
//			for(float y = pos.y - 800f; y < pos.y + 800f; y+= height)
//			{
//				Handles.DrawLine(new Vector3(-1000000f, Mathf.Floor(y / height) * height, 0f),
//				                 new Vector3(1000000f, Mathf.Floor(y / height) * height, 0f));
//			} // End draw vertical lines
//			
//			for(float x = pos.x - 800f; x < pos.x + 800f; x += width)
//			{
//				Handles.DrawLine(new Vector3(-1000000f, Mathf.Floor(x / width) * width, 0f),
//				                 new Vector3(1000000f, Mathf.Floor(x / width) * width, 0f));
//				
//			} // End draw horizontal lines
			Handles.DrawAAPolyLine(12,new Vector3[]{new Vector3(10,0,0),new Vector3(10,100,0),new Vector3(110,100,0),new Vector3(110,0,0)});
			Handles.DrawBezier(windowRect.center, windowRect2.center, new Vector2(windowRect.xMax + 50f,windowRect.center.y), new Vector2(windowRect2.xMin - 50f,windowRect2.center.y),Color.red,null,5f);
			Handles.EndGUI();

			BeginWindows();
			windowRect = GUI.Window (0, windowRect, WindowFunction, "Box1");
			windowRect2 = GUI.Window (1, windowRect2, WindowFunction, "Box2");
			
			EndWindows();

//			Vector3[] vecs= new Vector3[14] ;
//
//			for(int i=0; i < vecs.Length; ++i)
//			{
//				int j = i %3;
//				if( j==0)
//				{
//					vecs[i] = new Vector3( i/3 *20,);
//				}
//				else if( j== 1)
//				{
//
//				}
//				else if (j ==2)
//				{
//
//				}
//			}
//			Handles.BeginGUI();
//
//			Handles.DrawAAPolyLine(new Vector3[]{new Vector3(0,0,0),new Vector3(100,100,0),new Vector3(200,100,0)});
//			Handles.DrawLine(new Vector3(0,0,0),new Vector3(30000,0,0));
			
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


