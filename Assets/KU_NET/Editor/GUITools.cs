using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Kubility
{
	public static class GUITools
	{

		public static int CreateSearChTextField_GUI (Vector2 pos, int selected, string[] settings, ref string content)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);

			selected = EditorGUI.Popup (new Rect (pos.x, pos.y, 20, 20), "", selected, settings, "ToolbarSeachTextFieldPopup");

			content = GUI.TextField (new Rect (pos.x, pos.y, 102, 18), content, "toolbarSeachTextField");

			if (GUI.Button (new Rect (pos.x +102, pos.y, 20, 18), "", GUI.skin.FindStyle ("ToolbarSeachCancelButton"))) 
			{
				content = "";
			}
			GUILayout.EndHorizontal ();
			return selected;
		}

		public static void CreateSearChText (ref string content,float wid =100)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			GUI.SetNextControlName("Search");
			content = EditorGUILayout.TextField (content, GUI.skin.FindStyle ("ToolbarSeachTextField"), GUILayout.Width (wid));

			if (GUILayout.Button ("", GUI.skin.FindStyle ("ToolbarSeachCancelButton"), GUILayout.Width (20))) 
			{
				content = "";
				GUI.FocusControl(null);
			}
			
			GUILayout.EndHorizontal ();
		}
		
	}

	public static class RectExtensions
	{
		public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
	}

	public class EditorZoomArea
	{
		private static Matrix4x4 _prevGuiMatrix;
		private static Rect groupRect = default(Rect);
		public static Rect Begin(Rect screenCoordsArea, float zoomScale)
		{
			GUI.EndGroup();
			Rect rect = screenCoordsArea.ScaleSizeBy(1f / zoomScale, screenCoordsArea.TopLeft());
			rect.y += 21f;
			GUI.BeginGroup(rect);
			EditorZoomArea._prevGuiMatrix = GUI.matrix;
			Matrix4x4 lhs = Matrix4x4.TRS(rect.TopLeft(), Quaternion.identity, Vector3.one);
			Vector3 one = Vector3.one;
			one.y = zoomScale;
			one.x = zoomScale;
			Matrix4x4 rhs = Matrix4x4.Scale(one);
			GUI.matrix = lhs * rhs * lhs.inverse * GUI.matrix;
			return rect;
		}
		public static void End()
		{
			GUI.matrix = EditorZoomArea._prevGuiMatrix;
			GUI.EndGroup();
			EditorZoomArea.groupRect.y = 21f;
			EditorZoomArea.groupRect.width = (float)Screen.width;
			EditorZoomArea.groupRect.height = (float)Screen.height;
			GUI.BeginGroup(EditorZoomArea.groupRect);
		}
	}

}


