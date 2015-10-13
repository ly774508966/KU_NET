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


    bool foldout;

    //SingleArray<bool> barray = new SingleArray<bool>(RegisterTrans.TransTypes.Length,false);

    int selected = 0;
    FieldInfo[] fields = null;

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


