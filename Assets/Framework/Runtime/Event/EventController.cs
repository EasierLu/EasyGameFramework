using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Pool;
using EGFramework.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EGFramework.Runtime.Event
{
    public class EventController : IReference
    {
        public int MaxDelegateNum => m_ArrayPool.Length;

        private Dictionary<string, IEventHandler> m_EventDelegates;
        private CappedArrayPool<UniTask> m_ArrayPool;
        private HashSet<string> m_PermanentEvents;

        public EventController(int maxDelegateNum = 8)
        {
            m_EventDelegates = new Dictionary<string, IEventHandler>();
            m_ArrayPool = ReferencePool.Acquire<CappedArrayPool<UniTask>>();
            m_ArrayPool.Init(maxDelegateNum);
            m_PermanentEvents = new HashSet<string>();
        }

        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkPermanent(string eventKey)
        {
            if (!m_PermanentEvents.Contains(eventKey))
            {
                m_PermanentEvents.Add(eventKey);
            }
        }

        public void UnmarkPermanent(string eventKey)
        {
            if (m_PermanentEvents.Contains(eventKey))
            {
                m_PermanentEvents.Remove(eventKey);
            }
        }

        public bool ContainsEvent(string eventKey)
        {
            return m_EventDelegates.ContainsKey(eventKey);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public void Cleanup()
        {
            List<string> eventToRemove = new List<string>();

            foreach (var pair in m_EventDelegates)
            {
                if (!m_PermanentEvents.Contains(pair.Key))
                {
                    eventToRemove.Add(pair.Key);
                }
            }

            foreach (string eventKey in eventToRemove)
            {
                m_EventDelegates.Remove(eventKey);
            }
        }

        private T GetEventHandler<T>(string eventKey) where T : class, IEventHandler, new()
        {
            T collection;
            if (!m_EventDelegates.ContainsKey(eventKey))
            {
                collection = ReferencePool.Acquire<T>();
                collection.EventKey = eventKey;
                collection.ArrayPool = m_ArrayPool;
                m_EventDelegates.Add(eventKey, collection);
            }
            else
            {
                collection = m_EventDelegates[eventKey] as T;
            }

            if (collection == null)
            {
                Log.ErrorFormat("Key:{0}. Event handler error: types of parameters are not match.", eventKey);
            }

            return collection;
        }

        #region 增加事件
        public void AddEventListener(string eventKey, Action handle)
        {
            EventHandler collection = GetEventHandler<EventHandler>(eventKey);
            if (collection != null)
            {
                collection.Add(handle);
            }
        }

        public void AddEventListener(string eventKey, Func<CancellationToken, UniTask> handle)
        {
            EventHandler collection = GetEventHandler<EventHandler>(eventKey);
            if (collection != null)
            {
                collection.Add(handle);
            }
        }

        public void AddEventListener<T>(string eventKey, Action<T> handle)
        {
            EventHandler<T> collection = GetEventHandler<EventHandler<T>>(eventKey);
            if (collection != null)
            {
                collection.Add(handle);
            }
        }

        public void AddEventListener<T>(string eventKey, Func<T, CancellationToken, UniTask> handle)
        {
            EventHandler<T> collection = GetEventHandler<EventHandler<T>>(eventKey);
            if (collection != null)
            {
                collection.Add(handle);
            }
        }

        #endregion

        #region 移除事件

        public void RemoveEventListener(string eventKey, Action handle)
        {
            EventHandler collection = GetEventHandler<EventHandler>(eventKey);
            if (collection != null)
            {
                collection.Remove(handle);
            }
        }

        public void RemoveEventListener(string eventKey, Func<CancellationToken, UniTask> handle)
        {
            EventHandler collection = GetEventHandler<EventHandler>(eventKey);
            if (collection != null)
            {
                collection.Remove(handle);
            }
        }

        public void RemoveEventListener<T>(string eventKey, Action<T> handle)
        {
            EventHandler<T> collection = GetEventHandler<EventHandler<T>>(eventKey);
            if (collection != null)
            {
                collection.Remove(handle);
            }
        }

        public void RemoveEventListener<T>(string eventKey, Func<T, CancellationToken, UniTask> handle)
        {
            EventHandler<T> collection = GetEventHandler<EventHandler<T>>(eventKey);
            if (collection != null)
            {
                collection.Remove(handle);
            }
        }
        #endregion

        #region 触发事件
        public void TriggerEvent(string eventKey, CancellationToken ct = default)
        {
            AsyncTriggerEvent(eventKey, ct).Forget();
        }

        public async UniTask AsyncTriggerEvent(string eventKey, CancellationToken ct = default)
        {
            EventHandler collection = GetEventHandler<EventHandler>(eventKey);
            if (collection != null)
            {
                await collection.AsyncTrigger(ct);
            }
        }

        public void TriggerEvent<T>(string eventKey, T arg1, CancellationToken ct = default)
        {
            AsyncTriggerEvent(eventKey, arg1, ct).Forget();
        }

        public async UniTask AsyncTriggerEvent<T>(string eventKey, T arg1, CancellationToken ct = default)
        {
            EventHandler<T> collection = GetEventHandler<EventHandler<T>>(eventKey);
            if (collection != null)
            {
                await collection.AsyncTrigger(arg1, ct);
            }
        }
        #endregion

        public void Clear()
        {
            foreach (var collection in m_EventDelegates.Values)
            {
                ReferencePool.Release(collection);
            }
            m_EventDelegates.Clear();
            ReferencePool.Release(m_ArrayPool);
        }
    }
}