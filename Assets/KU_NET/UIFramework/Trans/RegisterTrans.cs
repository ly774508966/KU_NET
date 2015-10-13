using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;


namespace Kubility
{
	/// <summary>
	/// Register trans.
	/// </summary>
	public class RegisterTrans:SingleTon<RegisterTrans>
	{
		public static Type[]  TransTypes ={typeof(AnimationTrans),typeof(HideTrans)};
		List<FieldInfo[]> fieldList = new List<FieldInfo[]>();
		
		
		string[] arr;
		
		public RegisterTrans()
		{
			
			foreach(var sub in TransTypes)
			{
				fieldList.Add(sub.GetFields(BindingFlags.Public | BindingFlags.Instance  | BindingFlags.NonPublic));
			}
		}
		
		
		public string[] ToStringArray(string other)
		{
			if(arr == null)
			{
				arr = new string[TransTypes.Length];
				for(int i=0; i< TransTypes.Length;++i)
				{
					arr[i] = other + TransTypes[i].Name;
				}
			}
			
			return arr;
		}
		
		public FieldInfo[] GetFields(int index =0)
		{
			if(index < fieldList.Count)
			{
				return fieldList[index];
			}
			else
			{
				LogMgr.LogError("index error");
				return null;
			}
			
		}
		
		//		public static AbstractTrans Create(ref BaseView view)
		//		{
		//
		//			if(view)
		//
		//			return null;
		//		}
		
	}
	
	
}


