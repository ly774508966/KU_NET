using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Kubility.Editor
{
	public static class KbEditorUtils
	{
		public static void ListToolbar<T>(List<T> list, ref Vector2 curpos)
		{
			
			curpos = EditorGUILayout.BeginScrollView(curpos);
			for (int i = 0; i < list.Count; ++i)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Toggle(true, "  " + i.ToString(), GUILayout.Width(70));
				
				GUILayout.Space(20);
				
				GUILayout.Label(list[i].ToString(), GUILayout.Width(150));
				
				GUILayout.EndHorizontal();
			}
			
			EditorGUILayout.EndScrollView();
		}
		
		public static void AddTag(string tagname)
		{
			bool ret = false;
			for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
			{
				if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tagname))
				{
					ret = true;
					break;
				}
			}
			
			
			if (!ret)
			{
				
				SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				SerializedProperty it = tagManager.GetIterator();
				
				while (it.NextVisible(true))
				{
					
					if (it.name == "tags")
					{
						
						int size = it.arraySize;
						it.InsertArrayElementAtIndex(size);
						SerializedProperty dataPoint = it.GetArrayElementAtIndex(size);
						dataPoint.stringValue = tagname;
						tagManager.ApplyModifiedProperties();
					}
				}
			}
		}
		
		public static void UnkownTypeEditorShow(ref object target)
		{
			try
			{
				if (target == null)
					return;
				
				Type type = target.GetType();
				
				if (type == typeof(int))
				{
					target = EditorGUILayout.IntField(type.Name, (int)target);
					
				}
				else if (type == typeof(bool))
				{
					
					target = EditorGUILayout.Toggle(type.Name, (bool)target);
					
				}
				else if (type == typeof(float))
				{
					
					target = EditorGUILayout.FloatField(type.Name, (float)target);
					
				}
				else if (type == typeof(double))
				{
					
					target = EditorGUILayout.DoubleField(type.Name, (double)target);
					
				}
				else if (type == typeof(string))
				{
					
					
					target = EditorGUILayout.TextField(type.Name, (string)target);
					
				}
				else if (type == typeof(short))
				{
					
					target = (short)EditorGUILayout.IntField(type.Name, (short)target);
					
				}
				else if (type == typeof(GameObject))
				{
					
					target = (GameObject)EditorGUILayout.ObjectField(type.Name, (GameObject)target, typeof(GameObject), true);
					
				}
				else if (type == typeof(Transform))
				{
					
					target = (Transform)EditorGUILayout.ObjectField(type.Name, (Transform)target, typeof(Transform), true);
					
				}
				else if (type == typeof(Component))
				{
					
					target = (Component)EditorGUILayout.ObjectField(type.Name, (Component)target, typeof(Component), true);
					
				}
				else if (type == typeof(Animator))
				{
					
					target = (Animator)EditorGUILayout.ObjectField(type.Name, (Animator)target, typeof(Animator), true);
					
				}
				else if (type == typeof(MonoBehaviour))
				{
					
					target = (MonoBehaviour)EditorGUILayout.ObjectField(type.Name, (MonoBehaviour)target, typeof(MonoBehaviour), true);
					
				}
				else if (type == typeof(UILisenter))
				{
					target = (UILisenter)EditorGUILayout.ObjectField(type.Name, (UILisenter)target, typeof(UILisenter), true);
				}
			}
			catch (Exception ex)
			{
				LogMgr.LogError("Error   " + ex);
			}
			
		}
		
		public static void UnkownTypeEditorShow(FieldInfo field, object target)
		{
			try
			{
				if (target == null)
					return;
				
				if (field.FieldType == typeof(int))
				{
					
					int value = (int)field.GetValue(target);
					value = EditorGUILayout.IntField(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(bool))
				{
					bool value = (bool)field.GetValue(target);
					value = EditorGUILayout.Toggle(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(float))
				{
					float value = (float)field.GetValue(target);
					value = EditorGUILayout.FloatField(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(double))
				{
					double value = (double)field.GetValue(target);
					value = EditorGUILayout.DoubleField(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(string))
				{
					string value = (string)field.GetValue(target);
					
					value = EditorGUILayout.TextField(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(short))
				{
					short value = (short)field.GetValue(target);
					value = (short)EditorGUILayout.IntField(field.Name, value);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(GameObject))
				{
					GameObject value = (GameObject)field.GetValue(target);
					value = (GameObject)EditorGUILayout.ObjectField(field.Name, value, typeof(GameObject), true);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(Transform))
				{
					Transform value = (Transform)field.GetValue(target);
					value = (Transform)EditorGUILayout.ObjectField(field.Name, value, typeof(Transform), true);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(Component))
				{
					Component value = (Component)field.GetValue(target);
					value = (Component)EditorGUILayout.ObjectField(field.Name, value, typeof(Component), true);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(Animator))
				{
					Animator value = (Animator)field.GetValue(target);
					value = (Animator)EditorGUILayout.ObjectField(field.Name, value, typeof(Animator), true);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(MonoBehaviour))
				{
					MonoBehaviour value = (MonoBehaviour)field.GetValue(target);
					value = (MonoBehaviour)EditorGUILayout.ObjectField(field.Name, value, typeof(MonoBehaviour), true);
					field.SetValue(target, value);
				}
				else if (field.FieldType == typeof(UILisenter))
				{
					UILisenter value = (UILisenter)field.GetValue(target);
					value = (UILisenter)EditorGUILayout.ObjectField(field.Name, value, typeof(UILisenter), true);
					field.SetValue(target, value);
				}
				
			}
			catch (Exception ex)
			{
				LogMgr.LogError("Error   " + ex);
			}
			
		}
	}
}


