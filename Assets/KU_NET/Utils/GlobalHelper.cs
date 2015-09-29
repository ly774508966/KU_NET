using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Kubility;

/// <summary>
/// 存放未有附属拖拽对象且稳定的Global管理类
/// </summary>
public enum GameScene
{
	LogoScene = 0,
	Loading = 1,
	LoadBattleMap = 2,
	MapScene = 3,
	LoginScene = 4,
	BattleScene = 5,
}

public class GlobalHelper : MonoBehaviour
{
	public static GlobalHelper mIns;
	
	
	[HideInInspector]
	public GlobalMon global = null;
	
	public GameObject gCamera;
	
	public GameScene curGameScene = GameScene.LogoScene;
	public GameScene lastGameScene = GameScene.LogoScene;

	public int listsize = 0;
	private LinkedList<KeyValuePair<Type, int>> ComponentQueue = new LinkedList<KeyValuePair<Type, int>>();
	public List<Iobject> _InitObjList = new List<Iobject>();
	
	Iobject[] arr;

	VoidDelegate  fixedupdate;
	VoidDelegate  update;
	
//	public delegate void updateDelegate();
//	public delegate void FixedDelegate();
//	
//	private Dictionary<ComponentObject, updateDelegate> updateEvent = new Dictionary<ComponentObject, updateDelegate>();
//	private Dictionary<ComponentObject, FixedDelegate> fixedEvent = new Dictionary<ComponentObject, FixedDelegate>();
//	
//	private bool isUpdating;
//	private bool isFixUpdate;

	private VoidDelegate SceneChangeEv;
	
	public class ComponentObject
	{
		public Component com;
		public Type type;
		public GameObject gobj;
		
		public static implicit operator ComponentObject(Component comp)
		{
			ComponentObject p = new ComponentObject();
			p.com = comp;
			p.type = comp.GetType();
			p.gobj = comp.gameObject;
			return p;
		}
	}
	
	public enum TargetPlatform
	{
		None = 0,
		IOS = 8,
		ANDROID = 11,
	}
	
	public class GlobalMon
	{
		internal GameObject obj = null;
		internal GlobalMon()
		{
			obj = new GameObject("Global");
			DontDestroyOnLoad(obj);
			
		}
	}
	

	
	void Awake()
	{
		
		inspectorInstanceConstructor();
		DontDestroyOnLoad(gameObject);
		
		mIns = this;
		if (global == null)
		{
			global = new GlobalMon();
		}
	}
	
	void inspectorInstanceConstructor()
	{
		arr = _InitObjList.ToArray();
		
	}

	void Start()
	{    
		#if UNITY_ANDROID
		AddComponent<DownJoyPlugin>();
		#elif UNITY_IOS
		
		#endif
		
		
		checkList(curGameScene);
		
		new Task(StepAddObjs(), true);
		new Task(StepAddComponent(), true);
	}
	
	void checkList(GameScene scene)
	{
		
		for (int i = 0; i != arr.Length; ++i)
		{
			Iobject iobj = arr[i];
			if (iobj.Immidate || (scene != GameScene.LogoScene && scene == iobj.InitScene))
			{
				AddSubImmiate(iobj);
				
			}
		}
	}
	
	void OnLevelWasLoaded(int level)
	{
		LogMgr.Log("====NEXT Scene   " + (GameScene)level);
		UpdateSubs((GameScene)level);
	}
	

	
	void Update()
	{
				if(update != null)
					update ();
	}
	
	void FixedUpdate()
	{
				if (fixedupdate != null)
						fixedupdate ();

	}

	
	#region tools function

	public void UpdateSubs(GameScene scene)
	{
		checkList(scene);
		
		if(SceneChangeEv != null)
			SceneChangeEv();
		
		lastGameScene = curGameScene;
		//before this curGameScene is the last scene
		curGameScene = scene;
	}
	
	
	
