using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Kubility
{
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

	}
}


