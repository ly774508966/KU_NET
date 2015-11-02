using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kubility
{
    /// <summary>
    /// Single ton.
    /// </summary>
    public class SingleTon<T> where T : class,new()
    {
        protected static readonly object m_lock = new object();
        private static T _mins;
        public static T mIns
        {
            get
            {
                if (_mins == null)
                {
                    lock (m_lock)
                    {
                        if (_mins == null)
                        {
                            _mins = new T();

                        }

                    }
                }
                return _mins;

            }
        }

        public void Destroy()
        {
            _mins = null;
        }

    }

    public class SceneSingleTon<T> where T : class,new()
    {
        private static readonly object m_lock = new object();
        private static T _mins;
        public static T mIns
        {
            get
            {
                if (_mins == null)
                {
                    lock (m_lock)
                    {
                        if (_mins == null)
                        {
                            _mins = new T();


                        }

                    }
                }
                return _mins;

            }
        }

        public SceneSingleTon()
        {
            GlobalHelper.mIns.RegisterSceneDestroy(Destroy);
        }

        public void Destroy()
        {
            _mins = null;
            GlobalHelper.mIns.RemoveSceneDestroy(Destroy);
        }



    }


    /// <summary>
    /// Mono awake single ton.
    /// </summary>
    public class MonoEventsBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDestroy()
        {


        }

        protected virtual void OnEnble()
        {

        }

        protected virtual void OnDisable()
        {

        }

    }



    /// <summary>
    /// Mono single ton.
    /// </summary>
    public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object m_lock = new object();
        private static T _mins;
        public static T mIns
        {
            get
            {
                if (_mins == null)
                {
                    lock (m_lock)
                    {

                        if (_mins == null)
                        {

                            GameObject go = GameObject.FindGameObjectWithTag("Kubility");
                            if (go == null)
                            {
                                go = new GameObject("GLOBAL");
                                DontDestroyOnLoad(go);
                                go.tag = "Kubility";
                            }

                            _mins = go.AddComponent<T>();
                        }

                    }
                }

                return _mins;

            }
        }

        void Awake()
        {
            if(_mins == null)
                _mins = GetComponent<T>();
        }
    }
}


