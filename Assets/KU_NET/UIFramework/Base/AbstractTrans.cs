using UnityEngine;
using System.Collections;

namespace Kubility
{
	[System.Serializable]
	public abstract class AbstractTrans :ScriptableObject
	{
		public virtual void OnCreateTrans()
		{

		}

		public virtual void OnEnterTrans()
		{

		}

		public virtual void OnExitTrans()
		{

		}

		public virtual void OnDestroyTrans()
		{

		}

		public virtual void OnPauseTrans()
		{
			
		}

		public virtual void OnResumeTrans()
		{
			
		}
	}
}


