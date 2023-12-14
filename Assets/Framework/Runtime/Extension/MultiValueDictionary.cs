using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

namespace EGFramework.Runtime.Extension
{
    /// <summary>
    /// 多值字典
    /// </summary>
    /// <typeparam name="TKey"> key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    public class MultiValueDictionary<TKey, TValue> : IEnumerable
    {
        private Dictionary<TKey, List<TValue>> m_Dictionary;
        private int m_ValueCapacity = 4;

        public int Count => m_Dictionary.Count;

        public Dictionary<TKey, List<TValue>>.KeyCollection Keys => m_Dictionary.Keys;

        public Dictionary<TKey, List<TValue>>.ValueCollection Values => m_Dictionary.Values;

        public List<TValue> this[TKey key]
        {
            get
            {
                if (m_Dictionary.ContainsKey(key))
                {
                    return m_Dictionary[key];
                }
                return null;
            }
        }

        public MultiValueDictionary(int keyCapacity = 4, int valueCapacity = 4)
        {
            m_Dictionary = new Dictionary<TKey, List<TValue>>(keyCapacity);
            m_ValueCapacity = valueCapacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (!m_Dictionary.ContainsKey(key))
            {
                m_Dictionary.Add(key, new List<TValue>(m_ValueCapacity));
            }
            m_Dictionary[key].Add(value);
        }

        public void Add(TKey key, IEnumerable<TValue> values)
        {
            if (!m_Dictionary.ContainsKey(key))
            {
                m_Dictionary.Add(key, new List<TValue>(m_ValueCapacity));
            }
            m_Dictionary[key].AddRange(values);
        }

        public void Remove(TKey key, TValue value)
        {
            if (m_Dictionary.ContainsKey(key))
            {
                m_Dictionary[key].Remove(value);
            }
        }

        public void Remove(TKey key)
        {
            if (m_Dictionary.ContainsKey(key))
            {
                m_Dictionary.Remove(key);
            }
        }

        public void RemoveAt(TKey key, int index)
        {
            if (m_Dictionary.ContainsKey(key))
            {
                m_Dictionary[key].RemoveAt(index);
            }
        }

        public void Clear()
        {
            m_Dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TKey key, TValue value)
        {
            if (m_Dictionary.ContainsKey(key))
            {
                return m_Dictionary[key].Contains(value);
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }
    }
}
