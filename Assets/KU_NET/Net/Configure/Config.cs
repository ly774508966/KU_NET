using UnityEngine;
using System.Collections;

namespace Kubility
{
		public class Config : MonoSingleTon<Config>
		{
		
				public int HttpSpeedLimit = 102400;

				#region Message

				public int ARGS_MAX_NUM = 20;
				public int EACH_SOCKET_RECEIVE_SIZE = 300;
				// 10240;

				public short Retry_Times = 3;
				public int Thread_MaxSize = 16;

				#endregion
		}
}


