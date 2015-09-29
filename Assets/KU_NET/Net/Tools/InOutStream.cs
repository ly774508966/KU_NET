#define StandAlone
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Adobe PS – Big Endian
///BMP – Little Endian
///DXF(AutoCAD) – Variable
///GIF – Little Endian
///JPEG – Big Endian
///MacPaint – Big Endian
///RTF – Little Endian
/// </summary>

public class ByteStream
{
	/// <summary>
	/// 托管和非托管资源总和，比如bool，非托管大小为1
	/// </summary>
	public const int BYTE_LEN = 1; 
	public const int INT32_LEN = 4;
	public const int LONG_LEN = 8;
	public const int SHORT16_LEN = 2;
	public const int FLOAT_LEN = 4;
	public const int BOOL_LEN =1;
	public const int DOUBLE_LEN = 8;

	public static int readByte(Stream input)
	{
		return input.ReadByte();
	}

	public static byte[] readByteAsByte(Stream input)
	{
		return new byte[1]{(byte)input.ReadByte()};
	}
	
	public static void writeByte(Stream output, byte bv)
	{
		output.WriteByte(bv);
	}
	
	public static int readdouble(Stream input,out double value)
	{
		byte[] bytes = new byte[DOUBLE_LEN];
		int rlen = input.Read(bytes, 0, DOUBLE_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif

		value = System.BitConverter.ToDouble(bytes, 0);
		return rlen - DOUBLE_LEN;
	}

	public static byte[] readdoubleAsBytes(Stream input,out int bvalue)
	{
		byte[] bytes = new byte[DOUBLE_LEN];
		int rlen = input.Read(bytes, 0, DOUBLE_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen -DOUBLE_LEN ;
		return bytes;
	}
	
	public static void writedouble(Stream output, double val)
	{
		byte[] bytes = System.BitConverter.GetBytes(val);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, DOUBLE_LEN);
	}

	public static int readfloat(Stream input,out float value)
	{
		byte[] bytes = new byte[FLOAT_LEN];
		int rlen = input.Read(bytes, 0, FLOAT_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToSingle(bytes, 0);
		return rlen - FLOAT_LEN;
	}

	public static byte[] readfloatAsBytes(Stream input,out int bvalue)
	{
		byte[] bytes = new byte[FLOAT_LEN];
		int rlen = input.Read(bytes, 0, FLOAT_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen- FLOAT_LEN ;
		return bytes;
	}
	
	public static void writefloat(Stream output, float val)
	{
		byte[] bytes = System.BitConverter.GetBytes(val);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, FLOAT_LEN);
	}
	
	public static int readBool(Stream input,int size, out bool value)
	{
		byte[] bytes = new byte[size];
		int rlen = input.Read(bytes, 0, size);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToBoolean(bytes, 0);
		return rlen - size;
	}

	public static byte[] readBoolAsBytes(Stream input,int size,out int bvalue)
	{
		byte[] bytes = new byte[size];
		int rlen = input.Read(bytes, 0, size);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - size ;

		return bytes;
	}
	
	public static void writeBool(Stream output,int size, bool valInt)
	{
		byte[] bytes ;
		if(size ==4)
		{
			bytes =System.BitConverter.GetBytes(valInt == true?1:0);
		}
		else 
			bytes =System.BitConverter.GetBytes(valInt);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, bytes.Length);
	}
	

	public static int readShort16(Stream input ,out short value )
	{
		byte[] bytes = new byte[SHORT16_LEN];
		int rlen = input.Read(bytes, 0, SHORT16_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToInt16(bytes, 0);
		return rlen  - SHORT16_LEN ;
		
	}

	public static byte[] readShort16AsBytes(Stream input ,out int bvalue )
	{
		byte[] bytes = new byte[SHORT16_LEN];
		int rlen = input.Read(bytes, 0, SHORT16_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - SHORT16_LEN ;

		return bytes ;
		
	}

	public static void writeShort16(Stream output, short valInt)
	{
		byte[] bytes = System.BitConverter.GetBytes(valInt);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, SHORT16_LEN);
	}

	public static byte[] readInt32AsBytes(Stream input,out int bvalue)
	{
		
		byte[] bytes = new byte[INT32_LEN];
		int rlen = input.Read(bytes, 0, INT32_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - INT32_LEN ;
		return bytes;
	}

	public static int readInt32(Stream input,out Int32 value)
	{
		
		byte[] bytes = new byte[INT32_LEN];
		int rlen = input.Read(bytes, 0, INT32_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToInt32(bytes, 0);
		return rlen - INT32_LEN;
	}

	public static void writeInt32(Stream output, Int32 valInt)
	{
		byte[] bytes = System.BitConverter.GetBytes(valInt);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, INT32_LEN);
	}

	public static int readUInt32(Stream input ,out UInt32 value)
	{
		
		byte[] bytes = new byte[INT32_LEN];
		int rlen = input.Read(bytes, 0, INT32_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToUInt32(bytes, 0);
		return rlen - INT32_LEN;
	}

	public static byte[] readUInt32AsBytes(Stream input ,out int bvalue)
	{
		
		byte[] bytes = new byte[INT32_LEN];
		int rlen = input.Read(bytes, 0, INT32_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - INT32_LEN ;
		return bytes;
	}

	public static void writeUInt32(Stream output, UInt32 valInt)
	{
		byte[] bytes = System.BitConverter.GetBytes(valInt);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, INT32_LEN);
	}


	public static int readUInt64(Stream input,out UInt64 value)
	{
		
		byte[] bytes = new byte[LONG_LEN];
		int rlen = input.Read(bytes, 0, LONG_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToUInt64(bytes, 0);
		return rlen - LONG_LEN ;
	}

	public static byte[] readUInt64AsBytes(Stream input,out int bvalue)
	{
		
		byte[] bytes = new byte[LONG_LEN];
		int rlen = input.Read(bytes, 0, LONG_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - LONG_LEN;
		return bytes ;
	}

	public static void writeUInt64(Stream output, UInt64 valInt)
	{
		byte[] bytes = System.BitConverter.GetBytes(valInt);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, LONG_LEN);
	}

	
	public static int readLong(Stream input ,out Int64 value)
	{
		byte[] bytes = new byte[LONG_LEN];
		int rlen = input.Read(bytes, 0, LONG_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		value = System.BitConverter.ToInt64(bytes, 0);
		return rlen - LONG_LEN;
	}

	public static byte[] readLongAsBytes(Stream input ,out int bvalue)
	{
		byte[] bytes = new byte[LONG_LEN];
		int rlen = input.Read(bytes, 0, LONG_LEN);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		bvalue = rlen - LONG_LEN ;
		return bytes;
	}
	
	public static void writeLong(Stream output, Int64 val)
	{
		byte[] bytes = System.BitConverter.GetBytes(val);
#if StandAlone
#else
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(bytes);
#endif
		output.Write(bytes, 0, LONG_LEN);
	}

	public static int readStringWithLength(Stream input,UInt32 len,out string value)
	{

		byte[] bytes = new byte[len];
		int rlen = input.Read(bytes, 0, (int)len);
		value  =System.Text.Encoding.UTF8.GetString(bytes, 0, (int)len);
		return (int)(rlen - len);
	}

	public static byte[] readStringWithLengthAsBytes(Stream input,UInt32 len,out int value)
	{
		
		byte[] bytes = new byte[len];
		int rlen = input.Read(bytes, 0, (int)len);

		value=(int)(rlen - len) ;
		return bytes;
	}



	public static int readString(Stream input,out string value)
	{
		UInt32 len ;
		int lrlen = readUInt32(input,out len);
		if(lrlen == 0)
		{
			byte[] bytes = new byte[len];
			int rlen = input.Read(bytes, 0, (int)len);
			value  =System.Text.Encoding.UTF8.GetString(bytes, 0, (int)len);
			return (int)(rlen - len);
		}
		else
		{
			value = null;
			return lrlen;
		}

	}

	public static byte[] readStringAsBytes(Stream input,out int value)
	{
		UInt32 len ;
		int lrlen = readUInt32(input,out len);
		if(lrlen == 0)
		{
			byte[] bytes = new byte[len];
			int rlen = input.Read(bytes, 0, (int)len);

			value = (int)(rlen - len);
			return bytes;
		}
		else
		{
			value = -lrlen;
			return null;
		}
		
	}
	
	public static void writeString(Stream output, string valStr)
	{
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(valStr);
		writeUInt32(output, (UInt32)bytes.Length);

		output.Write(bytes, 0, bytes.Length);
	}
	
}

