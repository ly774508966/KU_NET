using UnityEngine;
using System.Collections;
using Kubility;

public class ContentFactory:ContentFactoryInterface
{
	
	public Content Create(UIType type)
	{
		Content con = new Content();
		con.m_viewType = type;
		if (type == MainViewType.U2View)
		{
			
			con.m_PrefabPath = "Prefab/U2View";
			return con;
		}
		else if (type == MainViewType.U3View)
		{
			
			con.m_PrefabPath = "Prefab/U3View";
			return con;
		}
		else if (type == MainViewType.U1View)
		{
			
			con.m_PrefabPath = "Prefab/U1View";
			return con;
		}
		else if (type == NViewType.N1View)
		{
			
			con.m_PrefabPath = "Prefab/N1View";
			return con;
		}
		else if (type == NViewType.N2View)
		{
			
			con.m_PrefabPath = "Prefab/N2View";
			return con;
		}
		else if (type == NViewType.N3View)
		{
			con.m_PrefabPath = "Prefab/N3View";
			return con;
		}
		else if( type== SceneViewType.Login_N1_Scene)
		{
			con.m_PrefabPath = "Prefab/MainView";
			return con;
		}
		
		
		throw new System.ArgumentException("type error");
		
	}
}

public class SceneRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ContentManager.mIns.Facotry = new ContentFactory();
		ContentManager.mIns.Push(SceneViewType.Login_N1_Scene);
	}

}
