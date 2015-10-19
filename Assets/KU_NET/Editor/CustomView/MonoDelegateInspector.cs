using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using Kubility;
using System.Reflection;

[CustomEditor(typeof(MonoDelegateView), true)]
[CanEditMultipleObjects]
public class MonoDelegateInspector : Editor
{

    int selected = 0;
    FieldInfo[] fields = null;
	readonly string defaultPath = "/Resources/Prefab/";
	string prefabPath ="";
	ReplacePrefabOptions ReplaceOptions = ReplacePrefabOptions.ConnectToPrefab;
	UnityEngine.Object prefab;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        MonoDelegateView mview = (MonoDelegateView)this.target;
        BaseView bview = mview.m_view;

        EditorGUILayout.PrefixLabel(new GUIContent("View Public Field", "use fewer Public fields will Be fine "));
        if (fields == null)
        {
            fields = bview.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        for (int i = 0; i < fields.Length; ++i)
        {
            KbEditorUtils.UnkownTypeEditorShow(fields[i], bview);
        }

        PrefabType prefabtype = PrefabUtility.GetPrefabType(mview);
        //		LogMgr.LogError (prefabtype.ToString());
        if (prefabtype == PrefabType.PrefabInstance || prefabtype == PrefabType.None)
        {
            bview.AutoPos = EditorGUILayout.Toggle("Auto Pos", bview.AutoPos);
            if (bview.AutoPos)
            {
                bview.pos = EditorGUILayout.Vector3Field("World Pos", mview.gameObject.transform.position);
            }

        }

        GUILayout.Space(10);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Create Trans  Target ");
        GUILayout.Space(10);
        selected = GUILayout.Toolbar(selected, RegisterTrans.mIns.ToStringArray(""));


        if (bview.Trans == null)
        {

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUILayout.Label("Author: Kubility ", GUILayout.Width(120));


            if (GUILayout.Button(new GUIContent("+", "it Will Create an Trans"), EditorStyles.toolbarButton))
            {
                Type transType = RegisterTrans.TransTypes[selected];
                var trans = Activator.CreateInstance(transType, new System.Object[] { null });//ScriptableObject.CreateInstance (transType);
                var field = bview.GetType().GetField("_Trans", BindingFlags.Instance | BindingFlags.NonPublic);

                field.SetValue(bview, trans);

            }

            GUILayout.EndHorizontal();
        }

        if (bview.Trans != null)
        {
            Type transType = RegisterTrans.TransTypes[selected];
            GUILayout.Space(10);
            DyNamicCreate(transType, bview);
        }
		string filepath=string.IsNullOrEmpty(prefabPath)? defaultPath +mview.gameObject.name:prefabPath;
		EditorGUILayout.Space();

		filepath =EditorGUILayout.TextField("PrefabPath",filepath);

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Convert To Prefab "))
		{
			prefab = PrefabUtility.CreateEmptyPrefab("Assets/"+filepath+".prefab") ;
			AssetDatabase.Refresh();
			int retryTimes =0;
			
			while( AssetDatabase.LoadAssetAtPath<GameObject>("Assets/"+filepath+".prefab") == null && retryTimes <100)
			{
				retryTimes++;
			}
			PrefabUtility.ReplacePrefab(mview.gameObject, prefab,ReplaceOptions);


		}

		ReplaceOptions = (ReplacePrefabOptions)EditorGUILayout.EnumPopup(ReplaceOptions);
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    void DyNamicCreate(Type transType, BaseView view)
    {

        if (view.Trans != null && view.Trans.GetType() == transType)
        {
            EditorGUILayout.BeginHorizontal();
            var tfields = RegisterTrans.mIns.GetFields(selected);
            foreach (var f in tfields)
            {
                KbEditorUtils.UnkownTypeEditorShow(f, view.Trans);
            }

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("-", "it remove an Trans"), EditorStyles.toolbarButton))
            {

                var field = view.GetType().GetField("_Trans", BindingFlags.Instance | BindingFlags.NonPublic);

                field.SetValue(view, null);

            }
            EditorGUILayout.EndHorizontal();

        }
        else if (view.Trans.GetType() != transType)
        {
            EditorGUILayout.HelpBox("Target Type  is " + view.Trans.GetType().Name, MessageType.Warning);
        }

    }



}


