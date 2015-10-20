using UnityEngine;
using System.Collections;
using Kubility;


public class SelectServerView : MonoDelegateView {

	protected override void Awake ()
	{
		base.Awake ();

		BaseView.Create<BaseView>(this,new HideTrans(gameObject));


	}

	protected override void Start ()
	{
		base.Start ();
	}
}
