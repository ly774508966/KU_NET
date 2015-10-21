using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TextureEdit {


	[MenuItem("Assets/KUI/Set Android Etc1", false, 0)]
	static public void SetAndroidEtc1 ()
	{

		SetEtc1(Selection.activeObject);

	}

	static void SetEtc1(UnityEngine.Object curObject)
	{
		Texture2D tex= curObject as Texture2D;
		if(tex != null)
		{
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.compressionQuality =1;
			textureImporter.spriteImportMode =  SpriteImportMode.Single;
			textureImporter.mipmapEnabled = false;
			
			textureImporter.SetPlatformTextureSettings("Android",2048, TextureImporterFormat.ETC_RGB4,true);
			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();
			TextureImporterSettings tis = new TextureImporterSettings();
			tis.allowsAlphaSplit =true;
			tis.spriteMode = 1;
			tis.mipmapEnabled =false;
			tis.textureFormat = TextureImporterFormat.ETC_RGB4;
			
			textureImporter.ReadTextureSettings(tis);
			textureImporter.SetTextureSettings(tis);
			textureImporter.SetAllowsAlphaSplitting(true);
			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();
		}
		else
		{
			LogMgr.LogError("Type Error  Not Texture");
		}
		

	}
}
