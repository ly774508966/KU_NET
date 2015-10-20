using UnityEngine;
using System.Collections;
using UnityEditor;

public class MyEdit {


	[MenuItem("Assets/KUI/Set Android Etc1", false, 0)]
	static public void OpenAtlasMaker ()
	{
		var tars = Selection.objects;

		Texture2D tex= Selection.activeObject as Texture2D;

		string path = AssetDatabase.GetAssetPath(tex);
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		textureImporter.textureFormat = TextureImporterFormat.ETC_RGB4;
		textureImporter.textureType = TextureImporterType.Sprite;
		textureImporter.compressionQuality =1;

		EditorUserBuildSettings.androidBuildSubtarget =MobileTextureSubtarget.ETC;

	}
}
