using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubility;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class Test
{
	public int vid = 12;
	public string mm = "saasd";
	public long sda = 1234;
	public float sdags = 23.123f;
	public double mka = 45.1239f;
	public string value = "dashfalsfllfabksdbfkabsbfdffF";
	public ushort asda = 23;
	public char da = 'd';
	public List<int> val = new List<int> ();
}


public class Manager : MonoBehaviour
{
	
	KTcpClient client;
	KTcpServer server;


	bool isDone = false;
	float value = 0;
	HttpClient http;
	
	// Use this for initialization
	void Start ()
	{


		var buffer = new NetByteBuffer (1024);
		buffer += 1.452f;
		buffer += (uint)12;
		buffer += "sda1asd";

		var v1 = (float)buffer;
		var v2 = (int)buffer;
		var v3 = (string)buffer;

		http = new HttpClient ();
		KThread.StartTask (ThreadListener);

		client = new KTcpClient ();
		client.Init ("127.0.0.1", 11000);
		HeartBeatStructData mess;
		mess.boolValue = true;
		mess.charValue = 2;
		mess.doubleValue = 2.3;
		mess.floatValue = 1.5f;
		mess.info = "i'm fine @ ~~~ 你好";
		mess.msgID = 102;



		var head = new StructMessageHead ();
		head.CMD = 102;

		head.bodyLen = (UInt32)Marshal.SizeOf (mess);
		
		var message = StructMessage.Create (head, mess);

		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});
		client.Send<HeartBeatStructData> (message, delegate(HeartBeatStructData obj) {
			LogMgr.LogError ("callback");
//			KTool.Dump (obj);
		});


		var test = new Test ();
		test.val.Add (123);
		test.val.Add (246);
		test.val.Add (45);
		var Jsonhead = new JsonMessageHead ();
		var jsonMessage = JsonMessage.Create<Test> (test, Jsonhead);

		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
			KTool.Dump (obj);

		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});
		client.Send<Test> (jsonMessage, delegate(Test obj) {
			LogMgr.LogError ("json");
//			KTool.Dump (obj);
		});

	}


	void ThreadListener ()
	{

		AsynchronousSocketListener.StartListening ();
	}

	void OnGUI ()
	{
		GUILayout.Label ("process = " + value.ToString (), GUILayout.Width (200));

		GUILayout.Space (10);
		GUILayout.Label ("state = " + isDone, GUILayout.Width (200));

		if (GUILayout.Button ("Pause", GUILayout.Width (200))) {
			http.Pause ();
		}

		if (GUILayout.Button ("Resume", GUILayout.Width (200))) {
			http.Resume ();
		}

		if (GUILayout.Button ("start", GUILayout.Width (200))) {
			http.BeginDownLoadFileFlushToFile ("http://118.192.69.207:8083/monster.apk",
				Application.persistentDataPath + "/mons.apk",
			delegate(byte[] arg1, float arg2, bool arg3) 
			{
					value = arg2;
					isDone = arg3;
				LogMgr.Log ("FlushToFile Onprocess " + "  float  =" + arg2.ToString () + " isDone " + arg3.ToString ());
				});
			
			http.StartConnect ();

//			http.BeginDownLoadFileFlushToMemory ("http://118.192.69.207:8083/monster.apk",
////			                                   Application.persistentDataPath + "/mons.apk",
//			delegate(byte[] arg1, float arg2, bool arg3)
//			{
//				value = arg2;
//				isDone = arg3;
//				LogMgr.Log ("FlushToMemory Onprocess " + "  float  =" + arg2.ToString () + " isDone " + arg3.ToString ());
//			});
//			
//			http.StartConnect ();
		}

	}


	void OnApplicationQuit ()
	{
		LogMgr.Log ("OnApplicationQuit   = ");
		AsynchronousSocketListener.allDone.Set ();

		AsynchronousSocketListener.stop = true;

		if (client != null) {
			client.quit = true;
			client.Close ();
		}

		if (http != null) {
			http.Close ();
		}

		KObject.Dump ();
	}

}
