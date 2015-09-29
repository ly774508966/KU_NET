using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding.Serialization.JsonFx;
using JsonFx.Xml;
using System.Xml;
using System.IO;
/// <summary>
/// 方便以后切换json库等
/// </summary>
public static class ParseUtils 
{

    public static T Json_Deserialize<T>(string value)
    {
        return  JsonReader.Deserialize <T>(value);
    }

    public static string Json_Serialize(object value)
    {
        return JsonWriter.Serialize(value);
    }

    public static T CoerceType<T>(object value)
    {
        return JsonReader.CoerceType<T>(value);
    }

    public static T XML_Deserialize<T>(string path)
    {

        var reader = new XmlDataReader(XmlDataReader.CreateSettings(), null);


        using (var textReader = new StreamReader(path))
        {
            return (T)reader.Deserialize(textReader,typeof(T));
        }

    }

    public static string XML_Serialize(object data)
    {
//        XmlWriterSettings setting = XmlDataWriter.CreateSettings(System.Text.Encoding.UTF8, true);
//        XmlDataWriter writer = new XmlDataWriter(setting,null);
//
//        string tempPath = Application.dataPath + "/Scripts/kb/lib/temp.txt";
//        string output;
//        using (TextWriter streamWriter = new StreamWriter(tempPath))
//        {
//            writer.Serialize(streamWriter, data);
//            output = streamWriter.ToString();
//        }

        //if (File.Exists(tempPath))
        //    File.Delete(tempPath);
//        return output;

		return null;
    }

}
