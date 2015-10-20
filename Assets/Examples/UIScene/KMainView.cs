using UnityEngine;
using System.Collections;
using Kubility;
using UnityEngine.UI;
using UnityEngine.Events;

public class KMainView : MonoDelegateView {

	public Button User_Btn;
	public Button Server_Btn;
	public Button Game_Btn;

	public Animator an;

	protected override void Awake ()
	{
		base.Awake ();
		User_Btn.AddListener(UserClick,gameObject);
		Server_Btn.AddListener(ServerClick);
		Game_Btn.AddListener(EnterClick);
		BaseView.Create<BaseView>(this,new HideTrans(gameObject));


	}

	public void UserClick(GameObject btn)
	{
		LogMgr.Log("UserClick");
	}

	public void ServerClick(Button btn)
	{

		ContentManager.mIns.Push(SceneViewType.SelectServerView);
	}

	public void EnterClick(Button btn)
	{
		LogMgr.Log("EnterClick " +btn );
	}

}
