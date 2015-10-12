using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Threading;


namespace Kubility
{
	public enum KMessageType
	{
		None,
		KHeart_Beat,
	}



	
	public class MessageHead
	{
		
		protected ByteBuffer _buffer;
		
		public const int HeadLen = 14;
		
		/// <summary>
		/// unique ID
		/// </summary>
		protected MiniTuple<UInt32,int> _Version;
		
		protected MiniTuple<UInt32,int> _CMD;
		
		protected MiniTuple<UInt32,int> _bodyLen;
		/// <summary>
		/// 0:default  1:json  2:struct
		/// </summary>
		protected MiniTuple<short,int> _Flag;
		
		public UInt32 bodyLen
		{
			get
			{
				return _bodyLen.field0;
			}
			
			set
			{
				_bodyLen.field0 = value;
			}
		}
		
		public UInt32 Version
		{
			get
			{
				return _Version.field0;
			}
			
			protected set
			{
				_Version.field0 = value;
			}
		}
		
		public UInt32 CMD
		{
			get
			{
				return _CMD.field0;
			}
			
			set
			{
				_CMD.field0 = value;
			}
		}
		
		public short Flag
		{
			get
			{
				return _Flag.field0;
			}
			
			protected set
			{
				_Flag.field0 = value;
			}
		}
		
		public ByteBuffer buffer
		{
			get
			{
				return _buffer;
			}
			
			set
			{
				_buffer = value;
			}
		}
		
		public MessageHead()
		{

			this.Version = Config.mIns.Version;
			this.buffer = new ByteBuffer();
		}
		
		public virtual void Reset()
		{
			this._bodyLen = new MiniTuple<uint, int>();
			this._bodyLen.field0 =0;
			this._bodyLen.field1=0;
			
			this._CMD = new MiniTuple<uint, int>();
			this._CMD.field0 =0;
			this._CMD.field1 = 0;
			
			this._Flag = new MiniTuple<short, int>();
			this._Flag.field0 = 0;
			this._Flag.field1 = 0;
			
			this._Version = new MiniTuple<uint, int>();
			this._Version.field0 = 0;
			this._Version.field1 = 0;
			
			this.buffer.Clear();
		}
		
		public virtual void Read(Stream buffer)
		{
			this._Version.field1 = ByteStream.readUInt32(buffer,out this._Version.field0);
			this._Flag.field1 = ByteStream.readShort16(buffer,out this._Flag.field0);
			this._CMD.field1 = ByteStream.readUInt32(buffer,out this._CMD.field0);
			this._bodyLen.field1 = ByteStream.readUInt32(buffer,out this._bodyLen.field0);
			
		}
		
		public virtual void  Read(byte[] bytes)
		{
			buffer += bytes;
			Version =(UInt32)buffer;
			Flag =(short)buffer;
			CMD =(UInt32)buffer;
			bodyLen =(UInt32)buffer;
		}
		
		public virtual byte[] Serialize()
		{
			byte[] bys = new byte[14];
			var bUid = BitConverter.GetBytes(Version);
			var bFlag = BitConverter.GetBytes(Flag);
			var bCMD = BitConverter.GetBytes(CMD);
			var byLen = BitConverter.GetBytes(bodyLen);
			
			System.Array.Copy(bUid,0,bys,0,bUid.Length);
			System.Array.Copy(bFlag,0,bys,4,bFlag.Length);
			System.Array.Copy(bCMD,0,bys,6,bCMD.Length);
			System.Array.Copy(byLen,0,bys,10,byLen.Length);
			
			return bys;
		}
		
		public static MessageHead Create(byte[] bytes)
		{
			MessageHead head = new MessageHead();
			head.Read(bytes);
			return head;
		}
	}
	
	public enum BaseMessageErrorCode
	{
		Success,
		HeadLess,
		BodyOver,
		BodyLess,
	}
	
	
	
	public class BaseMessage
	{
		/// <summary>
		/// The type of the message.
		/// </summary>
		protected KMessageType messageType ;
		
		public KMessageType MessType
		{
			get
			{
				return messageType;
			}
		}
		/// <summary>
		/// content
		/// </summary>
		protected Union<string,byte[]> DataBody;


		/// <summary>
		/// flag
		/// </summary>
		protected MessageHead head;
		
		public MessageHead DataHead
		{
			get
			{
				return head;
			}
			protected set
			{
				head = value;
			}
		}
		
		
		public BaseMessage()
		{
			
			this.messageType = KMessageType.None;
			this.head = new MessageHead();
			this.DataBody = new Union<string, byte[]>();
			
		}

		
		public static MessageHead ReadHead(byte[] bytes)
		{
			return MessageHead.Create(bytes);
		}
		
		
		public int GetPriority()
		{
			return  (int)messageType;
		}
		
		public virtual byte[] Serialize(bool addHead =true)
		{
			return null;
		}

		public virtual void Wait_Deserialize<T> (Action<T> ev) 
		{

		}
		
	}
}