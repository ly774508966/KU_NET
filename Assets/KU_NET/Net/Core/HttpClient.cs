using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubility;
using System;

public class HttpClient : AbstractNetUnit
{
    HttpUnit m_http;

    private Stack<Action> resplist;

    public KThread curThread
    {
        get
        {

            return m_http.getThread();
        }
    }

    public HttpClient()
    {
        m_http = CreateHttpConnect();
        resplist = new Stack<Action>();
//        GlobalHelper.mIns.RegisterFixedUpdate(DofixedUpdate);
    }


    public void BeginDownLoadFileFlushToFile(string URL, string Filepath, Action<byte[], float, bool> callback, bool AutoStart = false)
    {
        m_http.BeginDownLoadFileFlushToFile(URL, Filepath, callback, AutoStart);
    }

	public void BeginDownLoadFileFlushToMemory(string URL, Action<byte[], float, bool> callback, bool AutoStart = false)
	{
		m_http.BeginDownLoadFileFlushToMemory(URL,callback, AutoStart);
	}

    public void BeginGet(string URL, Action<string> callback, bool AutoStart = true)
    {
        m_http.BeginGet(URL, delegate(string obj)
        {
			if(!m_http.UseCor)
			{
				Action ac = delegate()
				{
					callback(obj);
				};
				
				resplist.Push(ac);
			}
			else
			{
				callback(obj);
			}
        }, AutoStart);
    }

    public void BeginPost(string URL, Action<string> callback, bool AutoStart = false)
    {
        m_http.BeginPost(URL, delegate(string obj)
        {
			if(!m_http.UseCor)
			{
				Action ac = delegate()
				{
					callback(obj);
				};
				
				resplist.Push(ac);
			}
			else
			{
				callback(obj);
			}

        }, AutoStart);
    }

    public void StartConnect()
    {
        m_http.StartConnect();
    }

    public void AddField(string field, string content)
    {
        m_http.AddField(field, content);
    }

	public void AddField(string field, int content)
	{
		m_http.AddField(field, content);
	}
	/// <summary>
	/// Pause the lastest thread
	/// </summary>
    public void Pause()
    {
        KThread th = curThread;
        if (th != null)
            th.ForceSuspend();
    }

    public void Close()
    {
        m_http.Close();
//        GlobalHelper.mIns.UnRegisterFixedUpdate(DofixedUpdate);
    }

    public void PopRequest()
    {
        m_http.PopRequest();
    }
	/// <summary>
	/// Resume the lastest thread
	/// </summary>
    public void Resume()
    {
        KThread th = curThread;
        if (th != null)
            th.ForceResume();
    }

    #region others

    public  void DofixedUpdate()
    {
        while (this.resplist.Count > 0)
        {
            resplist.Pop()();
        }
    }

    #endregion

}
