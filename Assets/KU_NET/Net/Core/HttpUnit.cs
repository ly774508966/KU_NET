//#define USE_COR

using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if USE_COR
#else
using System.Net;
using System.Threading;

#endif

namespace Kubility
{


    public class HttpUnit : ConnectEvents
    {

        enum HttpType
        {
            FREE,
            POST,
            GET,
            DOWNLOADFILE,
			DOWNLOADFILE_TOMEMORY,
        }

        Stack<MiniTuple<string, HttpType, object>> requestList;

        KThread m_thread;

		MiniTuple<string, HttpType, object> m_curRequest;

#if USE_COR
		Task m_task;
		WWWForm m_form;
		Stream m_stream;
		public readonly bool Unity = true;


#else

        public readonly bool Unity = false;

		MiniTuple<string, int, object> file;

        StringBuilder m_requestSR;
        /// <summary>
        /// fieldNums   timeout
        /// </summary>
        MiniTuple<short, int> m_varUtils;

		object m_lock ;

#endif

        #region interface

        VoidDelegate _ConnectCloseEvent = null;
        VoidDelegate _ConnectFailedEvent = null;
        VoidDelegate _TimeOutEvent = null;
        ExceptionDelegate _OthersErrorEvent = null;
        Action<string> _SuccessEvent = null;

        Action<byte[], float, bool> _onProcess;

        public VoidDelegate m_ConnectCloseEvent
        {
            get
            {
                return _ConnectCloseEvent;
            }
            set
            {
                _ConnectCloseEvent = value;
            }
        }

        public VoidDelegate m_ConnectFailedEvent
        {
            get
            {
                return _ConnectFailedEvent;
            }
            set
            {
                _ConnectFailedEvent = value;
            }
        }

        public VoidDelegate m_TimeOutEvent
        {
            get
            {
                return _TimeOutEvent;
            }
            set
            {
                _TimeOutEvent = value;
            }
        }

