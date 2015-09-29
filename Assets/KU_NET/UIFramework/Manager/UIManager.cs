using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kubility
{
	public enum UIAlign
	{
		LEFT_TOP,
		LEFT_DOWN,
		RIGHT_TOP,
		RIGHT_DOWN,
		CENTER,
	}



	public class UIManager :SingleTon<UIManager>
	{
		Dictionary<UIType,ClsTuple<BaseView,GameObject>> UiDict = new Dictionary<UIType, ClsTuple<BaseView,GameObject>> ();

		public static KUIROOT current;

		public sealed class KUIROOT
		{
			public GameObject gameobject;

#if KUGUI
			Canvas canvas;

			public Canvas CurrentCanvas
			{
				get
				{
					return canvas;
				}
			}
#endif
			public Transform transform
			{
				get
				{
#if KUGUI
					return canvas.transform;
#elif KNGUI

					return gameobject.transform;
#endif
				}
			}

			Camera m_camera;
			public Camera camera
			{
				get
				{
					return m_camera;
				}
			}



#if KUGUI
			public KUIROOT(Canvas can)
			{
				canvas = can;
				gameobject = can.gameObject;
				m_camera = canvas.worldCamera;
			}
#elif KNGUI
			public KUIROOT(GameObject gobj,Camera cam)
			{
				gameobject = gobj;
				m_camera = cam;
			}

			
#endif
			

		}

		public UIManager ()
		{

#if KUGUI
			var canvas = GameObject.FindObjectOfType<Canvas> ();
			current = new KUIROOT(canvas);
#elif KNGUI
			var NGUIroot = GameObject.FindGameObjectWithTag("NGUI");
			var NGUICamera =UICamera.mainCamera;
			current = new KUIROOT(NGUIroot,NGUICamera);
#endif

		}

#if KUGUI
		public Vector2 GetUIAlignPos (UIAlign align, Camera camera)
		{
			Canvas canvas = current.CurrentCanvas;
			bool isOverLay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;

			float ScreenHight = canvas.pixelRect.height;
			float ScreenWidth = canvas.pixelRect.width;
			
			Vector2 localPos;
			
			switch (align) {
			case UIAlign.CENTER:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth / 2, ScreenHight / 2), isOverLay ? null : camera, out localPos);
				
					break;
				}
			case UIAlign.LEFT_DOWN:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (0, 0), isOverLay ? null : camera, out localPos);
					break;
				}
			case UIAlign.LEFT_TOP:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (0, ScreenHight), isOverLay ? null : camera, out localPos);
					break;
				}
			case UIAlign.RIGHT_DOWN:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth, 0), isOverLay ? null : camera, out localPos);
					break;
				}
			case UIAlign.RIGHT_TOP:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth, ScreenHight), isOverLay ? null : camera, out localPos);
					break;
				}
			default:
				RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth / 2, ScreenHight / 2), isOverLay ? null : camera, out localPos);
				break;
			}

			return localPos;
			
		}

		public Vector2 GetUIAlignPos (UIAlign align)
		{
			Canvas canvas = current.CurrentCanvas;
			bool isOverLay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;

			float ScreenHight = canvas.pixelRect.height;
			float ScreenWidth = canvas.pixelRect.width;

			Vector2 localPos;

			switch (align) {
			case UIAlign.CENTER:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth / 2, ScreenHight / 2), isOverLay ? null : canvas.worldCamera, out localPos);
				
					break;
				}
			case UIAlign.LEFT_DOWN:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (0, 0), isOverLay ? null : canvas.worldCamera, out localPos);
					break;
				}
			case UIAlign.LEFT_TOP:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (0, ScreenHight), isOverLay ? null : canvas.worldCamera, out localPos);
					break;
				}
			case UIAlign.RIGHT_DOWN:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth, 0), isOverLay ? null : canvas.worldCamera, out localPos);
					break;
				}
			case UIAlign.RIGHT_TOP:
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth, ScreenHight), isOverLay ? null : canvas.worldCamera, out localPos);
					break;
				}
			default:
				RectTransformUtility.ScreenPointToLocalPointInRectangle (canvas.transform as RectTransform, new Vector2 (ScreenWidth / 2, ScreenHight / 2), isOverLay ? null : canvas.worldCamera, out localPos);
				break;
			}

			return localPos;

		}
#endif

		public BaseView TryGet (UIType Type, string path, string[] subs = null, GameObject Parent = null)
		{

			if (!UiDict.ContainsKey (Type)) {
				GameObject obj;
				GameObject prefab = Resources.Load<GameObject> (path);
				if (Parent != null) {
					obj = Parent.AddChild (prefab);
				} 
				else {

					obj = current.gameobject.AddChild (prefab);

				}
				BaseView view = obj.GetComponent< MonoDelegateView> ().m_view;

				UiDict.Add (Type, new ClsTuple<BaseView, GameObject> (view, obj));
				if (subs != null && obj != null) {
					new Task (AddSubPrefabs (subs, obj), true);
				}


			}

					
			return UiDict [Type].field0;
		}

		public bool TryRemove (UIType Type, bool DestroyGameObject = true)
		{
			if (!UiDict.ContainsKey (Type)) {
				return false;
			}


			ClsTuple<BaseView,GameObject> value = UiDict [Type];

			if (value.field1 == null) {
				return UiDict.Remove (Type);

			}

			if (DestroyGameObject) {
				GameObject.Destroy (value.field1);
			}

			return UiDict.Remove (Type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The sub prefabs.</returns>
		/// <param name="paths">Paths.</param>
		/// <param name="parent">Parent.</param>
		IEnumerator AddSubPrefabs (string[] paths, GameObject parent)
		{
			for (int i = 0; i < paths.Length; ++i) {
				string path = paths [i];
				yield return new WaitForFixedUpdate ();

				parent.AddChild (Resources.Load<GameObject> (path));

			}
		}


	}
}


