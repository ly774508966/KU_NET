using UnityEngine;
using System.Collections;
using Kubility;

public class N2View : MonoDelegateView {


	protected override void Awake ()
	{
		base.Awake ();

		BaseView.Create<BaseView>(this,new HideTrans(gameObject));
	}

	public void ButtonClick()
	{
		ContentManager.mIns.Push(NViewType.N3View);
	}



}
