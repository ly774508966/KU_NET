using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GlobalHelper))]
public class GlobalHelperUtils : Editor
{
    public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
    public int oldsize = 0;

    public override void OnInspectorGUI()
    {

        GlobalHelper global = (GlobalHelper)target;

        EditorGUILayout.ObjectField("脚本", global, typeof(GlobalHelper), true);

        //        global.gCamera = (GameObject)EditorGUILayout.ObjectField("公用Object ", global.gCamera, typeof(GameObject), true);
        //EditorGUIUtility.LookLikeControls(120);
        GUILayout.Space(10);

        EditorGUILayout.EnumPopup("上个场景", global.lastGameScene);
        EditorGUILayout.EnumPopup("当前场景", global.curGameScene);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("列表大小", GUILayout.Width(100));

        global.listsize = EditorGUILayout.IntField(global.listsize, GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        //if(!Application.isPlaying)
        //{
        for (int i = 0; i != global.listsize; ++i)
        {
            Iobject iobj = null;
            if (global.listsize > global._InitObjList.Count)
            {
                iobj = new Iobject();
                global._InitObjList.Add(iobj);
            }
            else
            {
                iobj = global._InitObjList[i];
            }

            iobj.obj = (GameObject)EditorGUILayout.ObjectField("游戏对象" + i, iobj.obj, typeof(GameObject), true);
            iobj.parent = (Transform)EditorGUILayout.ObjectField("父节点(null则自动挂接)", iobj.parent, typeof(Transform), true);
            iobj.InitScene = (GameScene)EditorGUILayout.EnumPopup("初始化场景", iobj.InitScene);
            iobj.Immidate = EditorGUILayout.Toggle("是否立刻添加", iobj.Immidate);
            GUILayout.Space(10);

            if (global.listsize < oldsize)
            {
                for (int j = 0; j != oldsize - global.listsize; ++j)
                {
                    global._InitObjList.RemoveAt(global._InitObjList.Count - 1);
                }
            }


            oldsize = global.listsize;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}