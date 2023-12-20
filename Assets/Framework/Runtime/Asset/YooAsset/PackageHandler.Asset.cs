using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Util;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public partial class PackageHandler
    {
        #region LoadAsset
        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var loadOperation = m_Package.LoadAssetSync<T>(assetName);
            if (loadOperation.Status == EOperationStatus.Succeed)
            {
                return loadOperation.AssetObject as T;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, assetName);
                return null;
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName, uint priority = 1000) where T : UnityEngine.Object
        {
            var loadOperation = m_Package.LoadAssetAsync<T>(assetName, priority);
            await loadOperation;
            if (loadOperation.Status == EOperationStatus.Succeed)
            {
                return loadOperation.AssetObject as T;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, assetName);
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
    }
}