        public ExceptionDelegate m_OthersErrorEvent
        {
            get
            {
                return _OthersErrorEvent;
            }
            set
            {
                _OthersErrorEvent = value;
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

        public Action<byte[], float, bool> onProcess
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

			requestList = new Stack<MiniTuple<string, HttpType, object>>();
			this.m_curRequest = new MiniTuple<string, HttpType, object>();
			ErrorManager.mIns.Register<HttpUnit>(this);
#if USE_COR
#else
			this.m_lock = new object();
			this.file = new MiniTuple<string, int, object>();

            this.file.field1 = Config.mIns.HttpSpeedLimit;

            this.m_requestSR = new StringBuilder(1024);

            this.m_varUtils = new MiniTuple<short, int>();
            this.m_varUtils.field0 = 0;
            this.m_varUtils.field1 = Config.mIns.HttpSpeedLimit;
#endif
        }


        public void BeginPost(string URL, Action<string> callback, bool AutoStart = false)
        {

            m_curRequest.field0 = URL;
            m_curRequest.field1 = HttpType.POST;
            m_SuccessEvent = callback;
#if USE_COR
			requestList.Push(m_curRequest);
			m_task = new Task (UnityConnect (callback), AutoStart);
#else
            m_varUtils.field0 = 0;
            m_thread = KThread.StartTask(HttpThread, false);
#endif


        }

        public void BeginGet(string URL, Action<string> callback, bool AutoStart = true)
        {

            m_curRequest.field0 = URL;
            m_curRequest.field1 = HttpType.GET;
            this.m_SuccessEvent = callback;
#if USE_COR
			requestList.Push(m_curRequest);
            m_task = new Task (UnityConnect (callback), AutoStart);
#else
            m_varUtils.field0 = 0;
            m_thread = KThread.StartTask(HttpThread, false);

#endif

        }

		public void BeginDownLoadFileFlushToMemory(string URL,  Action<byte[], float, bool> callback, bool AutoStart = false)
		{
			m_curRequest.field0 = URL;
			m_curRequest.field1 = HttpType.DOWNLOADFILE_TOMEMORY;
#if USE_COR
			requestList.Push(m_curRequest);
            m_task = new Task (UnityConnect (callback), AutoStart);
#else
			
			this.onProcess = callback;

			m_curRequest.field2 = file;
			
			m_thread = KThread.StartTask(ThreadDownLoad, false);
			
			#endif
			
		}


        public void BeginDownLoadFileFlushToFile(string URL, string Filepath, Action<byte[], float, bool> callback, bool AutoStart = false)
        {
            m_curRequest.field0 = URL;
            m_curRequest.field1 = HttpType.DOWNLOADFILE;
#if USE_COR
			m_curRequest.field2 = Filepath;
			requestList.Push(m_curRequest);
            m_task = new Task (UnityConnect (callback), AutoStart);
#else

            this.onProcess = callback;

            file.field0 = Filepath;
            m_curRequest.field2 = file;

            m_thread = KThread.StartTask(ThreadDownLoad, false);

#endif

        }

#if USE_COR
		IEnumerator UnityConnect (object obj)
		{
			if(requestList.Count == 0)
			{
				yield break;
			}

			var request =this.requestList.Pop();
			HttpType m_state = request.field1;

			WWW www = GetForm (request);
			yield return www;
			
			if (www.error == null) 
			{
				if (m_state == HttpType.GET || m_state == HttpType.POST)
				{
					Action<string> callback = (Action<string>)obj;
					if (callback != null)
						callback (www.text);
				} 
				else if (m_state == HttpType.DOWNLOADFILE || m_state == HttpType.DOWNLOADFILE_TOMEMORY)
				{

					Action<byte[], float, bool> callback = (Action<byte[], float, bool>) obj ;
					if(callback != null)
					{
						callback(www.bytes,1f,true);
					}


					if(m_state == HttpType.DOWNLOADFILE)
					{
						var data = request.field2 as string;

						FileStream fs = new FileStream(data,FileMode.OpenOrCreate,FileAccess.ReadWrite);
						fs.Write(www.bytes,0,www.bytes.Length);
						fs.Close();
                    }

				}

			} 
			else
			{
				if(m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent(new CustomException(www.error,ErrorType.NetError));
				}
			}
			m_state = HttpType.FREE;
			www.Dispose ();
			}
		
#endif

        public void StartConnect()
        {
#if USE_COR

			if (!m_task.Running) 
			{

				m_task.Start ();
			}
			else
			{
				LogMgr.Log ("Http Coroutine is Runing");
			}
#else
            try
            {


                HttpType m_state = m_curRequest.field1;
                if (m_state == HttpType.GET || m_state == HttpType.POST)
                {

					MiniTuple<string, System.Action<string>> value = new MiniTuple<string, System.Action<string>>();
                    value.field0 = m_requestSR.ToString();
                    value.field1 = m_SuccessEvent;
                    m_curRequest.field2 = value;

                    m_requestSR.Length = 0;
                    requestList.Push(m_curRequest);

                    m_thread.Start();
                }
                else if (m_state == HttpType.DOWNLOADFILE || m_state == HttpType.DOWNLOADFILE_TOMEMORY)
                {
                    file.field2 = onProcess;
                    m_curRequest.field2 = file;
                    requestList.Push(m_curRequest);
                    m_thread.Start();
                }

            }
            catch (Exception ex)
            {
				if(m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent(ex);
				}
            }

#endif
        }

#if USE_COR
#else
        void HttpThread()
        {
			MiniTuple<string, HttpType, object> curRequest = new MiniTuple<string, HttpType, object>();
			bool isEmpty =false;
			lock(m_lock)
			{
				if(requestList.Count >0)
				{
					curRequest  =requestList.Pop();
				}
				else
					isEmpty =true;
			}
			if(isEmpty)
				return;

			MiniTuple<string, System.Action<string>> temp = (MiniTuple<string, System.Action<string>>)curRequest.field2;
            string strRequest = temp.field0;
			HttpWebRequest req = null;
			HttpWebResponse Response = null;
            try
            {
				req = (HttpWebRequest)WebRequest.Create(curRequest.field0);
				if (curRequest.field1 == HttpType.POST)
				{
					
					req.Timeout = Config.mIns.Http_TimeOut;
					req.Method = "POST";
				}
				else if (curRequest.field1 == HttpType.GET)
				{
					
					req.Timeout = Config.mIns.Http_TimeOut; 
					req.ContentType = "application/x-www-form-urlencoded";
					req.Method = "GET";
				}
				
				byte[] btBodys = System.Text.Encoding.UTF8.GetBytes(strRequest);
				req.ContentLength = btBodys.Length;
				using (Stream respSb = req.GetRequestStream())
				{
					respSb.Write(btBodys, 0, btBodys.Length);
				}


				using ( Response = (HttpWebResponse)req.GetResponse())
				{
					var sr = new StreamReader(Response.GetResponseStream());
					
					string responseContent = sr.ReadToEnd();
					
					if (temp.field1 != null)
					{
						temp.field1(responseContent);
					}
					
					req.Abort();
					sr.Close();
				}
            }
            catch (Exception webEx)
            {

                DealWithEx(webEx);

            }
			finally
			{
				if(req != null)
				{
					req.Abort();
				}
				
				if(Response != null)
				{
					Response.Close();
				}

			}


        }
#endif

#if USE_COR
#else
        void ThreadDownLoad()
        {
			MiniTuple<string, HttpType, object> curRequest = new MiniTuple<string, HttpType, object>();

			bool isEmpty =false;
			lock(m_lock)
			{
				if(requestList.Count >0)
				{
					curRequest  =requestList.Pop();
				}
				else
					isEmpty =true;
			}
			if(isEmpty)
				return;

			HttpWebRequest request = null;
			HttpWebResponse response = null;
			HttpType curHttpType = curRequest.field1;
            //path,flag,limitspeed,ACTION
			var req = (MiniTuple<string, int, object>)curRequest.field2;
			Stream DataStream = null ;
			try
			{

				long oldSize =0;
				if(curHttpType == HttpType.DOWNLOADFILE_TOMEMORY)
				{
					DataStream = new MemoryStream();
				}
				else if (curHttpType == HttpType.DOWNLOADFILE)
				{
					DataStream = new FileStream(req.field0, FileMode.OpenOrCreate, FileAccess.ReadWrite);
					oldSize= DataStream.Length;
				}
				else
				{
					LogMgr.LogError("Http Download State Error");
					return;
				}

				Action<byte[], float, bool> callback = (Action<byte[], float, bool>)req.field2;
				//req
				
				request= (HttpWebRequest)HttpWebRequest.Create(curRequest.field0);
				request.AddRange("bytes",(int)oldSize);
				request.Timeout = Config.mIns.Http_TimeOut;
				request.ReadWriteTimeout = 20 * 1000;
				request.KeepAlive = false;
				request.AllowWriteStreamBuffering = false;
				request.AllowAutoRedirect = true;
				request.AutomaticDecompression = DecompressionMethods.None;
                
                response = (HttpWebResponse)request.GetResponse();

				if (response.Headers["Content-Range"] == null)
				{
					oldSize = 0;
				}
				//long to float   may cause error
				long totalSize = response.ContentLength + oldSize;

				using ( Stream stream = response.GetResponseStream())
				{
					if(curHttpType == HttpType.DOWNLOADFILE)
					{
						if (DataStream != null && oldSize > 0)
						{
							DataStream.Seek(0, SeekOrigin.End);
						}
						
                        Read(DataStream,stream,req,oldSize,response.ContentLength,callback);
					}
					else if(curHttpType == HttpType.DOWNLOADFILE_TOMEMORY)
					{
						
						Read(DataStream,stream,req,oldSize,response.ContentLength,delegate(byte[] arg1, float arg2, bool arg3) {
							if(!arg3)
							{
								callback(arg1,arg2,arg3);
							}
							else
							{
								byte[] totalBys = new byte[DataStream.Length];
								DataStream.Read(totalBys,0,totalBys.Length);
								callback(totalBys,(float)totalBys.Length /(float)totalSize,arg3);
							}
						});
						
					}
				}

				if(DataStream != null)
					DataStream.Close();

				System.GC.Collect();
				
				
			}
			catch (WebException ex)
			{
				DealWithEx(ex);
				
			}
			finally
			{

				if(request != null)
				{
					request.Abort();
				}

				if(response != null)
				{
					response.Close();
				}

				if(DataStream != null)
				{
					DataStream.Close();
				}

				System.GC.Collect();
			}

        }

		void Read(Stream DataIntStream,Stream DataOutStream,MiniTuple<string, int, object> req,long oldSize,long totalSize,Action<byte[], float, bool> callback)
		{
			bool bvalue = false;
			try
			{
                
	            while (!bvalue)
				{
					byte[] bys = new byte[req.field1];
					int readLen = DataOutStream.Read(bys, 0, bys.Length);
					int maxLen = readLen;
					int total = readLen;
					while (readLen > 0  )
					{
							
						if (DataIntStream != null)
						{
							DataIntStream.Write(bys, 0, readLen);
						}
						bvalue = total == totalSize;
						if (callback != null)
						{
							float value = (float)(oldSize + total) / (float)(totalSize+ oldSize);
							callback(bys, value, bvalue);
						}
						readLen = DataOutStream.Read(bys, 0, bys.Length);
							
						total += readLen;

                        maxLen = Math.Max(readLen, maxLen);
					}
	
				}
			}

			catch (Exception ex)
			{
				if (m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent(ex);
				}
				if (callback != null)
                {
                    callback(null, 0, true);
                }
            }
            
        }
        
        #endif
        public void AddField(string field, string content)
        {
#if USE_COR
			if (m_form == null)
			{
				m_form = new WWWForm ();
			}
			m_form.AddField (field, content);
#else
            if (m_varUtils.field0 == 0)
            {
                m_requestSR.AppendFormat("{0}={1}", field, content);
                m_varUtils.field0++;

            }
            else if (m_varUtils.field0 > 0)
            {

                m_requestSR.AppendFormat("&{0}={1}", field, content);

                m_varUtils.field0++;
            }

#endif
        }

        public void AddField(string field, int content)
        {
#if USE_COR
			if (m_form == null)
			{
				m_form = new WWWForm ();
			}
			m_form.AddField (field, content);
#else
            if (m_varUtils.field0 == 0)
            {
                m_requestSR.AppendFormat("{0}={1}", field, content.ToString());
                m_varUtils.field0++;

            }
            else if (m_varUtils.field0 > 0)
            {

                m_requestSR.AppendFormat("&{0}={1}", field, content.ToString());

                m_varUtils.field0++;
            }
#endif
        }

#if USE_COR
		public void AddBinaryData (string field, byte[] content)
		{

			if (m_form == null)
			{
				m_form = new WWWForm ();
			}
			m_form.AddBinaryData (field, content);
		}


#endif

        #region helper

        public KThread getThread()
        {
            return this.m_thread;
        }

        public void PopRequest()
        {
            if (this.requestList.Count > 0)
            {
                this.requestList.Pop();
            }
        }


        public void Close()
        {

            KThread.CloseAll();
            requestList.Clear();
			UnRegisterAll();

        }


#if USE_COR
		public Stream GetStream ()
		{
			return m_stream;
		}

		WWW GetForm (MiniTuple<string, HttpType, object> req)
		{
			WWW www;
			if (req.field1 == HttpType.POST) {
				if (m_form == null)
				{
					LogMgr.LogError ("Form is Null May Cause Erros while Use Http Post!");
				}
				www = new WWW (req.field0, m_form);
			} 
			else 
			{
				www = new WWW (req.field0);
			}

			return www;
		}

#else

        void DealWithEx(Exception Ex)
        {
            WebException webEx = Ex as WebException;
            if (webEx != null)
            {

                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    if (m_TimeOutEvent != null)
                    {
                        m_TimeOutEvent();
                    }
                }
                else if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                {
                    if (m_ConnectCloseEvent != null)
                    {
                        m_ConnectCloseEvent();
                    }
                }
                else if (webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    if (m_ConnectFailedEvent != null)
                    {
                        m_ConnectFailedEvent();
                    }
                }
                else if (webEx.Status != WebExceptionStatus.Success)
                {
                    if (m_OthersErrorEvent != null)
                    {
                        m_OthersErrorEvent(webEx);
                    }
                }
            }
			else
			{
				if (m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent(Ex);
				}
			}
            //						LogMgr.Log ("Exception  = " + webEx);

        }


        public void SetDownLoadSpeed(int speed)
        {
            file.field1 = speed;
        }

        public int GetDownloadSpeed()
        {
            return file.field1;
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
