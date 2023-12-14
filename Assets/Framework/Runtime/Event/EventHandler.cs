using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Pool;
using EGFramework.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EGFramework.Runtime.Event
{
    public class EventHandler<T> : IEventHandler
    {
        public string EventKey { get; set; }

        public CappedArrayPool<UniTask> ArrayPool { get; set; }

        private List<Func<T, CancellationToken, UniTask>> m_AsyncEvents = new List<Func<T, CancellationToken, UniTask>>();
        private Action<T> m_SyncEvent;

        public int Count => m_AsyncEvents.Count + m_SyncEvent.GetInvocationList().Length;

        public void Add(Action<T> handle)
        {
            m_SyncEvent += handle;
        }

        public void Add(Func<T, CancellationToken, UniTask> handle)
        {
            if (ArrayPool.Length < m_AsyncEvents.Count)
            {
                Log.Fatal($"Can not add async event,array pool is not enough.[{EventKey}]");
                return;
            }

            m_AsyncEvents.Add(handle);
        }

        public void Remove(Action<T> handle)
        {
            m_SyncEvent -= handle;
        }

        public void Remove(Func<T, CancellationToken, UniTask> handle)
        {
            m_AsyncEvents.Remove(handle);
        }

        public void Trigger(T msg, CancellationToken ct = default)
        {
            TriggerSyncEvent(msg);
            TriggerAsyncEvent(msg, ct).Forget();
        }

        public UniTask AsyncTrigger(T msg, CancellationToken ct = default)
        {
            TriggerSyncEvent(msg);
            return TriggerAsyncEvent(msg, ct);
        }

        private void TriggerSyncEvent(T msg)
        {
            try
            {
                m_SyncEvent?.Invoke(msg);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private async UniTask TriggerAsyncEvent(T msg, CancellationToken ct = default)
        {
            UniTask[] buffer;

            lock (m_AsyncEvents)
            {
                buffer = ArrayPool.Rent(m_AsyncEvents.Count);
                for (var i = 0; i < m_AsyncEvents.Count; i++)
                {
                    buffer[i] = m_AsyncEvents[i](msg, ct);
                }
            }
            try
            {
                await UniTask.WhenAll(buffer);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            finally
            {
                ArrayPool.Return(buffer);
            }
        }

        public void Clear()
        {
            m_AsyncEvents.Clear();
            ArrayPool = null;
            m_SyncEvent = null;
        }
    }

    public class EventHandler : IEventHandler
    {
        public string EventKey { get; set; }

        public CappedArrayPool<UniTask> ArrayPool { get; set; }

        private List<Func<CancellationToken, UniTask>> m_AsyncEvents = new List<Func<CancellationToken, UniTask>>();
        private Action m_SyncEvent;

        public int Count => m_AsyncEvents.Count + m_SyncEvent.GetInvocationList().Length;

        public void Add(Action handle)
        {
            m_SyncEvent += handle;
        }

        public void Add(Func<CancellationToken, UniTask> handle)
        {
            if (ArrayPool.Length < m_AsyncEvents.Count)
            {
                Log.Fatal($"Can not add async event,array pool is not enough.[{EventKey}]");
                return;
            }

            m_AsyncEvents.Add(handle);
        }

        public void Remove(Action handle)
        {
            m_SyncEvent -= handle;
        }

        public void Remove(Func<CancellationToken, UniTask> handle)
        {
            m_AsyncEvents.Remove(handle);
        }

        public void Trigger(CancellationToken ct = default)
        {
            TriggerSyncEvent();
            TriggerAsyncEvent(ct).Forget();
        }

        public UniTask AsyncTrigger(CancellationToken ct = default)
        {
            TriggerSyncEvent();
            return TriggerAsyncEvent(ct);
        }

        private void TriggerSyncEvent()
        {
            try
            {
                m_SyncEvent?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private async UniTask TriggerAsyncEvent(CancellationToken ct = default)
        {
            UniTask[] buffer;

            lock (m_AsyncEvents)
            {
                buffer = ArrayPool.Rent(m_AsyncEvents.Count);
                for (var i = 0; i < m_AsyncEvents.Count; i++)
                {
                    buffer[i] = m_AsyncEvents[i](ct);
                }
            }
            try
            {
                await UniTask.WhenAll(buffer);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            finally
            {
                ArrayPool.Return(buffer);
            }
        }

        public void Clear()
        {
            m_AsyncEvents.Clear();
            ArrayPool = null;
            m_SyncEvent = null;
        }
    }
}
