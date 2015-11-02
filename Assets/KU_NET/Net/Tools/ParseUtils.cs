//#define JSONFX
#define JsonDotNet
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
#if JSONFX
using Pathfinding.Serialization.JsonFx;
#elif JsonDotNet
using JsonDotNet;
using JsonDotNet.Extras;
using Newtonsoft.Json;
#elif ProtoBuf
using ProtoBuf;
#endif



/// <summary>
/// 方便以后切换json库等
/// </summary>
public static class ParseUtils
{
	/// <summary>
	/// json反序列化
	/// </summary>
	/// <returns>The deserialize.</returns>
	/// <param name="value">Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T Json_Deserialize<T>(string value)
    {
#if JsonDotNet
		return (T)JsonConvert.DeserializeObject(value,typeof(T));
#elif JSONFX
        return JsonReader.Deserialize<T>(value);
#endif
    }
	/// <summary>
	/// json反序列化
	/// </summary>
	/// <returns>The deserialize.</returns>
	/// <param name="value">Value.</param>
    public static object Json_Deserialize(string value)
    {
#if JsonDotNet
		return JsonConvert.DeserializeObject(value);
#elif JSONFX
        return JsonReader.Deserialize(value);
#endif
    }
	/// <summary>
	/// json序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="t"></param>
	/// <returns></returns>
    public static string Json_Serialize(object value)
    {
#if JsonDotNet
		return JsonConvert.SerializeObject(value);
#elif JSONFX
        return JsonWriter.Serialize(value);
#endif
    }

    public static T CoerceType<T>(object value)
    {
#if JsonDotNet
		throw new Exception("cant use CoerceType.plz use jsonfx");
#elif JSONFX
        return JsonReader.CoerceType<T>(value);
#endif
    }

	/// <summary>
	/// protobuf序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="t"></param>
	/// <returns></returns>
	public static string ProtoBuf_Serialize<T>(T t)
	{
#if ProtoBuf
		using (MemoryStream ms = new MemoryStream())
		{
			Serializer.Serialize<T>(ms, t);
			return Encoding.UTF8.GetString(ms.ToArray());
		}
#else
		return null;
	}
	/// <summary>
	/// protobuf序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="t"></param>
	/// <returns></returns>
	public static byte[] ProtoBuf_SerializeAsBytes<T>(T t)
	{
#if ProtoBuf
		using (MemoryStream ms = new MemoryStream())
		{
			Serializer.Serialize<T>(ms, t);
			return ms.ToArray();
		}
#else
		return null;
#endif
	}
	/// <summary>
	///protobuf 反序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="content"></param>
	/// <returns></returns>
	public static T  ProtoBuf_Deserialize<T>(string content)
	{
#if ProtoBuf
		using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
		{
			T t = Serializer.Deserialize<T>(ms);
			return t;
		}
#else
		return default(T);
#endif
	}

	/// <summary>
	///protobuf 反序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="content"></param>
	/// <returns></returns>
	public static T  ProtoBuf_DeserializeWithBytes<T>(byte[] content)
	{
#if ProtoBuf
		using (MemoryStream ms = new MemoryStream(content))
		{
			T t = Serializer.Deserialize<T>(ms);
			return t;
		}
#else
		return default(T);
#endif
	}

}
