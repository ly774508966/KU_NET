using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Kubility
{
    public enum ErrorType
    {
        None = -1,
        ConnectClose,
        ConnectFailed,
        TimeOut,
        NullRef,
        ArgError,
        NetError,
        UnKnown,
    }

    public class ErrorManager : MonoSingleTon<ErrorManager>
    {
        Stack<int> errorList = new Stack<int>();

		public bool HasError
		{
			get
			{
				return errorList.Count>0;
			}
		}

        #region connectevents

        public void Register<T>(T obj) where T : ConnectEvents
        {
            obj.m_ConnectCloseEvent += ConnectClose;
            obj.m_ConnectFailedEvent += ConnectError;
            obj.m_OthersErrorEvent += UnKnownError;
            obj.m_TimeOutEvent += TimeOut;
        }

        void ConnectError()
        {
            errorList.Push((int)ErrorType.ConnectFailed);
        }

        void ConnectClose()
        {
            errorList.Push((int)ErrorType.ConnectClose);
        }

        void TimeOut()
        {
            errorList.Push((int)ErrorType.TimeOut);
        }

        void UnKnownError(Exception ex)
        {
            if (ex.GetType() == typeof(NullReferenceException))
            {
                LogMgr.LogError("Null Ref Error");
                errorList.Push((int)ErrorType.NullRef);
            }
            else if (ex.GetType() == typeof(ArgumentException)) //
            {
                LogMgr.LogError("参数错误");
                errorList.Push((int)ErrorType.ArgError);
            }
            else if (ex.GetType() == typeof(System.Net.WebException))
            {
                LogMgr.LogError("网络错误");
                errorList.Push((int)ErrorType.NetError);
            }
            else if (ex.GetType() == typeof(CustomException))
            {
				CustomException cex = ex as CustomException;

				LogMgr.LogError(cex.Message);

				errorList.Push((int)cex.ErrorCode);
            }
            else
            {
                errorList.Push((int)ErrorType.UnKnown);
            }
        }
        #endregion

        public ErrorType PopError()
        {
            if (errorList.Count > 0)
            {
                ErrorType error = (ErrorType)errorList.Pop();
                // do something

                return error;
            }

            return ErrorType.None;
        }
    }
}


