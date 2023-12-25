using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Util;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public partial class PackageHandler
    {
        private Dictionary<string, AssetHandle> m_AssetHandle = new Dictionary<string, AssetHandle>();

        #region LoadAsset
        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            AssetHandle assetHandle = GetAssetHandleCache(assetName);
            if(assetHandle == null)
            {
                assetHandle = m_Package.LoadAssetSync<T>(assetName);
            }
            
            if (assetHandle.Status == EOperationStatus.Succeed)
            {
                T asset = assetHandle.AssetObject as T;
                m_AssetHandle.Add(assetName, assetHandle);
                return asset;
            }
            else
            {
                if (assetHandle.Status == EOperationStatus.Processing)
                {
                    Log.ErrorFormat("资源包{0}加载资源{1}失败，尝试加载的资源处于异步加载中无法使用同步立即加载...", m_PackageName, assetName);
                }
                else
                {
                    Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, assetName);
                }
                return null;
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName, uint priority = 1000) where T : UnityEngine.Object
        {
            AssetHandle assetHandle = GetAssetHandleCache(assetName);
            if (assetHandle == null)
            {
                assetHandle = m_Package.LoadAssetAsync<T>(assetName);
                m_AssetHandle.Add(assetName, assetHandle);
            }
            await assetHandle;
            if (assetHandle.Status == EOperationStatus.Succeed)
            {
                return assetHandle.AssetObject as T;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, assetName);
                return null;
            }
        }

        private AssetHandle GetAssetHandleCache(string assetName)
        {
            if (m_AssetHandle.ContainsKey(assetName))
            {
                return m_AssetHandle[assetName];
            }
            else
            {
                return null;
            }
        }

        /*
        public Object[] LoadAllAsset(string path)
        {
            AllAssetsHandle handle;
            if (m_AllAssetHandle.ContainsKey(path))
            {
                handle = m_AllAssetHandle[path];
            }
            else
            {
                handle = m_Package.LoadAllAssetsSync(path);
                m_AllAssetHandle.Add(path, handle);
            }

            if (handle.Status == EOperationStatus.Succeed)
            {
                return handle.AllAssetObjects;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载全部资源{1}失败", m_PackageName, path);
                return null;
            }
        }

        public async UniTask<Object[]> LoadAllAssetAsync(string path)
        {
            AllAssetsHandle handle;
            if (m_AllAssetHandle.ContainsKey(path))
            {
                handle = m_AllAssetHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                }
            }
            else
            {
                handle = m_Package.LoadAllAssetsAsync(path);
                m_AllAssetHandle.Add(path, handle);
                await handle;
            }
            
            if (handle.Status == EOperationStatus.Succeed)
            {
                return handle.AllAssetObjects;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载全部资源{1}失败", m_PackageName, path);
                return null;
            }
        }
        */
        #endregion

        public void UnloadAsset(string assetName)
        {
            if (m_AssetHandle.ContainsKey(assetName))
            {
                m_AssetHandle[assetName].Release();
                m_AssetHandle.Remove(assetName);
            }
            else
            {
                Debug.LogWarningFormat("尝试释放的资源不再缓存中...");
            }
        }
    }
}
