using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Kubility
{


	/// <summary>
	/// 本地数据缓冲类(as ADT)
	/// </summary>
	public class ByteBuffer 
	{
		public int DataCount
		{
			get
			{
				return Position;
			}
		}


		protected byte[] buffer;

		protected int Position;

//		public ByteBuffer()
//		{
//			buffer = new byte[1024];
//			Position = 0;
//		}

		public ByteBuffer(int size)
		{
			buffer = new byte[size];
			Position = 0;
		}

		public ByteBuffer(byte[] data) 
		{
			buffer = new byte[data.Length];
			Position = data.Length;

			System.Array.Copy(data,0,buffer,0,data.Length);
		}

		public void ReSize(int size)
		{
			if(size <0)
			{
				LogMgr.LogError("Resize error");
				return;
			}

			if(size > buffer.Length)
			{
				System.Array.Resize(ref buffer,size);
			}
			else if( size < buffer.Length)
			{
				Position = size;

				System.Array.Resize(ref buffer,size+1);//at least 1 byte
				buffer[size] =0;//clear

			}
		}

		public virtual byte[] Read(int begin,int len)
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
			return bys;
		}

		public void Copy(int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			System.Array.Copy(buffer,sourceIndex,destinationArray,destinationIndex,length);
		}

		protected void IncreaseCapacity(int size)
		{
			byte[] newbuffer = new byte[size];
			Array.Copy(buffer,0,newbuffer,0,buffer.Length);
			buffer = null;
			buffer = newbuffer;

		}

		/// <summary>
		/// 清理指定大小的数据
		/// </summary>
		/// <param name="len">Length.</param>
		public void Clear(int len =-1)
		{
			if(len > buffer.Length)
			{
				//all clear
				Clear();
			}
			else if(len >0)
			{
				int newLen = buffer.Length -len;
				for (int i = 0; i < buffer.Length ; i++) //否则后面的数据往前移
				{
					if(i < newLen)
						buffer[i] = buffer[len + i];
					else
						buffer[i] =0;
				}
				Position -= len;

			}
			else
			{
				buffer = null;
				buffer = new byte[512];
				Position =0;
			}

			
		}

		public byte[] ConverToBytes(bool clean =false)
		{
			if(DataCount >0)
			{
				byte[] newbys = new byte[DataCount];
				Copy(0,newbys,0,DataCount);
				if(clean)
				{
					Clear(DataCount);
				}
				return newbys;
			}
			else
				return null;

		}


		public Stream ConverToStream()
		{
			return new MemoryStream(buffer);
		}



		#region operater

		public static ByteBuffer operator  +(ByteBuffer left,ByteBuffer right)
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

		public static ByteBuffer operator  +(ByteBuffer left,byte[] right)
		{
			if(right != null)
			{
				int nextsize = right.Length + left.Position;
				if(nextsize > left.buffer.Length)
				{
					left.IncreaseCapacity(nextsize);
				}
				
				Array.Copy(right,0,left.buffer,left.Position,right.Length);
				left.Position += right.Length;
			}

			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,int right)
		{
			left +=BitConverter.GetBytes(right);	
			return left;
		}


		public static ByteBuffer operator +(ByteBuffer left,Bool8 right)
		{
			left += right.GetBytes ();
			return left;
		}

		public static ByteBuffer operator +(ByteBuffer left,Bool32 right)
		{
			left += right.GetBytes ();
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,float right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,double right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,short right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,long right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,uint right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,ushort right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}

		public static ByteBuffer operator  +(ByteBuffer left,ulong right)
		{
			left += BitConverter.GetBytes(right);	
			return left;
		}


		public static ByteBuffer operator  +(ByteBuffer left,string right)
		{
			left += right.Length;
			left += System.Text.Encoding.UTF8.GetBytes(right);
			return left;
		}


		#region Read
		public static explicit  operator Bool8 (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.BYTE_LEN)
			{
				byte[] tempBys = new byte[ByteStream.BYTE_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.BYTE_LEN);
				left.Clear(ByteStream.BYTE_LEN);
				return Bool8.ToBool8(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(Bool8);
		}

		public static explicit  operator Bool32 (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				return Bool32.ToBool32(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return false;
		}


		public static explicit  operator int (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				return BitConverter.ToInt32(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(int);
		}

		public static explicit  operator uint (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.INT32_LEN)
			{
				byte[] tempBys = new byte[ByteStream.INT32_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.INT32_LEN);
				left.Clear(ByteStream.INT32_LEN);
				return BitConverter.ToUInt32(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(UInt32);
		}

		public static explicit  operator short (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.SHORT16_LEN)
			{
				byte[] tempBys = new byte[ByteStream.SHORT16_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.SHORT16_LEN);
				left.Clear(ByteStream.SHORT16_LEN);
				return BitConverter.ToInt16(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(short);
		}

		public static explicit  operator float (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.FLOAT_LEN)
			{
				byte[] tempBys = new byte[ByteStream.FLOAT_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.FLOAT_LEN);
				left.Clear(ByteStream.FLOAT_LEN);
				return BitConverter.ToSingle(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(float);
		}

		public static explicit  operator double (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.DOUBLE_LEN)
			{
				byte[] tempBys = new byte[ByteStream.DOUBLE_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.DOUBLE_LEN);
				left.Clear(ByteStream.DOUBLE_LEN);
				return BitConverter.ToDouble(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(double);
		}

		public static explicit  operator ushort (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.SHORT16_LEN)
			{
				byte[] tempBys = new byte[ByteStream.SHORT16_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.SHORT16_LEN);
				left.Clear(ByteStream.SHORT16_LEN);
				return BitConverter.ToUInt16(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(ushort);
		}

		public static explicit  operator ulong (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.LONG_LEN)
			{
				byte[] tempBys = new byte[ByteStream.LONG_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.LONG_LEN);
				left.Clear(ByteStream.LONG_LEN);
				return BitConverter.ToUInt64(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(ulong);
		}

		public static explicit  operator long (ByteBuffer left)
		{
			if(left != null && left.DataCount >= ByteStream.LONG_LEN)
			{
				byte[] tempBys = new byte[ByteStream.LONG_LEN];
				Array.Copy(left.buffer,0,tempBys,0,ByteStream.LONG_LEN);
				left.Clear(ByteStream.LONG_LEN);
				return BitConverter.ToInt64(tempBys,0);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return default(long);
		}

		public static explicit  operator string (ByteBuffer left)
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
				return System.Text.Encoding.UTF8.GetString(tempBys);
			}
			LogMgr.LogError("Read from ByteBuffer Error");
			return null;
		}

		#endregion

		#endregion

	}
}


