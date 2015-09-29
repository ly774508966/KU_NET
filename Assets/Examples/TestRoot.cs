using UnityEngine;
using System.Collections;
using Kubility;
using System.IO;
using System;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TestRoot : MonoSingleTon<TestRoot> {

	[SerializeField,SetPropertyAttribute("NGUI")]
	public bool m_NGUI;
	[SerializeField,SetPropertyAttribute("UGUI")]
	private bool m_UGUI;

	public bool NGUI
	{
		get
		{
			return m_NGUI;
		}
		set
		{
			m_NGUI = value;
			#if UNITY_EDITOR
			if(value)
			{

				FileStream stream = new FileStream(Application.dataPath+"/smcs.rsp",FileMode.OpenOrCreate,FileAccess.Write);
				stream.SetLength(0);
				var bys =System.Text.Encoding.UTF8.GetBytes("-define:KNGUI");
				stream.Write(bys,0,bys.Length);
				stream.Close();
				
				FileStream csstream = new FileStream(Application.dataPath+"/Examples/TestRoot.cs",FileMode.OpenOrCreate,FileAccess.Write);
				csstream.Seek(0,SeekOrigin.End);
				var c_bys =System.Text.Encoding.UTF8.GetBytes(" \n");
				csstream.Write(c_bys,0,c_bys.Length);
				csstream.Close();
				AssetDatabase.Refresh();
				
				
			}
			
			#endif
		}
	}

	public bool UGUI
	{
		get
		{
			return m_UGUI;
		}
		set
		{
			m_UGUI = value;

			#if UNITY_EDITOR
			if(value)
			{

				FileStream stream = new FileStream(Application.dataPath+"/smcs.rsp",FileMode.OpenOrCreate,FileAccess.Write);
				stream.SetLength(0);
				var bys =System.Text.Encoding.UTF8.GetBytes("-define:KUGUI");
				stream.Write(bys,0,bys.Length);
				stream.Close();

				FileStream csstream = new FileStream(Application.dataPath+"/Examples/TestRoot.cs",FileMode.OpenOrCreate,FileAccess.Write);
				csstream.Seek(0,SeekOrigin.End);
				var c_bys =System.Text.Encoding.UTF8.GetBytes(" \n");
				csstream.Write(c_bys,0,c_bys.Length);
				csstream.Close();
				AssetDatabase.Refresh();

			}

			#endif
		}
	}

	public GameObject NGUIGameobject;

	public GameObject UGUIGameObject;


	void Awake ()
	{
#if KUGUI
		LogMgr.LogError("UGUI");

#elif KNGUI
		LogMgr.LogError("NGUI");
#endif



		
		if(NGUI)
		{
			NGUIGameobject.SetActive(true);
			ContentManager.mIns.Push (NViewType.N1View);
		}
		else if(UGUI)
		{
			UGUIGameObject.SetActive(true);
			ContentManager.mIns.Push (MainViewType.U1View);
		}

	}
	
	
	public void ButtonClick (GameObject gobh, BaseEventData data)
	{
		if(NGUI)
		{
			ContentManager.mIns.Push (NViewType.N2View);
		}
		else if(UGUI)
		{
			ContentManager.mIns.Push (MainViewType.U2View);
		}

	}
} 
 
 
