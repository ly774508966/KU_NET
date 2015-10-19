using UnityEngine;
using System.Collections;
using Kubility;

public class ProtobufMessFactory : DataInterface
{
	public ProtobufMessFactory(MessageDataType type):base(type)
	{
		
	}

	public override BaseMessage DynamicCreate (byte[] data, MessageHead head)
	{
		if(MessageInfo.MessageTypeCheck(MessageDataType.ProtoBuf))
		{
			ProtobufMessage message = new ProtobufMessage();
			message.DataHead = head;
			message.DataBody.m_SecondValue = data;
			return message;
		}

		return null;

	}
}
