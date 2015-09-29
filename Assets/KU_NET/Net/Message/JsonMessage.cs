using UnityEngine;
using System.Collections;
using System;
using System.IO;


namespace Kubility
{
	public class JsonMessageHead:MessageHead
	{
		public JsonMessageHead()
		{
			this.Flag = 1;
		}

//		public override byte[] Serialize ()
//		{
//			return base.Serialize ();
//		}

//		public override void Initialize ()
//		{
//			base.Initialize ();
//		}
	}


	public class AbstractJsonData
	{
		public int msgid;

	}

	public class JsonMessage :BaseMessage 
	{

//		public JsonMessage()
//		{
//			this.head.WriteFlag(1);
//		}

		AbstractJsonData _jsonData;

		public AbstractJsonData jsonData
		{
			get
			{
				if(string.IsNullOrEmpty(body.m_FirstValue) && _jsonData != null)
				{
					Serialize_Data();
				}
				return _jsonData;
			}

			protected set
			{
				_jsonData = value;
			}
		}

		public static JsonMessage Create(KMessageType mtype = KMessageType.None,AbstractJsonData data =null,JsonMessageHead  jhead = null )
		{
			JsonMessage message = new JsonMessage();
			message.messageType =mtype;
			message.jsonData = data;
			if(jhead != null)
				message.head = jhead;
			return message;
		}

		public void Deserialize_Data<T>() where T :AbstractJsonData
		{
			if(!string.IsNullOrEmpty(this.body.m_FirstValue))
			{
				jsonData = ParseUtils.Json_Deserialize<T>(this.body.m_FirstValue);
			}

		}

		void Serialize_Data()
		{
			this.body.m_FirstValue =ParseUtils.Json_Serialize(jsonData);
		}

		public override byte[] Serialize ()
		{
			byte[] bys = new byte[head.bodyLen + 14];

//			int rlen = head.stream.Read(bys,0,14);
//			if(rlen != 14)
//			{
//				while(rlen >0)
//				{
//					rlen = head.stream.Read(bys,0,rlen);
//				}
//			}

			byte[] bs = System.Text.Encoding.UTF8.GetBytes(this.body.m_FirstValue);
			System.Array.Copy(bs,0,bys,14,bs.Length);
			return bys;

		}




	}
}


