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


	}


	public class AbstractJsonData
	{
		


	}


	public class JsonMessage :BaseMessage 
	{


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

			if(jsonData != null)
				this.body.m_FirstValue +=ParseUtils.Json_Serialize(jsonData);
		}

		public override byte[] Serialize ()
		{
			ByteBuffer buffer = new ByteBuffer ();
			if(head != null)
				buffer += head.Serialize ();

			buffer += System.Text.Encoding.UTF8.GetBytes(this.body.m_FirstValue);
	
			return buffer.ConverToBytes();

		}




	}
}


