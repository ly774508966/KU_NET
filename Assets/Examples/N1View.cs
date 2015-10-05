using UnityEngine;
using System.Collections;
using Kubility;
using UnityEngine.UI;

public class N1View : MonoDelegateView
{

	protected override void Awake ()
	{
		base.Awake ();
		using(var cc = new ClsTuple<int,float>())
		{
			cc.field0 =12;
			cc.field1 = 123f;
		}



		BaseView.Create<BaseView>(this,new HideTrans(gameObject));
	}

	public void ButtonClick()
	{
		ContentManager.mIns.Push(NViewType.N2View);
	}
}
