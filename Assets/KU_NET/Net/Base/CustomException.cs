using UnityEngine;
using System.Collections;
using System;
using System.Net;

namespace Kubility
{
	public class CustomException :System.Exception
	{
		public ErrorType ErrorCode = ErrorType.UnKnown;
		public CustomException () : base ()
		{
			
		}
		
		public CustomException (string errInfo) : base (errInfo)
		{
			
		}

		public CustomException (string errInfo ,ErrorType error) : base (errInfo)
		{
			this.ErrorCode = error;
		}
		
		public CustomException (string errinfo, Exception ex) : base (errinfo, ex)
		{
			
		}

		public CustomException (string errinfo, Exception ex ,ErrorType error) : base (errinfo, ex)
		{
			this.ErrorCode = error;
		}
	}
}


