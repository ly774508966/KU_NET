using UnityEngine;
using System.Collections;

namespace Kubility
{
	public class ContentFactory
	{
		public static Content Create (UIType type)
		{
			Content con = new Content ();
			con.m_viewType = type;
			if (type == MainViewType.U2View) {

				con.m_PrefabPath = "Prefab/U2View";
				return con;
			} else if (type == MainViewType.U3View) {

				con.m_PrefabPath = "Prefab/U3View";
				return con;
			} else if (type == MainViewType.U1View) {

				con.m_PrefabPath = "Prefab/U1View";
				return con;
			} else if (type == NViewType.N1View) {

				con.m_PrefabPath = "Prefab/N1View";
				return con;
			} else if (type == NViewType.N2View) {

				con.m_PrefabPath = "Prefab/N2View";
				return con;
			} else if (type == NViewType.N3View) {
				con.m_PrefabPath = "Prefab/N3View";
				return con;
			}
				
						
			throw new System.ArgumentException ("type error");

		}
	}
}


