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

        private Dictionary<string, IEventHandler> m_EventHandlers;
        private CappedArrayPool<UniTask> m_ArrayPool;
        private HashSet<string> m_PermanentEvents;

        public EventController(int maxAsyncDelegateNum = 8)
        {
            m_EventHandlers = new Dictionary<string, IEventHandler>();
            m_ArrayPool = ReferencePool.Acquire<CappedArrayPool<UniTask>>();
            m_ArrayPool.Init(maxAsyncDelegateNum);
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
            return m_EventHandlers.ContainsKey(eventKey);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public void Cleanup()
        {
            List<string> eventToRemove = new List<string>();

            foreach (var pair in m_EventHandlers)
            {
                if (!m_PermanentEvents.Contains(pair.Key))
                {
                    eventToRemove.Add(pair.Key);
                }
            }

            foreach (string eventKey in eventToRemove)
            {
                m_EventHandlers.Remove(eventKey);
            }
        }

        private T GetEventHandler<T>(string eventKey) where T : class, IEventHandler, new()
        {
            T eventHandler;
            if (!m_EventHandlers.ContainsKey(eventKey))
            {
                eventHandler = ReferencePool.Acquire<T>();
                eventHandler.EventKey = eventKey;
                eventHandler.ArrayPool = m_ArrayPool;
                m_EventHandlers.Add(eventKey, eventHandler);
            }
            else
            {
                eventHandler = m_EventHandlers[eventKey] as T;
            }

            if (eventHandler == null)
            {
                Log.ErrorFormat("Key:{0}. Event handler error: types of parameters are not match.", eventKey);
            }

            return eventHandler;
        }

        #region 增加事件
        public void AddEventListener(string eventKey, Action handle)
        {
            EventHandler eventHandler = GetEventHandler<EventHandler>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Add(handle);
            }
        }

        public void AddEventListener(string eventKey, Func<CancellationToken, UniTask> handle)
        {
            EventHandler eventHandler = GetEventHandler<EventHandler>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Add(handle);
            }
        }

        public void AddEventListener<T>(string eventKey, Action<T> handle)
        {
            EventHandler<T> eventHandler = GetEventHandler<EventHandler<T>>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Add(handle);
            }
        }

        public void AddEventListener<T>(string eventKey, Func<T, CancellationToken, UniTask> handle)
        {
            EventHandler<T> eventHandler = GetEventHandler<EventHandler<T>>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Add(handle);
            }
        }

        #endregion

        #region 移除事件

        public void RemoveEventListener(string eventKey, Action handle)
        {
            EventHandler eventHandler = GetEventHandler<EventHandler>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Remove(handle);
                if (eventHandler.Count == 0)
                {
                    m_EventHandlers.Remove(eventKey);
                    ReferencePool.Release(eventHandler);
                }
            }
        }

        public void RemoveEventListener(string eventKey, Func<CancellationToken, UniTask> handle)
        {
            EventHandler eventHandler = GetEventHandler<EventHandler>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Remove(handle);
                if (eventHandler.Count == 0)
                {
                    m_EventHandlers.Remove(eventKey);
                    ReferencePool.Release(eventHandler);
                }
            }
        }

        public void RemoveEventListener<T>(string eventKey, Action<T> handle)
        {
            EventHandler<T> eventHandler = GetEventHandler<EventHandler<T>>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Remove(handle);
                if (eventHandler.Count == 0)
                {
                    m_EventHandlers.Remove(eventKey);
                    ReferencePool.Release(eventHandler);
                }
            }
        }

        public void RemoveEventListener<T>(string eventKey, Func<T, CancellationToken, UniTask> handle)
        {
            EventHandler<T> eventHandler = GetEventHandler<EventHandler<T>>(eventKey);
            if (eventHandler != null)
            {
                eventHandler.Remove(handle);
                if (eventHandler.Count == 0)
                {
                    m_EventHandlers.Remove(eventKey);
                    ReferencePool.Release(eventHandler);
                }
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
            EventHandler eventHandler = GetEventHandler<EventHandler>(eventKey);
            if (eventHandler != null)
            {
                await eventHandler.AsyncTrigger(ct);
            }
        }

        public void TriggerEvent<T>(string eventKey, T arg1, CancellationToken ct = default)
        {
            AsyncTriggerEvent(eventKey, arg1, ct).Forget();
        }

        public async UniTask AsyncTriggerEvent<T>(string eventKey, T arg1, CancellationToken ct = default)
        {
            EventHandler<T> eventHandler = GetEventHandler<EventHandler<T>>(eventKey);
            if (eventHandler != null)
            {
                await eventHandler.AsyncTrigger(arg1, ct);
            }
        }
        #endregion

        public void Clear()
        {
            foreach (var eventHandler in m_EventHandlers.Values)
            {
                ReferencePool.Release(eventHandler);
            }
            m_EventHandlers.Clear();
            ReferencePool.Release(m_ArrayPool);
        }
    }
}