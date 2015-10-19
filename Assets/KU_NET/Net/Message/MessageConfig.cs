using UnityEngine;
using System.Collections;

namespace Kubility
{
	[System.Serializable]
	public class MessageInfo 
	{
		static MessageDataType m_MessageType = MessageDataType.Struct;
		public static MessageDataType MessageType
		{
			get
			{
				return m_MessageType;
			}

			set
			{
				DataInterface.ConvertDataCore(value);
				m_MessageType =value;
			}
		}

		public static int HeadLen = 12;
		public static bool MessageTypeCheck(MessageDataType type)
		{
			if(type != MessageInfo.MessageType)
			{
				LogMgr.LogError("cant use "+type+"  because your MessageType is "+MessageInfo.MessageType);
				return false;
			}

			return true;
		}
	}
}

