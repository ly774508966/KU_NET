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

public class TestJson
{
	
	public string userToken;
}

public class TestJson2 :TestJson
{
	public string name ="cjsjy123";
	public string password ="123123";
}

public class t2
{
	public void test1()
	{
		LogMgr.Log("test1");
	}

	public void test2()
	{
		LogMgr.Log("test2");
	}

	public void test3()
	{
		LogMgr.Log("test3");
	}
}



public class Manager : MonoBehaviour {
	
	KTcpClient client;
	KTcpServer server ;
	Thread th;



	// Use this for initialization
	void Start ()
	{


//		http= new HttpClient();
//		http.BeginPost("http://192.168.1.5:7070/MonsterMaster/account/checkAccount.action",delegate(string obj)
//		{
//			LogMgr.Log("===1 result  = "+ obj);
//		});
//		http.AddField("params",ParseUtils.Json_Serialize(new TestJson2()));
//
//		http.StartConnect();
//
//		http.BeginPost("http://192.168.1.5:7070/MonsterMaster/account/checkAccount.action",delegate(string obj)
//		               {
//			LogMgr.Log("===2 result  = "+ obj);
//		});
//		http.AddField("params",ParseUtils.Json_Serialize(new TestJson2()));
//		
//		http.StartConnect();
//
//		http.BeginPost("http://192.168.1.5:7070/MonsterMaster/account/checkAccount.action",delegate(string obj)
//		               {
//			LogMgr.Log("===3 result  = "+ obj);
//		});
//		http.AddField("params",ParseUtils.Json_Serialize(new TestJson2()));
//		
//		http.StartConnect();
//
//		http.BeginPost("http://192.168.1.5:7070/MonsterMaster/account/checkAccount.action",delegate(string obj)
//		               {
//			LogMgr.Log("===4 result  = "+ obj);
//		});
//		http.AddField("params",ParseUtils.Json_Serialize(new TestJson2()));
//		
//		http.StartConnect();
		//
//		http.BeginPost("http://192.168.1.5:7070/MonsterMaster/account/checkAccount.action",delegate(string obj)
//		               {
//			LogMgr.Log("result  = "+ obj);
//		});
//		http.AddField("params",ParseUtils.Json_Serialize(new TestJson2()));
//		
//		http.StartConnect();


//		var b = new ByteBuffer();
//		b += new Byte[2]{1,2};
//		b +=2;
//		b +=5.2134f;
//		b +=(short)7;
//		b +=(double)9.872236;
//		b += "cjsjy123";
//
//
//		var v1 = (ushort) b;
//		var v2 =(UInt32)b;
//		var v3 = (float)b;
//		var v4 = (short)b;
//		var v5 =(double)b;
//		var v6 = (string)b;




		th = new Thread(testTh);th.Start();


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


	bool isDone =false;
	float value =0;
	HttpClient http;
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
		if(th != null)
			th.Abort();
		if(client != null)
		{
			client.quit = true;
			client.Close();
		}


//		server.Close();
//		client.CloseConnect();
	}



//	public  IEnumerator listenRec(KTcpServer server)
//	{
//
//		LogMgr.Log("listenRec   ");
//		server._socket.Get().BeginAccept(new AsyncCallback(KTcpServer.AcceptCallback),server._socket);
//		yield return new WaitForSeconds(3f);
//
//	}
//
//	IEnumerator ThreadSend()
//	{
//		while(Application.isPlaying)
//		{
//
//			LogMgr.Log("start send");
//			KTcpClient.Send(client._socket,"Clienttest !SSSS ");
//			yield return new WaitForSeconds(8f);
//			KTcpServer.Send(server._socket,"Server !!!! gggggg");
//			yield return new WaitForSeconds(3f);
//		}
//
//		yield return null;
//	}
	

}
