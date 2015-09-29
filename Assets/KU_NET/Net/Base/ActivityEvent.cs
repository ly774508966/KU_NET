using UnityEngine;
using System.Collections;

namespace Kubility
{
	public interface ActivityEvent
	{

		bool isRunning{get;}
		
		void OnCreate();
		
		void OnPause();
		
		void OnResume();
		
		void OnDestroy();
		
		void OnStart();
		
	}
}


