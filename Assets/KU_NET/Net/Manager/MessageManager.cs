﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Kubility
{
    public class MessageManager : SingleTon<MessageManager>
    {

        public class QuequeTuple
        {
            ClsTuple<int, LinkedList<BaseMessage>> queue;

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

            void Queue_Init(out ClsTuple<int, LinkedList<BaseMessage>> queue)
            {
                queue = new ClsTuple<int, LinkedList<BaseMessage>>();
                queue.field0 = 0;
                queue.field1 = new LinkedList<BaseMessage>();
            }

            bool ComparePriority(ClsTuple<int, LinkedList<BaseMessage>> queue, int value)
            {
                if (queue.field0 < value)
                {
                    queue.field0 = value;
                    return true;

                }
                return false;
            }

            public void Push_Back(BaseMessage t)
            {
                if (t == null)
                {
                    LogMgr.LogError("Push_Back  args is Null");
                    return;
                }

                if (ComparePriority(queue, t.GetPriority()))
                {
                    lock (queue.field1)
                    {
                        queue.field1.AddFirst(t);
                    }
                }
                else
                {
                    lock (queue.field1)
                    {
                        queue.field1.AddLast(t);
                    }
                }


            }

            public bool Pop_First(out BaseMessage ev)
            {

                if (queue.field1.Count > 0)
                {
                    ev = queue.field1.First.Value;
                    lock (queue.field1)
                    {
                        queue.field1.RemoveFirst();
                    }

                    return true;
                }
                else
                {
                    ev = null;
                    return false;
                }
            }

            public bool Pop_Back(out BaseMessage ev)
            {

                if (queue.field1.Count > 0)
                {
                    ev = queue.field1.Last.Value;
                    lock (queue.field1)
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
                lock (queue.field1)
                {
                    return queue.field1.Remove(message);
                }

            }

            public bool Remove(int index)
            {
                if (index > queue.field1.Count)
                {
                    return false;
                }
                else
                {
                    int i = 0;
                    LinkedListNode<BaseMessage> first = queue.field1.First;
                    while (first.Next != null)
                    {
                        if (i == index)
                        {
                            lock (queue.field1)
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
                if (queue.field1.Count == 0)
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
                if (queue.field1.Count == 0)
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
                if (index > queue.field1.Count)
                {
                    return null;
                }
                else
                {
                    int i = 0;
                    LinkedListNode<BaseMessage> first = queue.field1.First;
                    while (first.Next != null)
                    {
                        if (i == index)
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

        public void PushToWaitQueue<T>(BaseMessage message, Action<T> callback)
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

                    //					LogMgr.LogError("left  Len = "+leftLen +"  blen ="+ blen );

                    if (leftLen >= blen)
                    {
						BaseMessage message = DataInterface.CurDataCore.DynamicCreate(CacheRead(0, (int)blen), head);

						if(message == null)
						{
							LogMgr.LogError("Receive Null Or Deserialze failed or connection Closed");
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
                    ret = callbackDic.TryGetValue(message.DataHead.CMD, out stack);
                }

                if (ret)
                {
                    object ac = stack.Pop();
                    if (ac == null)
                    {
                        LogMgr.LogError("Receie data But Action is Null!");
                        return;
                    }

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


