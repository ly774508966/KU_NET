using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kubility;

public class U3View : MonoDelegateView
{
	public GameObject button;
	public ScrollRect scroll;
	
	protected override void Awake ()
	{
		base.Awake ();
		BaseView.Create<BaseView> (this,new HideTrans (gameObject));
		m_view.Push_AutoUIBehaviour (scroll);
		
		button.GetListener ().onPointerClick += ButtonClick;
	}
	
	public void ButtonClick (GameObject gobh, BaseEventData data)
	{
		
		ContentManager.mIns.Pop ();
	}
}

