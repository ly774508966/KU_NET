using UnityEngine;
using System.Collections;
using System;

namespace Kubility
{
	public delegate void VoidDelegate();

	/// <summary>
	/// Connect events.
	/// </summary>
	public interface ConnectEvents 
	{
		VoidDelegate m_ConnectCloseEvent {get;}
		VoidDelegate m_ConnectFailedEvent{get;}
		VoidDelegate m_TimeOutEvent{get;}
		VoidDelegate m_OthersErrorEvent{get;}
		Action<string> m_SuccessEvent{get;set;}
		
		Action<string,float,bool> onProcess {get;set;}

		void UnRegisterAll();

	}
}


