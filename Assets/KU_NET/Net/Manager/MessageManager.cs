using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
    public class MessageManager : SingleTon<MessageManager>
    {
		public 	enum MessageOperation
		{
			None,
			Operation_send,
			Operation_receive,
		}

		/// <summary>
		/// Represent a data type where elements with higher priority are "served" before elements with lower priority.
		/// Elements of same priority are served with random order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class PriorityQueue<T> where T : IComparable<T>
		{
			private List<T> dataHeap;
			
			/// <summary>
			/// Initializes a new instance of the Priority Queue that is empty.
			/// </summary>
			public PriorityQueue()
			{
				this.dataHeap = new List<T>();
			}
			
			/// <summary>
			/// Adds an element to the queue and then heapifies so that the element with highest priority is at front.
			/// </summary>
			/// <param name="value">The element to enqueue.</param>
			public void Enqueue(T value)
			{
				this.dataHeap.Add(value);
				BubbleUp();
			}
			
			/// <summary>
			/// Removes the element at the front and returns it.
			/// </summary>
			/// <returns>The element that is removed from the queue front.</returns>
			/// <exception cref="InvalidOperationException">The Queue is empty.</exception>
			public bool Dequeue(out T data)
			{
				if (this.dataHeap.Count <= 0)
				{
					data = default(T);
					return false;
				}
				
				T result = dataHeap[0];
				int count = this.dataHeap.Count - 1;
				dataHeap[0] = dataHeap[count];
				dataHeap.RemoveAt(count);
				ShiftDown();
				data =result;
				return true;
			}
			
			/// <summary>
			/// A method to maintain the heap order of the elements after enqueue. If the parent of the newly added 
			/// element is with less priority - swap them.
			/// </summary>
			private void BubbleUp()
			{
				int childIndex = dataHeap.Count - 1;
				
				while (childIndex > 0)
				{
					int parentIndex = (childIndex - 1) / 2;
					
					if (dataHeap[childIndex].CompareTo(dataHeap[parentIndex]) >= 0)
					{
						break;
					}
					
					SwapAt(childIndex, parentIndex);
					childIndex = parentIndex;
				}
			}
			
			/// <summary>
			/// A method to maintain the heap order of the elements after denqueue. We check priorities of both children and parent node.
			/// </summary>
			private void ShiftDown()
			{
				int count = this.dataHeap.Count - 1;
				int parentIndex = 0;
				
				while (true)
				{
					int childIndex = parentIndex * 2 + 1;
					if (childIndex > count)
					{
						break;
					}
					
					int rightChild = childIndex + 1;
					if (rightChild <= count && dataHeap[rightChild].CompareTo(dataHeap[childIndex]) < 0)
					{
						childIndex = rightChild;
					}
					if (dataHeap[parentIndex].CompareTo(dataHeap[childIndex]) <= 0)
					{
						break;
					}
					
					SwapAt(parentIndex, childIndex);
					parentIndex = childIndex;
				}
			}
			
			/// <summary>Returns the element at the front of the Priority Queue without removing it.</summary>
			/// <returns>The element at the front of the queue.</returns>
			/// <exception cref="InvalidOperationException">The Queue is empty.</exception>
			public T Peek()
			{
				if (this.dataHeap.Count == 0)
				{
					LogMgr.LogError("Queue is empty.");
					return default(T);
				}
				
				T frontItem = dataHeap[0];
				return frontItem;
			}
			
			/// <summary>
			/// Gets the number of elements currently contained in the <see cref="PriorityQueue"/>
			/// </summary>
			/// <returns>The number of elements contained in the <see cref="PriorityQueue"/></returns>
			public int Count
			{
				get
				{
					return dataHeap.Count;
				}
			}
			
			/// <summary>Removes all elements from the queue.</summary>
			public void Clear()
			{
				this.dataHeap.Clear();
			}
			

			
			/// <summary>
			/// Checks the consistency of the heap.
			/// </summary>
			/// <returns>True if the heap property is ok.</returns>
			public bool IsConsistent()
			{
				if (dataHeap.Count == 0)
				{
					return true;
				}
				
				int lastIndex = dataHeap.Count - 1; 
				for (int parentIndex = 0; parentIndex < dataHeap.Count; ++parentIndex) 
				{
					int leftChildIndex = 2 * parentIndex + 1; 
					int rightChildIndex = 2 * parentIndex + 2;
					
					if (leftChildIndex <= lastIndex && dataHeap[parentIndex].CompareTo(dataHeap[leftChildIndex]) > 0)
					{
						return false;
					}
					if (rightChildIndex <= lastIndex && dataHeap[parentIndex].CompareTo(dataHeap[rightChildIndex]) > 0)
					{
						return false;
					}
				}
				
				return true;
			}
			
			/// <summary>
			/// A method that swaps the elements at the given indices of the heap.
			/// </summary>
			/// <param name="first">The first element index.</param>
			/// <param name="second">The second element index.</param>
			private void SwapAt(int first,int second)
			{
				T value = dataHeap[first];
				dataHeap[first] = dataHeap[second];
				dataHeap[second] = value;
			}

		}
		
		
		struct MessageTask :IEquatable<MessageTask>,IComparable<MessageTask>
		{
			public BaseMessage message;
			public AsyncSocket user;
			public MessageOperation operation;

			public int CompareTo (MessageTask other)
			{
				if(this.message == null || other.message == null)
				{
					return 0;
				}
				
				return -(message.GetPriority() - other.message.GetPriority());
			}
			
			public bool Equals (MessageTask other)
			{
				if(message != message ) return false;
				if(user != other.user) return false;
				if( operation != other.operation) return false;
				return true;
			}
		}
		
		public class QuequeTuple
		{
			PriorityQueue<MessageTask> queue;
			
			public int Size
			{
				get
				{
					return queue.Count;
				}
			}
			
			internal QuequeTuple()
			{
				queue = new PriorityQueue<MessageTask>();
			}

			
			public void Push_Back(BaseMessage t,AsyncSocket socket, MessageOperation  op = MessageOperation.Operation_send)
			{
				if (t == null && op != MessageOperation.Operation_receive)
				{
					LogMgr.LogError("Push_Back  args is Null");
					return;
                }
				MessageTask task;
				task.message = t;
				task.user = socket;
				task.operation = op;

				lock(queue)
				{
					queue.Enqueue(task);
				}
            }

            public bool Pop_First(out BaseMessage ev,out AsyncSocket socket)
            {
				MessageTask task;
				bool ret =false;
				
				lock (queue)
				{
					ret =queue.Dequeue(out task);
					
				}
				
				if(!ret)
				{
					ev = null;
					socket = null;
					return false;
				}
				else
				{
					ev =task.message;
					socket =task.user;
					return true;
				}
			}
			
			
			public MessageOperation GetLastOperation()
			{
				if (queue.Count == 0)
				{
					return MessageOperation.None;
				}
				else
				{
					MessageTask task;
					lock(queue)
					{
						task = queue.Peek();
					}

					return task.operation;
				}
			}
          
            public void Clear()
            {
                this.queue.Clear();
            }

        }

        QuequeTuple SendQueue;
        QuequeTuple ReceiveQueue;
        QuequeTuple BufferQueue;

        LinkedList<MessageHead> m_DataBufferList;

        Dictionary<short, Stack<object>> callbackDic;

        Action<BaseMessage> custom;

        object mlock;

        ByteBuffer cache;

        public MessageManager()
        {
            this.cache = new ByteBuffer(2048);
            this.mlock = new object();
            this.SendQueue = new QuequeTuple();
            this.ReceiveQueue = new QuequeTuple();
            this.BufferQueue = new QuequeTuple();
            this.m_DataBufferList = new LinkedList<MessageHead>();
            this.callbackDic = new Dictionary<short, Stack<object>>();
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
            lock (m_lock)
            {
                cache += data;
            }
            CheckNewData();

        }

        public void PushToWaitQueue<T>( BaseMessage message,Action<T> callback)
        {
            if (!callbackDic.ContainsKey(message.DataHead.CMD))
            {
                var stack = new Stack<object>();
                stack.Push(callback);
                callbackDic.Add(message.DataHead.CMD, stack);
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


        byte[] CacheRead(int begin = 0, int len = -1)
        {
            byte[] readdata;
            if (len < 0)
            {
                lock (m_lock)
                {
                    readdata = cache.ConverToBytes();
                }
            }
            else
            {
                lock (m_lock)
                {
                    readdata = cache.Read(begin, len);
                }
            }

            return readdata;
        }

        void CheckNewData()
        {
            try
            {
                MessageHead head = null;
                lock (mlock)
                {
                    if (m_DataBufferList.Count > 0)
                    {
                        head = m_DataBufferList.First.Value;
                    }
                }

				if (cache.DataCount >= MessageInfo.HeadLen)
                {

                    int leftLen = cache.DataCount;
                    int blen = 0;
                    if (head == null)
                    {
                        head = BaseMessage.ReadHead(CacheRead());
                        lock (m_lock)
                        {
                            m_DataBufferList.AddLast(head);
							cache.Clear(MessageInfo.HeadLen);
                        }

                        leftLen = cache.DataCount;
                        blen = head.bodyLen;
                    }
                    else
                    {
                        blen = head.bodyLen;
                    }

                    if (leftLen >= blen)
                    {
						BaseMessage message = DataInterface.CurDataCore.DynamicCreate(CacheRead(0, (int)blen), head);

						if(message == null)
						{

							LogMgr.LogError("Error >> Receive Null Or Deserialze failed or connection Closed");
							return;
						}

                        if (custom == null)
                            DealWithMessage(message);
                        else
                            custom(message);


                        lock (mlock)
                        {
                            m_DataBufferList.RemoveFirst();
                        }

// 						LogMgr.LogError("reduce size  = "+(leftLen -cache.DataCount) );

                        if (cache.DataCount > 0)
                        {
                            CheckNewData();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogMgr.LogError(ex);
            }

        }

        void DealWithMessage(BaseMessage message)
        {

            if (message != null && message.DataHead.Version == Config.mIns.Version)
            {
                Stack<object> stack;
                bool ret = false;
                lock (mlock)
                {
					LogMgr.Log("Info  >>> Cmd = "+ message.DataHead.CMD);
                    ret = callbackDic.TryGetValue(message.DataHead.CMD, out stack);
                }

                if (ret)
                {
                    
					if (stack == null || stack.Count ==0)
					{
                        LogMgr.LogError("Receie data But Action is Null!");
                        return;
                    }
					object ac = stack.Pop();

                    if (MessageInfo.MessageType == MessageDataType.Json)
                    {
                        JsonMessage json = (JsonMessage)message;
                        Action<string> rac = (Action<string>)ac;
                        rac(json.jsonData);
                    }
					else if (MessageInfo.MessageType == MessageDataType.Struct)
                    {
                        StructMessage data = (StructMessage)message;
                        Action<ValueType> rac = (Action<ValueType>)ac;
                        rac(data.StructData);
                    }
					else if (MessageInfo.MessageType == MessageDataType.ProtoBuf)
					{
						ProtobufMessage data = (ProtobufMessage)message;
						Action<byte[]> rac = (Action<byte[]>)ac;
						rac(data.ProtobufData);
					}

                    lock (mlock)
                    {
                        if (stack.Count == 0)
                            callbackDic.Remove(message.DataHead.CMD);
                    }

                }

            }
        }
    }
}