	public void RegisterUpdate(VoidDelegate ev, TargetPlatform platform = TargetPlatform.None)
	{
		if (platform != TargetPlatform.None)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && platform != TargetPlatform.IOS)
			{
				
				return;
			}
			else if (Application.platform == RuntimePlatform.Android && platform != TargetPlatform.ANDROID)
			{
				
				return;
			}
		}

		update += ev;
	}
	
	public void UnRegisterUpdate(VoidDelegate ev)
	{
		update -= ev;
	}
	
	public void RegisterSceneDestroy( VoidDelegate ev)
	{
		SceneChangeEv+= ev;
	}
	
	public void RemoveSceneDestroy(VoidDelegate ev)
	{
		SceneChangeEv -= ev;
	}
	
	public void RegisterFixedUpdate( VoidDelegate ev, TargetPlatform platform = TargetPlatform.None)
	{
		
		
		if (platform != TargetPlatform.None)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && platform != TargetPlatform.IOS)
			{
				
				return;
			}
			else if (Application.platform == RuntimePlatform.Android && platform != TargetPlatform.ANDROID)
			{
				
				return;
			}
		}
		fixedupdate += ev;
	}

	public void UnRegisterFixedUpdate(VoidDelegate ev)
	{
		fixedupdate -= ev;
	}

	IEnumerator StepAddComponent()
	{
		yield return new WaitForSeconds(0.5f);
		while (ComponentQueue.Count > 0)
		{
			Type t = ComponentQueue.First.Value.Key;
			ComponentQueue.RemoveFirst();
			global.obj.AddComponent(t);
			
			yield return new WaitForSeconds(0.2f);
		}
		LogMgr.Log("=======AddComponent end");
		yield return null;
	}
	
	IEnumerator StepAddObjs()
	{
		yield return new WaitForSeconds(0.6f);
		GameScene cur = (GameScene)Application.loadedLevel;
		for (int i = 0; i != arr.Length; ++i)
		{
			Iobject iobj = arr[i];
			
			if (iobj.Immidate || iobj.InitScene != cur)
				continue;
			
			AddSubImmiate(iobj);
			yield return new WaitForSeconds(0.2f);
		}
		LogMgr.Log("=======StepAddObjs end ");
		yield return null;
	}
	
	public void AddSubImmiate(params Iobject[] objs)
	{
		for (int i = 0; i != objs.Length; ++i)
		{
			AddSubImmiate(objs[i]);
		}
	}
	
	public GameObject AddSub(string name)
	{
		Transform obj = global.obj.transform.FindChild(name);
		if (obj == null)
		{
			GameObject newObj = new GameObject(name);
			newObj.transform.parent = global.obj.transform;
			return newObj;
		}
		else
			return obj.gameObject;
	}
	
	void AddSubImmiate(Iobject iobj)
	{
		if (iobj.hasCreate)
			return;
		
		if (iobj.parent == null)
			global.obj.AddChild(iobj.obj);
		else
		{
			iobj.parent.gameObject.AddChild(iobj.obj);

		}
		
		iobj.hasCreate = true;
		
	}

	
	
	public T AddComponentImmediate<T>() where T : Component
	{
		if (global == null || global.obj == null)
		{
			LogMgr.Log("global or globalObject is Null");
			return null;
		}
		
		return global.obj.AddComponent<T>();
	}
	
	///<summary>
	///根据优先级初始化
	///</summary>
	public void AddComponent<T>(int level = 0) where T : Component
	{
		if (global == null || global.obj == null)
		{
			LogMgr.Log("global or globalObject is Null");
			return;
		}
		
		if (level == 0 || ComponentQueue.Count == 0)
		{
			ComponentQueue.AddLast(new KeyValuePair<Type, int>(typeof(T), level));
		}
		else
		{
			LinkedListNode<KeyValuePair<Type, int>> firstNode = ComponentQueue.First;
			if (firstNode.Value.Value > level)
			{
				ComponentQueue.AddFirst(new KeyValuePair<Type, int>(typeof(T), level));
			}
			else
			{
				ComponentQueue.AddAfter(firstNode, new KeyValuePair<Type, int>(typeof(T), level));
			}
			
		}
		
	}

	#endregion
	
}


public static class GlobalUtils
{
	public static void LoadScene(GameScene scene)
	{
		
		Application.LoadLevel(scene.ToString());
	}
}