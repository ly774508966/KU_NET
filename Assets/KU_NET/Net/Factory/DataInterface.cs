using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;


namespace Kubility
{
//	public interface DataInterface
//	{
//		DataInterface CurDataCore{set;get;}
//		object DynamicCreate(byte[] data, MessageHead head);
//	}

	public abstract class DataInterface
	{
		static Dictionary<int,DataInterface> dic = new Dictionary<int, DataInterface>();
		static int curType=-1;

		static DataInterface m_CurDataCore;
		public static DataInterface CurDataCore
		{
			get
			{
				return m_CurDataCore;
			}
		}


		public abstract BaseMessage DynamicCreate(byte[] data, MessageHead head);

		public static void ConvertDataCore(MessageDataType type)
		{

			int intvalue =(int )type;

			if(curType ==intvalue)
			{
				return;
			}

			if(!dic.ContainsKey(intvalue))
			{
				throw new CustomException("please init "+ type +" 's DataCore First !");
			}
			Interlocked.Exchange(ref curType,intvalue);
			Interlocked.Exchange(ref m_CurDataCore, dic[intvalue]);

		}

		public void Clear()
		{
			curType =-1;
			m_CurDataCore = null;
			dic.Clear();
		}

		public DataInterface(MessageDataType type)
		{
			int intvalue =(int )type;
			if(!dic.ContainsKey(intvalue))
			{
				dic.Add(intvalue,this);
			}

			m_CurDataCore = this;
		}
	}
	
}

