//#define IN_UNITY

using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if IN_UNITY


#else
using System.Net;
using System.Threading;

#endif

namespace Kubility
{


	public class HttpUnit :ConnectEvents
	{

		enum HttpType
		{
			FREE,
			POST,
			GET,
			DOWNLOADFILE,
		}

		Stack<ClsTuple< string,HttpType,object> > requestList;

		KThread m_thread;

		ClsTuple<string,HttpType,object> m_curRequest;

#if IN_UNITY
		Task m_task;
		WWWForm m_form;
		Stream m_stream;
		public readonly bool Unity =true;

#else

		public readonly bool Unity =false;

				ClsTuple< FileStream,string, long,byte,int,object> file ;

		StringBuilder m_requestSR ;
		/// <summary>
		/// fieldNums   timeout
		/// </summary>
				ClsTuple<short,int> m_varUtils ;

#endif

		#region interface
		VoidDelegate _ConnectCloseEvent =null ;
		VoidDelegate _ConnectFailedEvent =null;
		VoidDelegate _TimeOutEvent =null;
		VoidDelegate _OthersErrorEvent =null;
		Action<string> _SuccessEvent =null;
		
		Action<string,float,bool> _onProcess ;
		
		public VoidDelegate m_ConnectCloseEvent 
		{
			get
			{
				return _ConnectCloseEvent;
			}
		}
		public VoidDelegate m_ConnectFailedEvent
		{
			get
			{
				return _ConnectFailedEvent;
			}
		}
		public VoidDelegate m_TimeOutEvent
		{
			get
			{
				return _TimeOutEvent;
			}
		}
		public VoidDelegate m_OthersErrorEvent
		{
			get
			{
				return _OthersErrorEvent;
			}
		}
		public Action<string> m_SuccessEvent
		{
			get
			{
				return _SuccessEvent;
			}
			set
			{
				_SuccessEvent = value;
			}
		}
		
		public Action<string,float,bool> onProcess 
		{
			get
			{
				return _onProcess;
			}
			set
			{
				_onProcess = value;
			}
		}
		
		public void UnRegisterAll()
		{
			_ConnectCloseEvent = null;
			_ConnectFailedEvent = null;
			_TimeOutEvent = null;
			_SuccessEvent = null;
			_onProcess = null;
			_OthersErrorEvent = null;

		}

		#endregion

		public HttpUnit()
		{

			requestList = new Stack<ClsTuple<string, HttpType, object>> ();
#if IN_UNITY
#else
			this.m_curRequest = new ClsTuple<string,HttpType,object>();
			this.file = new ClsTuple< FileStream,string, long,byte,int,object>();

			this.file.field2 = 0;
			this.file.field3 = 0;
			this.file.field4 =Config.mIns.HttpSpeedLimit;

			this.m_requestSR = new StringBuilder(1024);

			this.m_varUtils = new ClsTuple<short, int>(0,0);
			this.m_varUtils.field0 = 0;
			this.m_varUtils.field1 = 20000;
#endif
		}


		public void BeginPost(string URL,Action<string> callback, bool AutoStart =false)
		{

			m_curRequest.field0 = URL;
			m_curRequest.field1 = HttpType.POST;
			m_SuccessEvent = callback;
#if IN_UNITY

			m_task = new Task(UnityConnect(callback),AutoStart);
#else
			m_varUtils.field0 =0;
			m_thread = KThread.StartTask(HttpThread,false);
#endif


		}
		
		public void BeginGet(string URL,Action<string> callback, bool AutoStart =true)
		{

			m_curRequest.field0 = URL;
			m_curRequest.field1 = HttpType.GET;
			this.m_SuccessEvent = callback;
#if IN_UNITY

			m_task = new Task(UnityConnect(callback),AutoStart);
#else
			m_varUtils.field0 = 0;
			m_thread = KThread.StartTask(HttpThread,false);
				
#endif

		}


		public void BeginDownLoadFileFlushToFile(string URL, string Filepath,Action<string,float,bool> callback,bool AutoStart=false)
		{
			m_curRequest.field0 = URL;
			m_curRequest.field1 = HttpType.DOWNLOADFILE;
#if IN_UNITY

			m_task = new Task(UnityConnect(callback),AutoStart);
#else

			this.onProcess = callback;

			if(file.field3 ==1 )
			{
				file.field3 = 0;
			}

			try
			{
				file.field0 = new FileStream(Filepath,FileMode.OpenOrCreate);
				file.field2 = file.field0.Length;//cur len
				
			}
			catch(Exception ex)
			{
				LogMgr.LogError(ex);
				if(m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent();
				}
			}


			m_curRequest.field2 = file;
			
			m_thread = KThread.StartTask(ThreadDownLoad,false);

#endif



		}




#if IN_UNITY
		IEnumerator UnityConnect(object obj)
		{

			var m_state =m_curRequest.field1;

			WWW www = GetForm(m_state == HttpType.POST);
			yield return www;
			
			if(www.error == null)
			{
				if(m_state == HttpType.GET || m_state == HttpType.POST)
				{

					Action<string> callback = (Action<string>)obj;
					if(callback != null)
						callback(www.text);
				}
				else if( m_state == HttpType.DOWNLOADFILE )
				{

					m_stream = new MemoryStream(www.bytes);

					MonoDelegate.mIns.Coroutine_Delay(30,delegate(){
						m_stream.Close();
						m_stream = null;
					});
				}

			}
			else
			{
				LogMgr.LogError("WWW : "+m_curRequest.field0 +"  error!  = "+www.error);
			}
			m_state = HttpType.FREE;
			www.Dispose();
		}
		
#endif
			
