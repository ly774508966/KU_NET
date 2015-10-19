using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;

namespace Kubility
{
	public class ProtobufMessageHead :MessageHead
	{

	}

	public class ProtobufMessageData
	{
		
	}

	public class ProtobufMessage : BaseMessage 
	{
		public byte[] ProtobufData
		{
			get
			{
				return this.DataBody.m_SecondValue;
			}
		}
		
		public static ProtobufMessage Create<T>(ProtobufMessageHead head,T data ) where  T :ProtobufMessageData
		{
			if(MessageInfo.MessageTypeCheck(MessageDataType.ProtoBuf))
			{
				ProtobufMessage message = new ProtobufMessage();
				message.head = head;
				message.DataBody.m_SecondValue = ParseUtils.ProtoBuf_SerializeAsBytes<T>(data);
				return message;
			}
			return null;
		}

		public override byte[] Serialize (bool addHead)
		{
			if(MessageInfo.MessageTypeCheck(MessageDataType.ProtoBuf))
			{
				ByteBuffer buffer = new ByteBuffer(1024);

				if (head != null && addHead)
				{
					head.bodyLen = (uint)DataBody.m_SecondValue.Length;
					buffer += head.Serialize();
				}
				else if (addHead && head == null)
				{
					LogMgr.LogError("head is Null");
				}

				buffer += DataBody.m_SecondValue;
				return buffer.ConverToBytes();

			}
			return null;
		}

		public override void Wait_Deserialize<T> (Action<T> ev)
		{
			if(MessageInfo.MessageTypeCheck(MessageDataType.ProtoBuf))
			{
				MessageManager.mIns.PushToWaitQueue(this, delegate(byte[] value)
				{
					T data =ParseUtils.ProtoBuf_DeserializeWithBytes<T>(value);
					ev(data);
				});
			}

		}
	}

}


