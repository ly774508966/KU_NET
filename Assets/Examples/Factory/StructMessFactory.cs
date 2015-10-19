using UnityEngine;
using System.Collections;
using Kubility;

public class StructMessFactory : DataInterface
{
	public StructMessFactory(MessageDataType type):base(type)
	{

	}

	public override BaseMessage DynamicCreate (byte[] data, MessageHead head)
	{
		if(MessageInfo.MessageTypeCheck(MessageDataType.Struct))
		{
			if(head.CMD == 102)
			{
				var rdata = (HeartBeatStructData)KTool.BytesToStruct(data,typeof(HeartBeatStructData));
				StructMessage value = StructMessage.Create(head,rdata);
				return value;
			}
			

		}
		return null;
	}
}
