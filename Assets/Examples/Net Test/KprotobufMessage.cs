using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubility;
using ProtoBuf;
using System;

[ProtoContract]  
public class KprotobufMessage : ProtobufMessageData {

	[ProtoMember(1)]  
	public int Id  
	{  
		get;  
		set;  
	}  
	
	
	[ProtoMember(2)]  
	public List<String> data  
	{  
		get;  
		set;  
	}  
	
	
	public override string ToString()  
	{  
		string str = Id+":";  
		foreach (string d in data)  
		{  
			str += d + ",";  
		}  
		return str;  
	}  

}