		public void StartConnect()
		{
#if IN_UNITY

			if(!m_task.Running)
			{
				m_task.Start();
			}
			else
			{
				LogMgr.Log("Http Coroutine is Runing");
			}
#else
			try
			{


				var m_state = m_curRequest.field1;
				if(m_state == HttpType.GET || m_state == HttpType.POST )
				{

					var value = new ClsTuple<string,System.Action<string>>();
					value.field0 =m_requestSR.ToString();
					value.field1 = m_SuccessEvent;
					m_curRequest.field2 = value;

					m_requestSR.Length =0;
					requestList.Push(m_curRequest);

					m_thread.Start();
				}
				else if(m_state == HttpType.DOWNLOADFILE)
				{
					file.field5 = onProcess;
					m_curRequest.field2 = file;
					requestList.Push(m_curRequest);
					m_thread.Start();
				}

			}
			catch(Exception ex)
			{
				LogMgr.LogError(ex);
			}

#endif
		}

#if IN_UNITY
#else
		void HttpThread()
		{
			ClsTuple<string,HttpType,object> curRequest = requestList.Pop();
			ClsTuple<string,System.Action<string>> temp = (ClsTuple<string,System.Action<string>>) curRequest.field2;
			string strRequest =temp.field0;
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(curRequest.field0);
				if(curRequest.field1 == HttpType.POST)
				{

					req.Timeout = 20000;
								
					req.Method = "POST";
				}
				else if(curRequest.field1 == HttpType.GET)
				{

					req.Timeout = 20000;
					req.ContentType ="application/x-www-form-urlencoded";
					req.Method = "GET";
				}


				byte[] btBodys = System.Text.Encoding.UTF8.GetBytes(strRequest);
				req.ContentLength = btBodys.Length;
				req.GetRequestStream().Write(btBodys,0, btBodys.Length);
				LogMgr.Log("resquest  = "+strRequest);

				var Response = (HttpWebResponse)req.GetResponse();
				var sr = new StreamReader(Response.GetResponseStream());
				
				string responseContent = sr.ReadToEnd();
				
				if(temp.field1 != null)
				{
					temp.field1(responseContent);
				}
				
				req.Abort();
				Response.Close();
				sr.Close();

			}
			catch (WebException  webEx)
			{
				DealWithEx(webEx);

			}
			

		}
#endif

#if IN_UNITY
#else


		
		void ThreadDownLoad()
		{
			var curRequest = requestList.Pop();

			var file =(ClsTuple< FileStream,string, long,byte,int,object>)curRequest.field2;
			Action<string,float,bool> callback = (Action<string,float,bool>)file.field5;
			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(curRequest.field0);
				request.AddRange((int)file.field2);
				
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();
				long totalSize = response.ContentLength;

				//如果返回的response头中Content-Range值为空，说明服务器不支持Range属性，不支持断点续传,返回的是所有数据
				if (response.Headers["Content-Range"] == null)
				{
					Interlocked.Exchange(ref file.field2,0);
					
				}
//				LogMgr.Log("Content-Range = + " +response.Headers["Content-Range"] );

				while(  file.field3 == 0)
				{
					if(file.field0 != null && file.field2 >0)
					{
						file.field0.Seek(file.field2,SeekOrigin.Begin);
					}


					if (response != null)
					{

						try
						{
							BufferedStream bs = new BufferedStream(stream);
							byte[] bys = new byte[file.field4];
							int readLen = bs.Read(bys,0,bys.Length);
							int maxLen =readLen;
							while (readLen  >0)
							{
//								DownLoadEv.WaitOne();
								if(file.field0 != null)
								{
									file.field0.Write(bys,0,readLen);
								}

								string newstr = file.field1 +Encoding.UTF8.GetString(bys);
								Interlocked.Exchange(ref file.field1 ,newstr);

								Interlocked.Add(ref file.field2,readLen);

								if(callback != null)
								{
									bool bvalue  = file.field3 == 1;
									float value =(float)file.field2/(float)totalSize;
									callback(file.field1,value,bvalue);
								}

								readLen =bs.Read(bys,0,bys.Length);
								maxLen = Math.Max(readLen,maxLen);
							}


							file.field3 = 1;

						}
						catch(Exception ex)
						{
							LogMgr.Log("Exception  = "+ ex);
							file.field3 = 1;
						}
					}
					else
					{
						if(m_OthersErrorEvent != null)
						{
							m_OthersErrorEvent();
						}
						break;
					}
				}

				response.Close();
				request.Abort();
				response.Close();
				stream.Close();
				DownloadAbort();
	
			}
			catch(WebException ex)
			{
				DealWithEx(ex);
				DownloadAbort();

			}
			LogMgr.Log("DOWNLOADFILE ISDONE");

		}

