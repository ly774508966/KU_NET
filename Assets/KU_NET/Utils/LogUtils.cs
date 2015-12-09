using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Kubility
{
	public enum LogType
	{
		INFO,
		ERROR,
		WARN,
	}

	public static class LogUtils 
	{
		public static void LogFullInfo(string varname,MonoBehaviour mono ,object exter = null)
		{

			DateTime currentTime = DateTime.Now;
			if(exter == null)
				LogMgr.Log( currentTime.ToString() +" || varname:" +varname +" || " + mono +" Enble:"+mono.enabled.ToString() +" || active :"+ mono.gameObject.activeSelf.ToString());
			else
				LogMgr.Log( currentTime.ToString() +" || varname:" +varname +" || " + mono +" Enble:"+mono.enabled.ToString() +" || active :"+ mono.gameObject.activeSelf.ToString() +" || "+ exter.ToString());

		}

		public static void LogFullInfo(string varname,LogType type = LogType.INFO)
		{
			DateTime currentTime = DateTime.Now;
#if UNITY_EDITOR

			LogMgr.Log(type.ToString()+" >> "+ varname +" || Time >>" +currentTime.ToString());
#else
			if(type == LogType.INFO)
			{
				LogMgr.Log("Info >> "+ varname +" || Time >>" +currentTime.ToString());
			}
			else if(type == LogType.WARN)
			{
				LogMgr.Log("Warnning >> "+ varname +" || Time >>" +currentTime.ToString());
			}
			else
			{
				LogMgr.Log("Error >> "+ varname +" || Time >>" +currentTime.ToString());
			}

#endif
		}

	}
}


