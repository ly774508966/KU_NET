using UnityEngine;
using System.Collections;
using System;

namespace Kubility
{
		public delegate void VoidDelegate ();
		public delegate void ExceptionDelegate(Exception ex);

		/// <summary>
		/// Connect events.
		/// </summary>
		public interface ConnectEvents
		{
				VoidDelegate m_ConnectCloseEvent { get; set;}

				VoidDelegate m_ConnectFailedEvent{ get;set; }

				VoidDelegate m_TimeOutEvent{ get;set; }

				ExceptionDelegate m_OthersErrorEvent{ get;set; }

				Action<string> m_SuccessEvent{ get; set; }

				Action<byte[],float,bool> onProcess { get; set; }

				void UnRegisterAll ();

		}
}


