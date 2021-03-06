﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Kubility
{

    public class StructMessageHead : MessageHead
    {
    }



    public sealed class StructMessage : BaseMessage
    {
        private ValueType _StructData;

        public ValueType StructData
        {
            get
            {
                return _StructData;
            }
            private set
            {
                _StructData = value;
            }
        }

        public static StructMessage Create(MessageHead head, ValueType data)
        {
			if(MessageInfo.MessageTypeCheck(MessageDataType.Struct))
			{
				StructMessage message = new StructMessage();
				message.head = head;
				message._StructData = data;
				return message;
			}

			return null;
        }

        public override void Wait_Deserialize<T>(Action<T> ev)
        {
			if(MessageInfo.MessageTypeCheck(MessageDataType.Struct))
			{
				MessageManager.mIns.PushToWaitQueue(this, delegate(ValueType value)
				                                    {
					T data = (T)Convert.ChangeType(value, typeof(T));
					ev(data);
				});
			}


        }


        public override byte[] Serialize(bool addHead = true)
        {
			if(MessageInfo.MessageTypeCheck(MessageDataType.Struct))
			{
				ByteBuffer buffer = new ByteBuffer(512);
				if (head != null && addHead)
				{
					buffer += head.Serialize();
				}
				else if (addHead && head == null)
				{
					LogMgr.LogError("head is Null");
				}
				
				buffer += KTool.StructToBytes(_StructData);
				return buffer.ConverToBytes();
			}
			return null;

        }

    }
}


