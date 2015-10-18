using UnityEngine;
using System.Collections;
using Kubility;

public class SceneViewType : UIType
{
	public SceneViewType (string name) : base (name)
	{
		
	}
	
	public SceneViewType (string name, int value) : base (name, value)
	{
		
	}

	public static SceneViewType Login_N1_Scene = new SceneViewType("Login_N1_Scene");

}
