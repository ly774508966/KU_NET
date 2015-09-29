using UnityEngine;

[System.Serializable]
public class Iobject
{
	[SerializeField]
	public GameObject obj;
	[SerializeField]
	public Transform	parent;
	
	public bool Immidate =false;

	public bool hasCreate = false;

	public GameScene InitScene =GameScene.LogoScene;
	
	public Iobject()
	{
		this.obj = null;
		this.parent = null;
		this.Immidate = false;
		this.InitScene = GameScene.LogoScene;
		this.hasCreate = false;
	}

	public Iobject(Iobject copy)
	{
		this.obj = copy.obj;
		this.parent = copy.parent;
		this.Immidate=copy.Immidate;
		this.InitScene = copy.InitScene;
		this.hasCreate = copy.hasCreate;
	}
	
	public Iobject(Transform parentTrans,GameObject pobj ,GameScene pscene,bool isImmidate =false)
	{
		this.obj = pobj;
		this.parent =  parentTrans;
		this.Immidate= isImmidate;
		this.InitScene = pscene;
		this.hasCreate = false;
		if(isImmidate && parent != null)
			obj.transform.parent = this.parent;
		
	}
	
		
}