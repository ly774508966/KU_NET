//#define Protobuf
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Threading;
#if Protobuf
using ProtoBuf;
#endif

namespace Kubility
{
    public enum KMessageType
    {
        None,
        KHeart_Beat,
    }

	public enum MessageDataType
	{
		Struct,
		Json,
		ProtoBuf,
	}

    public class MessageHead
    {

        protected NetByteBuffer _buffer;
        /// <summary>
        /// unique ID
        /// </summary>
        protected MiniTuple<UInt32, int> _Version;
		/// <summary>
		/// 主版本号
		/// </summary>

        protected MiniTuple<Int16, int> _CMD;
        ///<summary> 
        ///子版本号
        ///</summary>
        protected MiniTuple<Int16, int> _subCmd;
        ///<summary> 
        ///包体长度
        ///</summary>
        protected MiniTuple<Int32, int> _bodyLen;
        ///<summary> 
        ///校验码
        ///</summary>
        protected MiniTuple<Int32, int> _code;

//        protected MiniTuple<short, int> _Flag;

        public Int32 bodyLen
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

        public Int16 CMD
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
        
        public Int16 Sub_CMD
        {
            get
            {
                return _subCmd.field0;
            }

            set
            {
                _subCmd.field0 = value;
            }
        }
        
        public Int32 Code
        {
            get
            {
                return _code.field0;
            }

            set
            {
                _code.field0 = value;
            }
        }

        public NetByteBuffer buffer
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

        }

        public virtual void Reset()
        {
            this._bodyLen = new MiniTuple<int, int>();
            this._CMD = new MiniTuple<short, int>();
            this._subCmd = new MiniTuple<short,int>();
            this._code = new MiniTuple<int,int>();
            this._Version = new MiniTuple<uint, int>();

            this.buffer.Clear();
        }

        public virtual void Read(Stream buffer)
        {
            // this._Version.field1 = ByteStream.readUInt32(buffer, out this._Version.field0);
//            this._Flag.field1 = ByteStream.readShort16(buffer, out this._Flag.field0);
            
            this._bodyLen.field1 = ByteStream.readInt32(buffer, out this._bodyLen.field0);
            this._CMD.field1 = ByteStream.readShort16(buffer, out this._CMD.field0);
            this._subCmd.field1 = ByteStream.readShort16(buffer,out this._subCmd.field0);
            this._code.field0 = ByteStream.readInt32(buffer,out this._code.field0);

        }

        public virtual void Read(byte[] bytes)
        {
            buffer = new NetByteBuffer(bytes);
//            Version = (UInt32)buffer;
//            Flag = (short)buffer;
			bodyLen = (int)buffer;
            CMD = (short)buffer;
			Sub_CMD = (short)buffer;
			Code =(int)buffer;
           
        }

        public virtual byte[] Serialize()
        {

            NetByteBuffer by = new NetByteBuffer(MessageInfo.HeadLen);
//            by += Version;
//            by += Flag;
			by += bodyLen;
            by += CMD;
			by += Sub_CMD;
			by += Code;
           

            return by.ConverToBytes();
        }

    }


    public class BaseMessage
    {
        /// <summary>
        /// The type of the message.
        /// </summary>
        protected KMessageType messageType;

        public KMessageType MessageType
        {
            get
            {
                return messageType;
            }
			set
			{
				messageType = value;
			}
        }
        /// <summary>
        /// content
        /// </summary>
        protected Union<string, byte[]> m_DataBody;
		public Union<string, byte[]> DataBody
		{
			get
			{
				return m_DataBody;
			}

			set
			{
				m_DataBody = value;
			}
		}


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
            set
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
            MessageHead head = new MessageHead();
            head.Read(bytes);
            return head;
        }


        public int GetPriority()
        {
            return (int)messageType;
        }

        public virtual byte[] Serialize(bool addHead = true)
        {
			LogMgr.LogError("no override please  check");
            return null;
        }

        public virtual void Wait_Deserialize<T>(Action<T> ev)
        {
			LogMgr.LogError("no override please  check");
        }

    }
}