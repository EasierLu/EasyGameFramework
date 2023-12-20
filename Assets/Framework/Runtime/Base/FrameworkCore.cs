using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Pool;
using EGFramework.Runtime.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Base
{
    public class FrameworkCore
    {
        private static Transform m_ComponentRoot;
        private const int DEFAULT_CAPACITY = 32;
        private static readonly List<FrameworkComponent> m_Components = new List<FrameworkComponent>(DEFAULT_CAPACITY);
        private static readonly Dictionary<string, FrameworkComponent> m_ComponentsDic = new Dictionary<string, FrameworkComponent>(DEFAULT_CAPACITY);

        public static void Init(Transform componentRoot = null)
        {
            if (m_ComponentRoot == null)
            {
                if (componentRoot == null)
                {
                    GameObject gameObject = new GameObject(typeof(FrameworkCore).Name);
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    m_ComponentRoot = gameObject.transform;
                }
                else
                {
                    m_ComponentRoot = componentRoot;
                    UnityEngine.Object.DontDestroyOnLoad(componentRoot);
                }
            }
            else
            { 
                Log.Error("FrameworkCore has been initialized.");
            }
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var component in m_Components)
            {
                component.ComponentUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public static async UniTask Shutdown()
        {
            foreach (var component in m_Components)
            {
                await component.Shutdown();
            }

            m_Components.Clear();
            m_ComponentsDic.Clear();
            ReferencePool.ClearAll();
            Utility.Marshal.FreeCachedHGlobal();
        }

        public static async UniTask<T> AddComponent<T>() where T : FrameworkComponent
        {
            return await AddComponent(typeof(T)) as T;
        }

        public static async UniTask<FrameworkComponent> AddComponent(Type componentType)
        {
            string fullName = componentType.FullName;
            if (m_ComponentsDic.ContainsKey(fullName))
            { 
                Log.Error(Utility.Text.Format("Already exist component '{0}'.", fullName));
                return m_ComponentsDic[fullName];
            }

            FrameworkComponent component = CreateComponent(componentType);
            if (component == null)
            {
                throw new FrameworkException(Utility.Text.Format("Can not add component '{0}'.", componentType.FullName));
            }

            await component.Init();

            m_ComponentsDic.Add(componentType.FullName, component);

            if (m_Components.Count == 0)
            {
                m_Components.Add(component);
            }
            else
            {
                int index = m_Components.Count;
                for (int i = 0; i < m_Components.Count; i++)
                {
                    if (m_Components[i].Priority > component.Priority)
                    { 
                        index = i; 
                        break;
                    }
                }
                m_Components.Insert(index, component);
            }
            

            return component;
        }

        public static T GetComponent<T>() where T : FrameworkComponent
        {
            return GetComponent(typeof(T)) as T;
        }

        public static FrameworkComponent GetComponent(Type componentType)
        {
            if (m_ComponentsDic.ContainsKey(componentType.FullName))
            {
                return m_ComponentsDic[componentType.FullName];
            }
            else
            {
                Log.Error(Utility.Text.Format("Can not find component '{0}'.", componentType.FullName));
                return null;
            }
        }

        public static void RemoveComponent<T>() where T : FrameworkComponent
        {
            RemoveComponent(typeof(T));
        }

        public static void RemoveComponent(Type componentType)
        {
            if(m_ComponentsDic.ContainsKey(componentType.FullName))
            {
                for (int i = 0; i < m_Components.Count; i++)
                {
                    var component = m_Components[i];
                    if (component.GetType() == componentType)
                    {
                        m_Components.Remove(component);
                        break;
                    }
                }
                m_ComponentsDic[componentType.FullName].Shutdown().Forget();
                m_ComponentsDic.Remove(componentType.FullName);
            }
            else
            {
                Log.Error(Utility.Text.Format("Can not find component '{0}'.", componentType.FullName));
            }
        }

        public static bool HasComponent<T>() where T : FrameworkComponent
        {
            return HasComponent(typeof(T));
        }

        public static bool HasComponent(Type componentType)
        {
            return m_ComponentsDic.ContainsKey(componentType.FullName);
        }

        private static FrameworkComponent CreateComponent(Type t)
        {
            if (m_ComponentRoot == null)
            { 
                throw new FrameworkException(Utility.Text.Format("You must initialize '{0}' first.", typeof(FrameworkCore).Name));
            }
            GameObject go = new GameObject(t.Name);
            go.transform.SetParent(m_ComponentRoot);
            return go.AddComponent(t) as FrameworkComponent;
        }
    }
}
