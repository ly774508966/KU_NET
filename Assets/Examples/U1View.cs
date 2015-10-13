using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Kubility;
using UnityEngine.UI;

public class U1View : MonoDelegateView
{

	public GameObject button;
	public RawImage image;

	protected override void Awake ()
	{
		base.Awake ();
//		Vector3 pos = m_view.pos;
		BaseView.Create<UGUIView> (this, new HideTrans (gameObject));
//		m_view.pos = pos; 
		button.GetListener ().onPointerClick = ButtonClick;

//		m_view.Push_AutoUIBehaviour (image);

	}

	int val;
	public ScrollRect scroll;
	public Scrollbar bar;
	public Toggle toggle;
	public InputField inp;
	public Dropdown drop;

	public void ButtonClick (GameObject gobh, BaseEventData data)
	{

		ContentManager.mIns.Push (MainViewType.U2View);

	}
}
