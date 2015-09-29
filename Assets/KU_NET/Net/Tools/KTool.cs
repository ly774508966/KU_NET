using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Kubility
{
	public static class KTool 
	{
		public delegate void VoidDelegate();
		
		public static Stopwatch StartTiming()
		{
			Stopwatch watch = new Stopwatch();
			watch.Reset();
			watch.Start();
			return watch;
		}
		
		public static void LogTime(Stopwatch watch,string headstr=null)
		{
			watch.Stop();
			if(headstr == null)
				LogMgr.Log("Cost Time =  " + watch.ElapsedMilliseconds.ToString() + " ms");
			else
				LogMgr.Log(headstr + watch.ElapsedMilliseconds .ToString()+ " ms");
		}
		
		public static void  DumpTime (VoidDelegate dv)
		{
			Stopwatch watch = new Stopwatch();
			watch.Reset();
			watch.Start();
			dv();
			watch.Stop();
			LogMgr.Log("Cost Time =  " + watch.ElapsedMilliseconds.ToString() +" ms");
			LogMgr.Log("Cost Time Span =  "  + watch.Elapsed.ToString() );
		}
		
		public static void Dump(object obj)
		{
			Type type =obj.GetType();
			FieldInfo[] fields = type.GetFields( BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Public | BindingFlags.Static);
			PropertyInfo[] propertys = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Public | BindingFlags.Static);
			
			for(int i=0; i < fields.Length; ++i)
			{
				FieldInfo info = fields[i];
				LogMgr.Log("FieldInfo Name =" + info.Name+" Type = + " + info.FieldType.ToString()  + " Value ="+ info.GetValue(obj) );
			}
			
			for(int i=0; i < propertys.Length; ++i)
			{
				PropertyInfo info = propertys[i];
				LogMgr.Log("PropertyInfo Name =" + info.Name+" Type = + " + info.PropertyType.ToString()  + " Value ="+ info.GetValue(obj,null) );
			}
			
		}

		public static string DumpAsString(object obj)
		{
			StringBuilder builder = new StringBuilder();
			Type type =obj.GetType();
			FieldInfo[] fields = type.GetFields( BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Public | BindingFlags.Static);
			PropertyInfo[] propertys = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Public | BindingFlags.Static);
			
			for(int i=0; i < fields.Length; ++i)
			{
				FieldInfo info = fields[i];
				builder.Append("FieldInfo Name =" + info.Name+" Type = + " + info.FieldType.ToString()  + " Value ="+ info.GetValue(obj) +"\n");
			}
			
			for(int i=0; i < propertys.Length; ++i)
			{
				PropertyInfo info = propertys[i];
				builder.Append("PropertyInfo Name =" + info.Name+" Type = + " + info.PropertyType.ToString()  + " Value ="+ info.GetValue(obj,null)+"\n" );
			}

			return builder.ToString();
			
		}
		
		public static void Dump(object obj,Type type,FieldInfo[] fields ,PropertyInfo[] propertys)
		{
			for(int i=0; i < fields.Length; ++i)
			{
				FieldInfo info = fields[i];
				LogMgr.Log("FieldInfo Name =" + info.Name+" Type = + " + info.FieldType.ToString()  + " Value ="+ info.GetValue(obj) );
			}
			
			for(int i=0; i < propertys.Length; ++i)
			{
				PropertyInfo info = propertys[i];
				LogMgr.Log("PropertyInfo Name =" + info.Name+" Type = + " + info.PropertyType.ToString()  + " Value ="+ info.GetValue(obj,null) );
			}
			LogMgr.Log("-----------------");
			
		}
		
		
	}
}


