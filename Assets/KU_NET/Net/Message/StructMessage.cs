using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Kubility
{
	
	public class StructMessageHead : MessageHead
	{
		public StructMessageHead()
		{
			this.Flag = 2;
		}

	}
	


	public sealed class StructMessage :BaseMessage
	{
		private ValueType _StructData;
		
		public ValueType StructData
		{
			get
			{
				return _StructData;
			}
			protected set
			{
				_StructData = value;
			}
		}
		
		public static StructMessage Create(MessageHead head,ValueType data)
		{
			StructMessage message = new StructMessage();
			message.head = head;
			message._StructData = data;
			return message;
		}
		
		public override void Wait_Deserialize<T> (Action<T> ev)  
		{

			MessageManager.mIns.PushToWaitQueue(this,delegate(ValueType value)
			{
			 	T data =(T)Convert.ChangeType(value,typeof(T));
				ev(data);
			});
		}
		
		
		public override byte[] Serialize (bool addHead =true)
		{
			ByteBuffer buffer = new ByteBuffer(512);
			if(head != null && addHead)
			{
				buffer += head.Serialize();
			}
			else if(addHead && head == null)
			{
				LogMgr.LogError("head is Null");
			}

			buffer +=  KTool.StructToBytes(_StructData);
			return buffer.ConverToBytes();
		}

	}
}


