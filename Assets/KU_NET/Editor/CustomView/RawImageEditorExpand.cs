using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;

namespace Kubility
{
	[CustomEditor(typeof(RawImageExpand), true)]
	[CanEditMultipleObjects]
	public class RawImageEditorExpand : RawImageEditor
	{
		
		float showSize = 1f;
		float zoomScale = 1f;
		public override void OnInspectorGUI ()
		{
			
			base.OnInspectorGUI ();
			EditorGUILayout.BeginVertical();
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Show size ");
			zoomScale = GUILayout.HorizontalSlider(zoomScale,0,10);
			EditorGUILayout.EndHorizontal();
			
			
			if (EditorGUILayout.BeginFadeGroup (showSize)) {
				EditorGUILayout.BeginHorizontal ();
				{
					
					GUILayout.Space (EditorGUIUtility.labelWidth);
					if (GUILayout.Button (new GUIContent ("Geometric zoom ", "Geometric zoom  .only wid & height "), EditorStyles.miniButton)) {
						for (int i=0; i < targets.Length; ++i) {
							Graphic graphic = targets [i] as Graphic;
							if (graphic != null) {
								Undo.RecordObject (graphic.rectTransform, "Geometric zoo");
								graphic.SetNativeSize ();
								Vector2 oldsize = graphic.rectTransform.sizeDelta;
								
								graphic.rectTransform.sizeDelta = new Vector2 (oldsize.x * zoomScale, oldsize.y * zoomScale);
								EditorUtility.SetDirty (graphic);
							}
						}
						
					}
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndFadeGroup ();
			EditorGUILayout.EndVertical();
		}
		
		public override bool RequiresConstantRepaint ()
		{
			return base.RequiresConstantRepaint ();
		}
		
		public override bool HasPreviewGUI ()
		{
			return base.HasPreviewGUI ();
		}
		
		public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height)
		{
			return base.RenderStaticPreview (assetPath, subAssets, width, height);
		}
		
		public override void OnPreviewGUI (Rect r, GUIStyle background)
		{
			base.OnPreviewGUI (r, background);
		}
		
		public override void OnInteractivePreviewGUI (Rect r, GUIStyle background)
		{
			base.OnInteractivePreviewGUI (r, background);
		}
		
		public override void OnPreviewSettings ()
		{
			base.OnPreviewSettings ();
		}
		
		public override string GetInfoString ()
		{
			return base.GetInfoString();
			
		}
		
		public override void ReloadPreviewInstances ()
		{
			base.ReloadPreviewInstances ();
		}
		
		protected override void OnHeaderGUI ()
		{
			base.OnHeaderGUI ();
		}
		
		public override void DrawPreview (Rect previewArea)
		{
			base.DrawPreview (previewArea);
		}
		
		public override bool UseDefaultMargins ()
		{
			return base.UseDefaultMargins ();
		}
		
		protected override void OnDisable ()
		{
			base.OnDisable ();
		}
		
		protected override void OnEnable ()
		{
			base.OnEnable ();
		}
	}
}


