using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kubility
{
	public static class StaticUtils
	{
		public static GameObject AddChild (this GameObject parent, GameObject prefab)
		{
			GameObject go = GameObject.Instantiate (prefab) as GameObject;
			if (go != null && parent != null) {
				Transform t = go.transform;
				t.parent = parent.transform;
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				go.layer = parent.layer;
			}
			return go;
		}

		public static GameObject AddChildImitate (this GameObject parent, GameObject gobj)
		{

			if (gobj != null && parent != null) {
				Transform t = gobj.transform;
				t.parent = parent.transform;
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				gobj.layer = parent.layer;
			}
			return gobj;
		}

		public static  UILisenter GetListener (this GameObject go)
		{
			//gc  may it will be better (interface)
			UILisenter listener = go.GetComponent<UILisenter> ();
			if (listener == null)
				listener = go.AddComponent<UILisenter> ();
			return listener;
		}

#region AutoAlignWithRectTrans

#if KUGUI

		public static void AutoAlignWithRectTrans (this Text text, UIAlign align, float z = 0f)
		{
			
			LogMgr.LogError (align.ToString ());
			if (align == UIAlign.CENTER) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				text.alignment = TextAnchor.MiddleCenter;
				text.rectTransform.localPosition = new Vector3 (pos.x, pos.y, z);
				
			} else if (align == UIAlign.LEFT_DOWN) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				text.alignment = TextAnchor.LowerLeft;
				float wid = text.rectTransform.sizeDelta.x;
				float height = text.rectTransform.sizeDelta.y;
				
				Vector3 fpos = new Vector3 (pos.x + wid / 2, pos.y + height / 2, z);
				text.rectTransform.localPosition = fpos;
			} else if (align == UIAlign.LEFT_TOP) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				text.alignment = TextAnchor.LowerLeft;
				float wid = text.rectTransform.sizeDelta.x;
				float height = text.rectTransform.sizeDelta.y;
				
				Vector3 fpos = new Vector3 (pos.x + wid / 2, pos.y - height / 2, z);
				text.rectTransform.localPosition = fpos;
			} else if (align == UIAlign.RIGHT_TOP) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				text.alignment = TextAnchor.LowerRight;
				float wid = text.rectTransform.sizeDelta.x;
				float height = text.rectTransform.sizeDelta.y;
				
				Vector3 fpos = new Vector3 (pos.x - wid / 2, pos.y - height / 2, z);
				text.rectTransform.localPosition = fpos;
			} else if (align == UIAlign.RIGHT_DOWN) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				text.alignment = TextAnchor.LowerRight;
				float wid = text.rectTransform.sizeDelta.x;
				float height = text.rectTransform.sizeDelta.y;
				
				Vector3 fpos = new Vector3 (pos.x - wid / 2, pos.y + height / 2, z);
				text.rectTransform.localPosition = fpos;
			}
		}
		
		public static void AutoAlign (this GameObject target, Vector2 contentSize, UIAlign align = UIAlign.CENTER, float z = 0f)
		{
			Canvas canvas = UIManager.current.CurrentCanvas;
			RectTransform rectTransform = UIManager.current.transform as RectTransform;
			Camera camera =	UIManager.current.camera;
			float height = canvas.pixelRect.height;
			float wid = canvas.pixelRect.width;
			float scaleX = rectTransform.localScale.x;//scaleFactor
			float scaleY = rectTransform.localScale.y;//scaleFactor
			
			bool isOverLay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
			
			if (align == UIAlign.CENTER) {
				Vector3 wpos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle (rectTransform, new Vector3 (wid / 2, height / 2, z), isOverLay ? null : camera, out wpos);
				
				target.transform.position = wpos;
			} else if (align == UIAlign.LEFT_DOWN) {
				Vector3 wpos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle (rectTransform, new Vector3 (0, 0, z), isOverLay ? null : camera, out wpos);
				Vector3 nwpos = new Vector3 (wpos.x + contentSize.x * scaleX / 2, wpos.y + contentSize.y * scaleY / 2, wpos.z);
				target.transform.position = nwpos;
				
			} else if (align == UIAlign.LEFT_TOP) {
				Vector3 wpos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle (rectTransform, new Vector3 (0, height, z), isOverLay ? null : camera, out wpos);
				Vector3 nwpos = new Vector3 (wpos.x + contentSize.x * scaleX / 2, wpos.y - contentSize.y * scaleY / 2, wpos.z);
				target.transform.position = nwpos;
				
			} else if (align == UIAlign.RIGHT_TOP) {
				Vector3 wpos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle (rectTransform, new Vector3 (wid, height, z), isOverLay ? null : camera, out wpos);
				Vector3 nwpos = new Vector3 (wpos.x - contentSize.x * scaleX / 2, wpos.y - contentSize.y * scaleY / 2, wpos.z);
				target.transform.position = nwpos;
				
			} else if (align == UIAlign.RIGHT_DOWN) {
				Vector3 wpos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle (rectTransform, new Vector3 (wid, 0, z), isOverLay ? null : camera, out wpos);
				Vector3 nwpos = new Vector3 (wpos.x - contentSize.x * scaleX / 2, wpos.y + contentSize.y * scaleY / 2, wpos.z);
				target.transform.position = nwpos;
				
			}
		}
		
		public static void AutoAlignWithRectTrans<T> (this T target, UIAlign align, float z = 0f) where T:UIBehaviour
		{
			
			RectTransform rectTransform = (RectTransform)target.transform;
			//it may not 1,1,1
			rectTransform.localScale = Vector3.one;
			if (align == UIAlign.CENTER) {
				target.transform.localPosition = UIManager.mIns.GetUIAlignPos (align);
			} else if (align == UIAlign.LEFT_DOWN) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				float wid = rectTransform.sizeDelta.x;
				float height = rectTransform.sizeDelta.y;
				target.transform.localPosition = new Vector3 (pos.x + wid / 2, pos.y + height / 2, z);
				
			} else if (align == UIAlign.LEFT_TOP) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				float wid = rectTransform.sizeDelta.x;
				float height = rectTransform.sizeDelta.y;
				target.transform.localPosition = new Vector3 (pos.x + wid / 2, pos.y - height / 2, z);
				
			} else if (align == UIAlign.RIGHT_TOP) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				float wid = rectTransform.sizeDelta.x;
				float height = rectTransform.sizeDelta.y;
				target.transform.localPosition = new Vector3 (pos.x - wid / 2, pos.y - height / 2, z);
				
			} else if (align == UIAlign.RIGHT_DOWN) {
				Vector2 pos = UIManager.mIns.GetUIAlignPos (align);
				float wid = rectTransform.sizeDelta.x;
				float height = rectTransform.sizeDelta.y;
				target.transform.localPosition = new Vector3 (pos.x - wid / 2, pos.y + height / 2, z);
				
			}
		}
				

#endif


#endregion

		
	}
}


