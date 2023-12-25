using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Pool;
using EGFramework.Runtime.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class GameObjectPool : IReference
    {
        public string PoolName { get; private set; }
        private string m_PackageName;
        private Transform m_PoolRoot;
        private IAssetManager m_AssetManager;
        private Dictionary<string, GameObject> m_OriginObjects;
        private Dictionary<string, Queue<GameObject>> m_GameObjects;
        private Dictionary<string, int> m_ReferenceNum;

        public GameObjectPool() { }

        public GameObjectPool(IAssetManager assetManager, Transform poolRoot, string package = null, int capacity = 8)
        {
            Init(assetManager, poolRoot, package, capacity);
        }

        public void Init(IAssetManager assetManager, Transform poolRoot, string package = null, int capacity = 8)
        {
            m_PackageName = package;
            m_PoolRoot = poolRoot;
            m_AssetManager = assetManager;
            m_OriginObjects = new Dictionary<string, GameObject>(capacity);
            m_GameObjects = new Dictionary<string, Queue<GameObject>>(capacity);
            m_ReferenceNum = new Dictionary<string, int>(capacity);
        }

        public void Clear()
        {
            DestroyAll();
            foreach (var item in m_OriginObjects.Keys)
            {
                m_AssetManager.Unload(item);
            }
            m_OriginObjects.Clear();
            m_GameObjects.Clear();
            m_ReferenceNum.Clear();
            m_AssetManager = null;
            m_PoolRoot = null;
            m_PackageName = null;
        }

        public async UniTask PreloadAsync(string path, int num)
        {
            GameObject origin = await GetOriginAsync(path);
            for (int i = 0; i < num; i++) 
            {
                var go = Object.Instantiate(origin, m_PoolRoot);
                AddToCache(path, go);
            }
        }

        public GameObject Get(string path, Transform parent = null)
        {
            GameObject go;
            if (!TryGetFormCache(path, out go))
            {
                go = Create(path, parent);
                AddReferenceNum(path);
            }
            else
            {
                SetGameObjectStatus(go, parent);
            }

            return go;
        }

        public async UniTask<GameObject> GetAsync(string path, Transform parent = null)
        {
            GameObject go;
            if (!TryGetFormCache(path, out go))
            {
                go = await CreateAsync(path, parent);
                AddReferenceNum(path);
            }
            else
            { 
                SetGameObjectStatus(go, parent);
            }
            return go;
        }

        public bool TryGetFormCache(string path, out GameObject go)
        {
            go = GetFormCache(path);
            return go != null;
        }

        public void Release(string path, GameObject obj)
        {
            AddToCache(path, obj);
            RemoveReferenceNum(path);
        }

        public void Destroy(string path, GameObject obj, bool immediate = false)
        {
            if (immediate)
            {
                Object.DestroyImmediate(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
            var refNum = RemoveReferenceNum(path);
            if (refNum <= 0)
            {
                m_OriginObjects.Remove(path);
                m_AssetManager.Unload(path, m_PackageName);
            }
        }

        public void DestroyByPath(string path, bool immediate = false)
        {
            Queue<GameObject> cachedObj;
            if (m_GameObjects.TryGetValue(path, out cachedObj))
            {
                while (cachedObj.Count > 0)
                {
                    var obj = cachedObj.Dequeue();
                    Destroy(path, obj, immediate);
                }
                m_GameObjects.Remove(path);
            }
        }

        public void DestroyAll()
        {
            var cachedObjPath = m_GameObjects.Keys.ToArray();
            foreach (var path in cachedObjPath)
            {
                DestroyByPath(path);
            }
        }

        public GameObject Create(string path, Transform parent = null)
        {
            GameObject origin;
            if (!m_OriginObjects.TryGetValue(path, out origin))
            {
                origin = m_AssetManager.LoadAsset<GameObject>(path, m_PackageName);
                m_OriginObjects.Add(path, origin);
            }

            parent ??= m_PoolRoot;
            var ins = Object.Instantiate(origin, parent);
            SetGameObjectStatus(ins);
            return ins;
        }

        private async UniTask<GameObject> CreateAsync(string path, Transform parent = null)
        {
            GameObject origin = await GetOriginAsync(path);
            parent ??= m_PoolRoot;
            var ins = Object.Instantiate(origin, parent);
            SetGameObjectStatus(ins);
            return ins;
        }

        private async UniTask<GameObject> GetOriginAsync(string path)
        {
            GameObject origin;
            if (!m_OriginObjects.TryGetValue(path, out origin))
            {
                origin = await m_AssetManager.LoadAssetAsync<GameObject>(path, m_PackageName);
                m_OriginObjects.Add(path, origin);
            }
            return origin;
        }

        private void AddToCache(string path, GameObject go)
        {
            if (go.activeSelf)
            {
                go.SetActive(false);
            }
            if (go.transform.parent != m_PoolRoot)
            {
                go.transform.SetParent(m_PoolRoot);
            }

            if (!m_GameObjects.ContainsKey(path))
            {
                m_GameObjects.Add(path, new Queue<GameObject>());
            }
            m_GameObjects[path].Enqueue(go);
        }

        private GameObject GetFormCache(string path)
        {
            GameObject go = null; ;
            if (m_GameObjects.ContainsKey(path))
            {
                go = m_GameObjects[path].Dequeue();
                AddReferenceNum(path);
            }
            return go;
        }

        private void AddReferenceNum(string path, int num = 1)
        {
            if (!m_ReferenceNum.ContainsKey(path))
            {
                m_ReferenceNum.Add(path, num);
            }
            else
            {
                m_ReferenceNum[path] += num;
            }
        }

        private int RemoveReferenceNum(string path, int num = 1)
        {
            if (m_ReferenceNum.ContainsKey(path))
            {
                int curNum = m_ReferenceNum[path];
                curNum -= num;

                if (curNum > 0)
                {
                    m_ReferenceNum[path] = curNum;
                }
                else if (curNum == 0)
                {
                    m_ReferenceNum.Remove(path);
                }
                else
                {
                    Log.ErrorFormat("Reference num error: {0} {1}", curNum, path);
                    m_ReferenceNum.Remove(path);
                }
                return curNum;
            }
            else
            {
                Log.ErrorFormat("Reference num error: {0} doesn't exist.");
                return -1;
            }
        }

        private void SetGameObjectStatus(GameObject go, Transform parent = null)
        {
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            go.transform.localScale = Vector3.one;
        }
    }
}
