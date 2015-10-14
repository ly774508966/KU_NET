using UnityEngine;
using System.Collections;
using System;

namespace Kubility
{
	public class NetByteBuffer :ByteBuffer
	{
//		public NetByteBuffer():base()
//		{
//
//		}

		public NetByteBuffer(int size):base(size)
		{

		}

		public NetByteBuffer(byte[] bys):base(bys)
		{

		}

		public override byte[] Read(int begin,int len)
		{
			if(len > DataCount-begin)
				len = DataCount-begin;
			
			byte[] bys = new byte[len];
			
			int i =0,j=0;
			for(i = begin; i < begin + len;++i,++j)
			{
				bys[j] = this.buffer[i];
				
			}
			if(DataCount-i- 1 >0)
			{
				System.Array.Copy(buffer,i,buffer,begin,DataCount-len);
				Position -= len;
			}
			else
			{
				ReSize(0);
			}

			if(BitConverter.IsLittleEndian)
				Array.Reverse(bys);

			return bys;
		}
		
		#region operater

		public static NetByteBuffer operator  +(NetByteBuffer left,NetByteBuffer right)
		{
			if(right != null)
			{
				int nextsize = right.DataCount + left.Position;
				if(nextsize > left.buffer.Length)
				{
					left.IncreaseCapacity(nextsize);
				}
				
				int endpos = left.Position +right.DataCount;
				for(int i=left.Position,j=0; i < endpos ;++i,++j)
				{
					left.buffer[i] = right.buffer[j];
				}
				right.Position =0;
				
				left.Position += right.DataCount;
			}
			
			return left;
		}
		public static NetByteBuffer operator  +(NetByteBuffer left,byte[] right)
		{
			if(right != null)
			{
				int nextsize = right.Length + left.Position;
				if(nextsize > left.buffer.Length)
				{
					left.IncreaseCapacity(nextsize);
				}
				if(BitConverter.IsLittleEndian)
					Array.Reverse(right);
				
				Array.Copy(right,0,left.buffer,left.Position,right.Length);
				left.Position += right.Length;
			}
			
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,int right)
		{
			left +=BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator +(NetByteBuffer left,Bool8 right)
		{
			left += right.GetBytes ();
			return left;
		}
		
		public static NetByteBuffer operator +(NetByteBuffer left,Bool32 right)
		{
			left += right.GetBytes ();
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,float right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,double right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,short right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static NetByteBuffer operator  +(NetByteBuffer left,long right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,uint right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,ulong right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static NetByteBuffer operator  +(NetByteBuffer left,ushort right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}
		
		public static NetByteBuffer operator  +(NetByteBuffer left,string right)
		{
			left += right.Length;
			left += System.Text.Encoding.UTF8.GetBytes(right);
			return left;
		}
		
		
		#region Read
		public static explicit  operator Bool8 (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.BYTE_LEN)
			{
				byte[] tempBys = new byte[ByteStream.BYTE_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.BYTE_LEN);
				left.Clear(ByteStream.BYTE_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return Bool8.ToBool8(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(Bool8);
		}
		
		public static explicit  operator Bool32 (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return Bool32.ToBool32(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return false;
		}
		
		
		public static explicit  operator int (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToInt32(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(int);
		}
		
		public static explicit  operator uint (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToUInt32(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(UInt32);
		}
		
		public static explicit  operator short (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.SHORT16_LEN)
			{
				byte[] tempBys = new byte[ByteStream.SHORT16_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.SHORT16_LEN);
				left.Clear(ByteStream.SHORT16_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToInt16(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(short);
		}
		
		public static explicit  operator float (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.FLOAT_LEN)
			{
				byte[] tempBys = new byte[ByteStream.FLOAT_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.FLOAT_LEN);
				left.Clear(ByteStream.FLOAT_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToSingle(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(float);
		}
		
		public static explicit  operator double (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.DOUBLE_LEN)
			{
				byte[] tempBys = new byte[ByteStream.DOUBLE_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.DOUBLE_LEN);
				left.Clear(ByteStream.DOUBLE_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToDouble(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(double);
		}
		
		public static explicit  operator ushort (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.SHORT16_LEN)
			{
				byte[] tempBys = new byte[ByteStream.SHORT16_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.SHORT16_LEN);
				left.Clear(ByteStream.SHORT16_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToUInt16(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(ushort);
		}
		
		public static explicit  operator ulong (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.LONG_LEN)
			{
				byte[] tempBys = new byte[ByteStream.LONG_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.LONG_LEN);
				left.Clear(ByteStream.LONG_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToUInt64(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(ulong);
		}
		
		public static explicit  operator long (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.LONG_LEN)
			{
				byte[] tempBys = new byte[ByteStream.LONG_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.LONG_LEN);
				left.Clear(ByteStream.LONG_LEN);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return BitConverter.ToInt64(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(long);
		}
		
		public static explicit  operator string (NetByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				int strLen = (int)left;
				if(strLen == 0)
				{
					return null;
				}
				byte[] tempBys = new byte[strLen];
				Array.Copy(left.buffer,0,tempBys,0,strLen);
				left.Clear(strLen);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(tempBys);
				return System.Text.Encoding.UTF8.GetString(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return null;
		}
		
		#endregion
		
		#endregion
	}
}


