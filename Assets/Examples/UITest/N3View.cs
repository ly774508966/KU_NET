using UnityEngine;
using System.Collections;
using Kubility;

public class N3View : MonoDelegateView
{
	protected override void Awake ()
	{
		base.Awake ();
		
		BaseView.Create<BaseView>(this,new HideTrans(gameObject));
	}
	
	public void ButtonClick()
	{

		ContentManager.mIns.Pop();
	}

}
