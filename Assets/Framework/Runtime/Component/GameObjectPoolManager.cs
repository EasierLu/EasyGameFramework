using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Asset;
using EGFramework.Runtime.Base;
using EGFramework.Runtime.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class GameObjectPoolManager : FrameworkComponent
    {
        private IAssetManager m_AssetManager;

        private Dictionary<string, GameObjectPool> m_Pools;

        public override async UniTask Init()
        {
            m_AssetManager = GetFrameworkComponent<AssetManager>();
            m_Pools = new Dictionary<string, GameObjectPool>();
            await base.Init();
        }

        public override async UniTask Shutdown()
        {
            foreach (var pool in m_Pools.Values)
            {
                ReferencePool.Release(pool);
            }
            m_Pools.Clear();
            await base.Shutdown();
        }

        public GameObjectPool GetPool(string name)
        {
            
            return CreatePool(name);
        }

        public GameObjectPool CreatePool(string name, string package = null)
        {
            if (!m_Pools.ContainsKey(name))
            {
                var pool = ReferencePool.Acquire<GameObjectPool>();
                GameObject root = new GameObject(name);
                root.transform.SetParent(transform);
                root.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                root.transform.localScale = Vector3.one;

                pool.Init(m_AssetManager, root.transform, package);
                m_Pools.Add(name, pool);
            }

            return m_Pools[name];
        }

        public void RemovePool(string name)
        {
            if(m_Pools.ContainsKey(name)) 
            {
                var pool = m_Pools[name];
                ReferencePool.Release(pool);
                m_Pools.Remove(name);
            }
        }
    }
}
