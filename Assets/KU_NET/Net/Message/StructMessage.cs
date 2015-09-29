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

	[Serializable]  
	[StructLayout(LayoutKind.Sequential,Pack = 1)]//按1字节对齐  
	public struct StructMessageData
	{
		public int msgID;
		public bool boolValue;
		public short charValue;
		public float floatValue;
		public double doubleValue;
		[MarshalAsAttribute(UnmanagedType.ByValTStr,SizeConst=120)]  
		public string info;

		public static void Init(byte[] Bys,out StructMessageData data)
		{
			MemoryStream stream = new MemoryStream(Bys);
			ByteStream.readInt32(stream,out data.msgID);
			ByteStream.readBool(stream,4,out data.boolValue);
			ByteStream.readShort16(stream,out data.charValue);
			ByteStream.readfloat(stream,out data.floatValue);
			ByteStream.readdouble(stream,out data.doubleValue);
			ByteStream.readStringWithLength(stream,120,out data.info);
			stream.Close();
		}

	}

	public class StructMessage :BaseMessage
	{
		protected StructMessageData? _StructData;

		public StructMessageData? StructData
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

		public static StructMessage Create(MessageHead head,StructMessageData data)
		{
			StructMessage message = new StructMessage();
			message.head = head;
			message.StructData = data;
			return message;
		}


		public override byte[] Serialize ()
		{

			if(head != null)
			{
				ByteBuffer buffer = new ByteBuffer();

				buffer += head.Serialize();
				buffer += StructToBytes(StructData);

				return buffer.ConverToBytes();
			}
			else
			{
				LogMgr.LogError(" Error  head is Null");
			}


			return null;
		}


		public static byte[] StructToBytes(object structObj)
		{
			if(structObj != null)
			{
				int size =  Marshal.SizeOf(structObj);
				
				IntPtr buffer = Marshal.AllocHGlobal(size);
				try
				{
					Marshal.StructureToPtr(structObj, buffer, true);
					byte[] bytes = new byte[size];
					Marshal.Copy(buffer, bytes, 0, size);
					return bytes;
				}
				finally
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
			else
				return null;

		}
		
		public static object BytesToStruct(byte[] bytes, Type strcutType)
		{
			int size =  Marshal.SizeOf(strcutType);
			IntPtr buffer = Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(bytes, 0, buffer, size);
				return Marshal.PtrToStructure(buffer, strcutType);
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		}

	}
}


