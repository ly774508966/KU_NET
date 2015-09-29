using UnityEngine;
using System.Collections;
using UnityEditor;

public class PackageImporter : AssetPostprocessor {
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets,string[] movedAssets, string[] movedFromAssetPaths) 
	{

		if(importedAssets.Length >0)
		{
			KbEditorUtils.AddTag("Kubility");
		}
	}
}
