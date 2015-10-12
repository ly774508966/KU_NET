using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Kubility
{
//	public interface  DataInterface
//	{
//		/// <summary>
//		/// ret  = read size
//		/// </summary>
//		/// <param name="data">Data.</param>
//		void  Deserialize(ByteBuffer data);
//		
//	}
//
//


	public static class StructDataFactory
	{

		public static StructMessage Create(ByteBuffer data,MessageHead head)  
		{

			if(head.CMD == 102)
			{
				var rdata = (HeartBeatStructData)KTool.BytesToStruct(data,typeof(HeartBeatStructData));
				StructMessage value = StructMessage.Create(head,rdata);
				return value;
			}

			return null;
		}
	}

}