#endif
		public void AddField(string field,string content)
		{
#if IN_UNITY
			if(m_form  == null)
			{
				m_form = new WWWForm();
			}
			m_form.AddField(field,content);
#else 
			if(m_varUtils.field0 == 0)
			{
				m_requestSR.AppendFormat("{0}={1}",field,content);
				m_varUtils.field0++;

			}
			else if(m_varUtils.field0 >0)
			{

				m_requestSR.AppendFormat("&{0}={1}",field,content);

				m_varUtils.field0++;
			}

#endif
		}
		
		public void AddField(string field,int content)
		{
#if IN_UNITY
			if(m_form  == null)
			{
				m_form = new WWWForm();
			}
			m_form.AddField(field,content);
#else
			if(m_varUtils.field0 == 0)
			{
				m_requestSR.AppendFormat("{0}={1}",field,content.ToString());
				m_varUtils.field0++;
				
			}
			else if(m_varUtils.field0 >0)
			{
				
				m_requestSR.AppendFormat("&{0}={1}",field,content.ToString());
				
				m_varUtils.field0++;
			}
#endif
		}

#if IN_UNITY		
		public void AddBinaryData(string field,byte[] content)
		{

			if(m_form  == null)
			{
				m_form = new WWWForm();
			}
			m_form.AddBinaryData(field,content);

		}


#endif		
		
			
#region helper
		public KThread getThread()
		{
			return this.m_thread;
		}

		public void PopRequest()
		{
			if(this.requestList.Count >0)
			{
				this.requestList.Pop();
			}
		}


		public void Close()
		{

			KThread.CloseAll();

			foreach(var sub in requestList)
			{
			  var subf =	(ClsTuple< FileStream,string, long,byte,int,object>)sub.field2;
				if(subf.field0 != null)
				{
					subf.field0.Close();
				}
			}
		}


#if IN_UNITY
		public Stream GetStream()
		{
			return m_stream;
		}

		WWW GetForm(bool isPost)
		{
			WWW  www;
			if(isPost)
			{
				if(m_form == null)
				{

					LogMgr.LogError("Form is Null May Cause Erros while Use Http Post!");
				}
				www = new WWW(m_curRequest.field0,m_form);
			}
			else
			{
				www = new WWW(m_curRequest.field0);
			}
			
			
			return www;
		}

#else

		void DownloadAbort()
		{
			if(onProcess != null)
			{
				bool bvalue  = file.field3 == 1;
				onProcess(file.field1,1,bvalue);
			}
			
			if(file.field0 != null)
			{
				file.field0.Flush();
				file.field0.Close();
				file.field0 = null;
			}
			file.field0 = null;
			file.field1="";
			file.field2=0;

		}


		void DealWithEx(WebException webEx)
		{
			LogMgr.Log("WebException  = "+ webEx);
			if (webEx.Status == WebExceptionStatus.Timeout)
			{
				if(m_TimeOutEvent != null)
				{
					m_TimeOutEvent();
				}
			}
			else if(webEx.Status == WebExceptionStatus.ConnectionClosed)
			{
				if(m_ConnectCloseEvent != null)
				{
					m_ConnectCloseEvent();
				}
			}
			else if(webEx.Status == WebExceptionStatus.ConnectFailure)
			{
				if(m_ConnectFailedEvent != null)
				{
					m_ConnectFailedEvent();
				}
			}
			else if(webEx.Status !=  WebExceptionStatus.Success)
			{
				if(m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent();
				}
			}
		}

		int GetFrom()
		{
			long value = file.field2 ;
			if(value <0)
			{
				return 0;
			}
			else
			{
				return (int)value;
			}
		}

		int GetEnd(long totalSize)
		{
			long value = file.field2 + file.field4 -1;
			if(value >= totalSize)
			{
				return (int)(totalSize-1);
			}
			else
				return (int)value;
		}

		public void SetDownLoadSpeed(int speed)
		{
			file.field4 =speed;
		}
		
		public int GetDownloadSpeed()
		{
			return file.field4;
		}
		
		public void SetOutTime(int time)
		{
			m_varUtils.field1 = time;
		}
		
		public int GetOutTime()
		{
			return m_varUtils.field1;
		}

#endif
#endregion
		
		
		
	}

}
