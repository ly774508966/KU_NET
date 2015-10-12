using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
	public class MessageManager :SingleTon<MessageManager> 
	{
		
		public class QuequeTuple
		{
			MiniTuple<int,LinkedList<BaseMessage>> queue;
			
			public int Size
			{
				get
				{
					return queue.field1.Count;
				}
			}
			
			internal QuequeTuple()
			{
				Queue_Init(out queue);
			}
			
			void Queue_Init(out MiniTuple<int,LinkedList<BaseMessage>> queue)
			{
				queue = new MiniTuple<int, LinkedList<BaseMessage>>();
				queue.field0 =0;
				queue.field1 = new LinkedList<BaseMessage>();
			}
			
			bool ComparePriority(MiniTuple<int,LinkedList<BaseMessage>> queue,int value)
			{
				if(queue.field0 < value)
				{
					queue.field0 = value;
					return true;
					
				}
				return false;
			}
			
			public void Push_Back(BaseMessage t)
			{
				if(t == null)
				{
					LogMgr.LogError("Push_Back  args is Null");
					return;
				}
				
				if(ComparePriority(queue,t.GetPriority()))
				{
					lock(queue.field1)
					{
						queue.field1.AddFirst(t);
					}
				}
				else
				{
					lock(queue.field1)
					{
						queue.field1.AddLast(t);
					}
				}
				
				
			}
			
			public bool Pop_First(out BaseMessage ev) 
			{
				
				if(queue.field1.Count >0)
				{
					ev = queue.field1.First.Value;
					lock(queue.field1)
					{
						queue.field1.RemoveFirst();
					}
					
					return true;
				}
				else
				{
					ev  = null;
					return false;
				}
			}
			
			public bool Pop_Back(out BaseMessage ev) 
			{
				
				if(queue.field1.Count >0)
				{
					ev =queue.field1.Last.Value;
					lock(queue.field1)
					{
						queue.field1.RemoveLast();
					}
					return true;
				}
				else
				{
					ev = null;
					return false;
				}
			}
			
			public bool Remove(BaseMessage message)
			{
				lock(queue.field1)
				{
					return queue.field1.Remove(message);
				}
				
			}
			
			public bool Remove(int index)
			{
				if(index > queue.field1.Count)
				{
					return false;
				}
				else
				{
					int i=0;
					LinkedListNode<BaseMessage> first = queue.field1.First;
					while(first.Next != null)
					{
						if(i ==  index)
						{
							lock(queue.field1)
							{
								queue.field1.Remove(first);
							}
							
							return true;
						}
						
						i++;
						first = first.Next;
					}
					return false;
				}
			}
			
			
			public BaseMessage Get_First()
			{
				if(queue.field1.Count == 0)
				{
					return null;
				}
				else
				{
					return queue.field1.First.Value;
				}
			}
			
			public BaseMessage Get_Last()
			{
				if(queue.field1.Count == 0)
				{
					return null;
				}
				else
				{
					return queue.field1.Last.Value;
				}
			}
			
			public BaseMessage Get(int index)
			{
				if(index > queue.field1.Count)
				{
					return null;
				}
				else
				{
					int i=0;
					LinkedListNode<BaseMessage> first = queue.field1.First;
					while(first.Next != null)
					{
						if(i ==  index)
						{
							return first.Value;
						}
						
						i++;
						first = first.Next;
					}
					return null;
					
				}
			}
			
			public void Clear()
			{
				this.queue.field1.Clear();
			}
			
		}
		
		QuequeTuple SendQueue ;
		QuequeTuple ReceiveQueue ;
		QuequeTuple BufferQueue ;
		
		LinkedList<MiniTuple<MessageHead,ByteBuffer>> m_SendBufferList ;
		
		Dictionary<uint,Stack<object>> callbackDic ;

		Action<BaseMessage> custom;

		object mlock ;
		
		public MessageManager()
		{
			this.mlock = new object();
			this.SendQueue = new QuequeTuple();
			this.ReceiveQueue = new QuequeTuple();
			this.BufferQueue = new QuequeTuple();
			this.m_SendBufferList = new LinkedList<MiniTuple<MessageHead, ByteBuffer>>();
			this.callbackDic = new Dictionary<uint, Stack<object>>();
		}
		
		
		public QuequeTuple GetSendQueue()
		{
			return SendQueue;
		}
		
		public QuequeTuple GetReceiveQueue()
		{
			return ReceiveQueue;
		}
		
		
		public QuequeTuple GetBufferQueue()
		{
			return BufferQueue;
		}
		
		public void PushToReceiveBuffer(byte[] data)
		{
			
			CheckNewData(data);
			
		}
		
		public void PushToWaitQueue<T>(BaseMessage message,Action<T> callback)
		{
			if(!callbackDic.ContainsKey(message.DataHead.CMD))
			{
				var stack = new Stack<object>();
				stack.Push(callback);
				callbackDic.Add(message.DataHead.CMD,stack);
			}
			else
			{
				callbackDic[message.DataHead.CMD].Push(callback);
			}
		}

		public void RegiseterCustomDeal(Action<BaseMessage> ev)
		{
			custom = ev;
		}
		
		MiniTuple<MessageHead,ByteBuffer> TryGetTuple()
		{
			if(m_SendBufferList.Count >0)
			{
				return m_SendBufferList.First.Value;
			}
			else
			{
				var tuple =new MiniTuple<MessageHead,ByteBuffer>();
				tuple.field1 = new ByteBuffer();
				m_SendBufferList.AddLast(tuple);
				return tuple;
			}
		}


		
		void CheckNewData(byte[] data)
		{
			try
			{
				MiniTuple<MessageHead,ByteBuffer> tuple;
				lock(mlock)
				{
					tuple = TryGetTuple();
				}

				//LogMgr.LogError("new data = "+ data.Length +" now ="+ tuple.field1.DataCount);
				tuple.field1 += data;
				if(tuple.field1.DataCount >= MessageHead.HeadLen)
				{
					if(tuple.field0 ==null)
						tuple.field0 = BaseMessage.ReadHead(tuple.field1.ConverToBytes());
					else
						tuple.field0.buffer += data;
					
					//left data
					int leftLen = tuple.field0.buffer.DataCount;
					UInt32 blen = tuple.field0.bodyLen;
//					LogMgr.LogError("left  Len = "+leftLen +"  blen ="+ blen +" tuple1 = "+ tuple.field1.DataCount);

					if(leftLen >= blen)
					{
						BaseMessage message= null;
						if(tuple.field0.Flag ==1)//json
						{
							message = JsonMessage.Create(tuple.field0.buffer.Read(0,(int)blen),tuple.field0);

						}
						else if(tuple.field0.Flag ==2)
						{
							message =StructDataFactory.Create(tuple.field0.buffer,tuple.field0);
						}
						else if(tuple.field0.Flag ==0)
						{
							LogMgr.LogError("Read Flag is 0 cant create");
						}

						if(custom == null)
							DealWithMessage(message);
						else
							custom(message);

					
						lock(mlock)
						{
							
							m_SendBufferList.RemoveFirst();
						}

						//LogMgr.LogError("=====   left = "+ tuple.field0.buffer.DataCount );
						
						if(leftLen > blen)
						{
							CheckNewData(tuple.field0.buffer.ConverToBytes());
						}
						
						
						
					}
					
				}
			}
			catch(Exception ex)
			{
				LogMgr.LogError(ex);
			}
			
		}
		
		void DealWithMessage(BaseMessage message)
		{

			if(message != null && message.DataHead.Version == Config.mIns.Version)
			{
				Stack<object> stack;
				bool ret =false;
				lock(mlock)
				{
					ret = callbackDic.TryGetValue(message.DataHead.CMD,out stack);
				}

				if(	ret)
				{
					object ac = stack.Pop();
					if(ac == null)
					{
						LogMgr.LogError("Receie data But Action is Null!");
						return;
					}

					if(message.DataHead.Flag == 1)
					{
						JsonMessage json =(JsonMessage)message;
						Action<string> rac = (Action<string>)ac;
						rac(json.jsonData);
					}
					else if(message.DataHead.Flag == 2)
					{
						StructMessage data =(StructMessage)message;
						Action<ValueType> rac = (Action<ValueType>)ac;
						rac(data.StructData);
					}

					lock(mlock)
					{
						if(stack.Count ==0)
							callbackDic.Remove(message.DataHead.CMD);
					}

				}
				
			}
		}
	}
}


