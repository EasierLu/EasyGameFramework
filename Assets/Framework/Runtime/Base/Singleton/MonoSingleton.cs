using System;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        protected static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = typeof(T).Name;
                    m_Instance = gameObject.AddComponent<T>();
                    DontDestroyOnLoad(gameObject);
                    m_Instance.transform.SetParent(GetSingletonRoot());
                }
                return m_Instance;
            }
        }

        public static T GetInstance() => m_Instance;

        protected virtual void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                m_Instance.transform.SetParent(GetSingletonRoot());
            }

            OnInit();
        }

        /// <summary>
        /// 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
        /// </summary>
        public void Startup() { }

        protected virtual void OnInit()
        {

        }

        public void Dispose()
        {
            if (m_Instance != null)
            {
                Destroy(m_Instance.gameObject);
                m_Instance = null;
            }
        }

        private static Transform m_SingletonRoot;

        private static Transform GetSingletonRoot()
        {
            if (m_SingletonRoot == null)
            {
                m_SingletonRoot = new GameObject("MonoSingleton").transform;
                GameObject.DontDestroyOnLoad(m_SingletonRoot);
            }
            return m_SingletonRoot;
        }
    }
}
