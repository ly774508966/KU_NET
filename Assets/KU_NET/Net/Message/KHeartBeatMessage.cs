using UnityEngine;
using System.Collections;
using Kubility;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;


//public class KHeartBeatMessage : BaseMessage
//{
//
//	public KHeartBeatMessage()
//	{
//		this.messageType = KMessageType.KHeart_Beat;
//	}
//}

[Serializable]  
[StructLayout(LayoutKind.Sequential,Pack = 1)]//按1字节对齐  
public struct HeartBeatStructData
{
	public int msgID;
	public bool boolValue;
	public short charValue;
	public float floatValue;
	public double doubleValue;
	[MarshalAsAttribute(UnmanagedType.ByValTStr,SizeConst=120)]  
	public string info;

	public void Deserialize (ByteBuffer data)
	{
		LogMgr.LogError("HeartBeatStructData");
		throw new NotImplementedException ();
	}
}
