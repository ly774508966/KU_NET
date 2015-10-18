using UnityEngine;
using System.Collections;
using Kubility;
using UnityEngine.UI;
using UnityEngine.Events;

public class KMainView : MonoDelegateView {

	public Button User_Btn;
	public Button Server_Btn;
	public Button Game_Btn;

	protected override void Awake ()
	{
		User_Btn.AddListener(UserClick,gameObject);
		Server_Btn.AddListener(ServerClick);
		Game_Btn.AddListener(EnterClick);
		BaseView.Create<BaseView>(this,new HideTrans(gameObject));
		base.Awake ();

	}

	public void UserClick(GameObject btn)
	{
		LogMgr.Log("UserClick");
	}

	public void ServerClick(Button btn)
	{
		LogMgr.Log("ServerClick");
	}

	public void EnterClick(Button btn)
	{
		LogMgr.Log("EnterClick " +btn );
	}

}
