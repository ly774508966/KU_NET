using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Kubility;
using UnityEngine.UI;

public class U1View : MonoDelegateView
{

	public GameObject button;
	public RawImage image;
		
	public ScrollRect scroll;
	public Scrollbar bar;
	public Toggle toggle;
	public InputField inp;
	int val;

	protected override void Awake ()
	{
		base.Awake ();
//		Vector3 pos = m_view.pos;
		BaseView.Create<UGUIView> (this, new HideTrans (gameObject));
//		m_view.pos = pos; 
		button.GetListener ().onPointerClick = ButtonClick;

//		m_view.Push_AutoUIBehaviour (image);

	}




	public void ButtonClick (GameObject gobh, BaseEventData data)
	{

		ContentManager.mIns.Push (MainViewType.U2View);

	}
}
