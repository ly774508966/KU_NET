using UnityEngine;
using System.Collections;
using Kubility;

public static class FactoryUtils 
{
	public static void InitFactory()
	{
		new StructMessFactory(MessageDataType.Struct);
		new JsonMessFactory(MessageDataType.Json);
		new ProtobufMessFactory(MessageDataType.ProtoBuf);
	}

}
