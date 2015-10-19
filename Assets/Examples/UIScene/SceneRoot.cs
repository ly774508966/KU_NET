using UnityEngine;
using System.Collections;
using Kubility;

public class SceneRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ContentManager.mIns.Facotry = new ContentFactory();
		ContentManager.mIns.Push(SceneViewType.Login_N1_Scene);
	}

}
