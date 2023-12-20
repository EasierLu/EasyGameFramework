using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public partial class PackageHandler
    {
        private Dictionary<string, SceneHandle> m_ScenecCache = new Dictionary<string, SceneHandle>();

        #region Load Scene
        public async UniTask<Scene> LoadSceneAsync(string location, LoadSceneMode sceneMode, bool active = false, uint priority = 0)
        {
            SceneHandle handle;
            if (sceneMode == LoadSceneMode.Additive && m_ScenecCache.ContainsKey(location))
            {
                handle = m_ScenecCache[location];
            }
            else
            {
                handle = m_Package.LoadSceneAsync(location, sceneMode, !active, priority);
            }
            await handle;
            return handle.SceneObject;
        }
        #endregion

        public async UniTask UnloadScene(string location)
        {
            if (m_ScenecCache.ContainsKey(location))
            {
                var handle = m_ScenecCache[location];
                m_ScenecCache.Remove(location);
                await handle.UnloadAsync();
            }
        }
    }
}
