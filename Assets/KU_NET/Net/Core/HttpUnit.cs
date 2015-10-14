#define USE_COR

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
        }

        Stack<ClsTuple<string, HttpType, object>> requestList;

        KThread m_thread;

        ClsTuple<string, HttpType, object> m_curRequest;

#if USE_COR
		Task m_task;
		WWWForm m_form;
		Stream m_stream;
		public readonly bool Unity = true;


#else

        public readonly bool Unity = false;

        ClsTuple<string, int, object> file;

        StringBuilder m_requestSR;
        /// <summary>
        /// fieldNums   timeout
        /// </summary>
        MiniTuple<short, int> m_varUtils;

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

            requestList = new Stack<ClsTuple<string, HttpType, object>>();
            this.m_curRequest = new ClsTuple<string, HttpType, object>();
#if USE_COR
#else

            this.file = new ClsTuple<string, int, object>();

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

			m_task = new Task (UnityConnect (callback), AutoStart);
#else
            m_varUtils.field0 = 0;
            m_thread = KThread.StartTask(HttpThread, false);

#endif

        }


        public void BeginDownLoadFileFlushToFile(string URL, string Filepath, Action<byte[], float, bool> callback, bool AutoStart = false)
        {
            m_curRequest.field0 = URL;
            m_curRequest.field1 = HttpType.DOWNLOADFILE;
#if USE_COR

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

			var m_state = m_curRequest.field1;

			WWW www = GetForm (m_state == HttpType.POST);
			yield return www;
			
			if (www.error == null) 
			{
				if (m_state == HttpType.GET || m_state == HttpType.POST)
				{
					Action<string> callback = (Action<string>)obj;
					if (callback != null)
						callback (www.text);
				} 
				else if (m_state == HttpType.DOWNLOADFILE)
				{
					m_stream = new MemoryStream (www.bytes);
					MonoDelegate.mIns.Coroutine_Delay (30, delegate() {
												m_stream.Close ();
												m_stream = null;
										});
				}

			} 
			else
			{
				if(m_OthersErrorEvent != null)
				{
					m_OthersErrorEvent(new CustomException("www error",ErrorType.NetError));
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


                var m_state = m_curRequest.field1;
                if (m_state == HttpType.GET || m_state == HttpType.POST)
                {

                    var value = new ClsTuple<string, System.Action<string>>();
                    value.field0 = m_requestSR.ToString();
                    value.field1 = m_SuccessEvent;
                    m_curRequest.field2 = value;

                    m_requestSR.Length = 0;
                    requestList.Push(m_curRequest);

                    m_thread.Start();
                }
                else if (m_state == HttpType.DOWNLOADFILE)
                {
                    file.field2 = onProcess;
                    m_curRequest.field2 = file;
                    requestList.Push(m_curRequest);
                    m_thread.Start();
                }

            }
            catch (Exception ex)
            {
                LogMgr.LogError(ex);
            }

#endif
        }

#if USE_COR
#else
        void HttpThread()
        {
            ClsTuple<string, HttpType, object> curRequest = requestList.Pop();
            ClsTuple<string, System.Action<string>> temp = (ClsTuple<string, System.Action<string>>)curRequest.field2;
            string strRequest = temp.field0;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(curRequest.field0);
                if (curRequest.field1 == HttpType.POST)
                {

                    req.Timeout = Config.mIns.Http_TimeOut;
                    req.Method = "POST";
                }
                else if (curRequest.field1 == HttpType.GET)
                {

                    req.Timeout = Config.mIns.Http_TimeOut; req.Timeout = 20000;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Method = "GET";
                }


                byte[] btBodys = System.Text.Encoding.UTF8.GetBytes(strRequest);
                req.ContentLength = btBodys.Length;
                req.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                //LogMgr.Log("resquest  = " + strRequest);

                var Response = (HttpWebResponse)req.GetResponse();
                var sr = new StreamReader(Response.GetResponseStream());

                string responseContent = sr.ReadToEnd();

                if (temp.field1 != null)
                {
                    temp.field1(responseContent);
                }

                req.Abort();
                Response.Close();
                sr.Close();

            }
            catch (WebException webEx)
            {
                DealWithEx(webEx);

            }


        }
#endif

#if USE_COR
#else
        void ThreadDownLoad()
        {
            var curRequest = requestList.Pop();
            //path,flag,limitspeed,ACTION
            using (var req = (ClsTuple<string, int, object>)curRequest.field2)
            {
                try
                {
                    var fsstream = new FileStream(req.field0, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    long oldSize = fsstream.Length;
                    Action<byte[], float, bool> callback = (Action<byte[], float, bool>)req.field2;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(curRequest.field0);
                    request.AddRange((int)oldSize);
                    request.Timeout = Config.mIns.Http_TimeOut;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();


                    //如果返回的response头中Content-Range值为空，说明服务器不支持Range属性，不支持断点续传,返回的是所有数据
                    if (response.Headers["Content-Range"] == null)
                    {
                        oldSize = 0;

                    }
                    long totalSize = response.ContentLength + oldSize;
                    bool bvalue = false;

                    while (!bvalue)
                    {
                        if (fsstream != null && oldSize > 0)
                        {
                            fsstream.Seek(0, SeekOrigin.End);
                        }


                        if (response != null)
                        {

                            try
                            {
                                BufferedStream bs = new BufferedStream(stream);
                                byte[] bys = new byte[req.field1];
                                int readLen = bs.Read(bys, 0, bys.Length);
                                int maxLen = readLen;
                                int total = readLen;
                                while (readLen > 0)
                                {

                                    if (fsstream != null)
                                    {
                                        fsstream.Write(bys, 0, readLen);
                                    }
                                    bvalue = total == response.ContentLength;

                                    if (callback != null)
                                    {

                                        float value = (float)(oldSize + total) / (float)totalSize;
                                        callback(bys, value, bvalue);
                                    }

                                    readLen = bs.Read(bys, 0, bys.Length);

                                    total += readLen;
                                    maxLen = Math.Max(readLen, maxLen);
                                }

                                LogMgr.LogError("total =" + total.ToString() + " maxSpeed =" + maxLen.ToString());


                            }
                            catch (Exception ex)
                            {
                                if (m_OthersErrorEvent != null)
                                {
                                    m_OthersErrorEvent(ex);
                                }

                            }
                        }
                        else
                        {
                            if (m_OthersErrorEvent != null)
                            {
                                m_OthersErrorEvent(new CustomException("reponse is Null ", ErrorType.ConnectFailed));
                            }
                            break;
                        }
                    }

                    response.Close();
                    request.Abort();
                    response.Close();
                    stream.Close();
                    fsstream.Close();


                }
                catch (WebException ex)
                {
                    DealWithEx(ex);

                }
            }

            //LogMgr.Log("DOWNLOADFILE ISDONE");

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

        }


#if USE_COR
		public Stream GetStream ()
		{
			return m_stream;
		}

		WWW GetForm (bool isPost)
		{
			WWW www;
			if (isPost) {
				if (m_form == null)
				{
					LogMgr.LogError ("Form is Null May Cause Erros while Use Http Post!");
				}
				www = new WWW (m_curRequest.field0, m_form);
			} 
			else 
			{
				www = new WWW (m_curRequest.field0);
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
