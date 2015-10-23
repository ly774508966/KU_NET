#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditor;

namespace Kubility
{
    /// <summary>
    /// Register trans.
    /// </summary>
    public class RegisterTrans : SingleTon<RegisterTrans>
    {
        public static Type[] TransTypes = { typeof(AnimationTrans), typeof(HideTrans) };
        List<FieldInfo[]> fieldList = new List<FieldInfo[]>();


        string[] arr;

        public RegisterTrans()
        {

            foreach (var sub in TransTypes)
            {
                fieldList.Add(sub.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
            }
        }


        public string[] ToStringArray(string other)
        {
            if (arr == null)
            {
                arr = new string[TransTypes.Length];
                for (int i = 0; i < TransTypes.Length; ++i)
                {
                    arr[i] = other + TransTypes[i].Name;
                }
            }

            return arr;
        }

        public FieldInfo[] GetFields(int index = 0)
        {
            if (index < fieldList.Count)
            {
                return fieldList[index];
            }
            else
            {
                LogMgr.LogError("index error");
                return null;
            }

        }

		public class RegisterView : SingleTon<RegisterView>
		{
			Dictionary<SerializedObject,List<SerializedProperty>> dic  = new Dictionary<SerializedObject,List<SerializedProperty>>();

			Dictionary<Type,string[]> strDic = new Dictionary<Type,string[]>();

			public RegisterView()
			{
				strDic.Add(typeof(BaseView),new string[]
				{
					"isRunning",
					"AutoPos",
					"Pos",
					"Trans"
				}
				);

				strDic.Add(typeof(UGUIView),new string[]
				{
					"isRunning",
					"AutoPos",
					"Pos",
					"Trans",
					"height",
					"width",
					"ani"
				}
				);
			}

			public SerializedProperty[] GetSerializedProperty(SerializedObject sobj, Type type)
			{
				if (dic.ContainsKey(sobj))
				{
					return dic[sobj].ToArray();
				}
				else
				{
					if(strDic.ContainsKey(type))
					{
						string[] strs = strDic[type];
						List<SerializedProperty> list = new List<SerializedProperty>();
						foreach(var sub in strs)
						{
							list.Add(sobj.FindProperty(sub));
						}

						dic.Add(sobj,list);
						return list.ToArray();
					}
					else
					{
						LogMgr.LogError("Cant fin in RegisterView");

						return null;
					}

				}
			}

		}


    }


}

#endif

