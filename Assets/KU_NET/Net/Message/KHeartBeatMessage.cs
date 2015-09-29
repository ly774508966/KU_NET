using UnityEngine;
using System.Collections;
using Kubility;



public class KHeartBeatMessage : BaseMessage
{

	public KHeartBeatMessage()
	{
		this.messageType = KMessageType.KHeart_Beat;
	}
}
