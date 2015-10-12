using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Kubility;
using UnityEngine.UI;

public class U2View : MonoDelegateView,IDragHandler
{

	public GameObject button;
	public GameObject button2;
	public Image mbt;
	int val = 0;

	public void OnDrag (PointerEventData eventData)
	{
		Vector3 wpos;

		RectTransformUtility.ScreenPointToWorldPointInRectangle (UIManager.current.transform as RectTransform, eventData.position, null, out wpos);

		gameObject.transform.position = wpos;
	}

	protected override void Awake ()
	{

		base.Awake ();

		BaseView.Create<BaseView> (this,new HideTrans (gameObject));

		button.GetListener ().onPointerClick += ButtonClick;
		button2.GetListener ().onPointerClick += ButtonClick2;
	}

	public void ButtonClick (GameObject gobh, BaseEventData data)
	{
				

		ContentManager.mIns.Push (MainViewType.U3View);
		
	}

	public void ButtonClick2 (GameObject gobh, BaseEventData data)
	{
#if KUGUI
		Vector3 size = new Vector3 (mbt.rectTransform.sizeDelta.x, mbt.rectTransform.sizeDelta.y + 100, 0);
		gameObject.AutoAlign (size, (UIAlign)((val++) % 5));
#elif KNGUI


#endif

	}
}
