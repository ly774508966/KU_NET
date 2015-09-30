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




public class Manager : MonoBehaviour {
	
	KTcpClient client;
	KTcpServer server ;
	KThread th;

	bool isDone =false;
	float value =0;
	HttpClient http;
	
	// Use this for initialization
	void Start ()
	{


		http= new HttpClient();

		th = KThread.StartTask(testTh);


		client = new KTcpClient();
		client.Init("127.0.0.1",11000);
		StructMessageData mess;
		mess.boolValue= true;
		mess.charValue = 2;
		mess.doubleValue = 2.3;
		mess.floatValue = 1.5f;
		mess.info ="sjy";
		mess.msgID = 102;



		var head = new StructMessageHead();
		head.CMD = 102;

		head.bodyLen =(UInt32) Marshal.SizeOf(mess);
		
		var message =StructMessage.Create(head,mess);

		client.Send(message);//1
		client.Send(message);//2
		client.Send(message);//3
		client.Send(message);//4
		client.Send(message);//5
		client.Send(message);//6
		client.Send(message);//7
		client.Send(message);//8
		client.Send(message);//9
		
		
		
	}
	
	void test2()
	{
		AsynchronousClient.StartClient();
	}

	void testTh()
	{

		AsynchronousSocketListener.StartListening();


	}



	void OnGUI()
	{
		GUILayout.Label("process = "+ value.ToString(),GUILayout.Width(200));

		GUILayout.Space(10);
		GUILayout.Label("state = "+ isDone ,GUILayout.Width(200));

		if (GUILayout.Button("Pause",GUILayout.Width(200)))
		{
			http.Pause();
		}

		if (GUILayout.Button("Resume",GUILayout.Width(200)))
		{
			http.Resume();
		}

		if (GUILayout.Button("start",GUILayout.Width(200)))
		{
			http.BeginDownLoadFileFlushToFile("http://118.192.69.207:8083/monster.apk",
			                                           Application.persistentDataPath+"/mons.apk",
			                                           delegate(string arg1, float arg2, bool arg3)
			 {
				value = arg2;
				isDone = arg3;
				LogMgr.Log("Onprocess " +"  float  ="+ arg2.ToString()  +" isDone "+ arg3.ToString());
			});
			
			http.StartConnect();
		}

		if(isDone)
		{
			FileStream s = new FileStream(Application.persistentDataPath+"/test.txt",FileMode.OpenOrCreate, FileAccess.Read);
			byte[] bs =new byte[1024];
			s.Read(bs,0,200);


			GUILayout.Label(Encoding.UTF8.GetString(bs));

			s.Close();
		}
	}


	void OnApplicationQuit()
	{
		LogMgr.Log("OnApplicationQuit   = ");
		AsynchronousSocketListener.allDone.Set();

		AsynchronousSocketListener.stop = true;
//		if(th != null)
//			th.Abort();
		if(client != null)
		{
			client.quit = true;
			client.Close();
		}

		if(http != null)
		{
			http.Close();
		}
	}

}
