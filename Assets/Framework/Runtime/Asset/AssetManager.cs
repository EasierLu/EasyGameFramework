using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime
{
    public class AssetManager : IAssetManager
    {
        public string readOnlyPath { get; set; }
        public string readWritePath { get; set; }
        public string version => m_Package.GetPackageVersion();
        public AssetMode assetMode { get; set; }
        public string updataUrl { get; set; }

        private ResourcePackage m_Package;
        private Dictionary<string, AssetHandle> m_AssetHandle = new Dictionary<string, AssetHandle>();
        private Dictionary<string, AllAssetsHandle> m_AllAssetHandle = new Dictionary<string, AllAssetsHandle>();
        private Dictionary<string, RawFileHandle> m_RawFileHandle = new Dictionary<string, RawFileHandle>();

        public async UniTask Initialize()
        {
            await InitializeYooAsset();
        }

        private async UniTask InitializeYooAsset()
        {
#if !UNITY_EDITOR
            if (assetMode == AssetMode.Editor)
            {
                assetMode = AssetMode.Offline;
            }
#endif
            YooAssets.Initialize();
            m_Package = YooAssets.CreatePackage("DefaultPackage");
            YooAssets.SetDefaultPackage(m_Package);

            var initParameters = GetYooAssetInitializeParameters();
            var initOperation = m_Package.InitializeAsync(initParameters);
            await initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            }
        }

        private InitializeParameters GetYooAssetInitializeParameters()
        {
            if (assetMode == AssetMode.Editor)
            {
#if UNITY_EDITOR
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.ScriptableBuildPipeline, "DefaultPackage");
                Debug.Log(initParameters.SimulateManifestFilePath);
                return initParameters;
#else   
                return null;
#endif
            }
            else if (assetMode == AssetMode.HostPlay)
            {
                var initParameters = new HostPlayModeParameters();
                //initParameters.QueryServices = new GameQueryServices();
                //initParameters.DecryptionServices = new GameDecryptionServices();
                //initParameters.RemoteServices = new RemoteServices(updateUrl, fallbackHostServer);
                return initParameters;
            }
            else if (assetMode == AssetMode.WebGL)
            {
                var initParameters = new WebPlayModeParameters();
                //initParameters.QueryServices = new GameQueryServices(); //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
                //initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                return initParameters;
            }
            else
            {
                var initParameters = new OfflinePlayModeParameters();
                return initParameters;
            }
        }

        public Object[] LoadAllAasset(string path)
        {
            //if (m_AllAssetHandle.ContainsKey(path))
            //{
            //    return m_AllAssetHandle[path].AllAssetObjects;
            //}
            //AllAssetsHandle handle = m_Package.LoadAllAssetsSync(path);
            //m_AllAssetHandle.Add(path, handle);

            //return handle.AllAssetObjects;
            throw new System.NotImplementedException();
        }

        public async UniTask<Object[]> LoadAllAssetAsync(string path)
        {
            return await LoadAllAssetAsync<Object>(path);
        }

        public async UniTask<Object[]> LoadAllAssetAsync<T>(string path)
        {
            AllAssetsHandle handle;
            if (m_AllAssetHandle.ContainsKey(path))
            {
                handle = m_AllAssetHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                }
                return handle.AllAssetObjects;
            }

            handle = m_Package.LoadAllAssetsAsync(path);
            m_AllAssetHandle.Add(path, handle);
            await handle;
            return handle.AllAssetObjects;
        }

        public T LoadAsset<T>(string path) where T : Object
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
            T asset;
            if (m_AssetHandle.ContainsKey(path))
            {
                var handle = m_AssetHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                }
                asset = m_AssetHandle[path].AssetObject as T;
            }
            else
            {
                var assetHandle = m_Package.LoadAssetAsync(path);
                m_AssetHandle.Add(path, assetHandle);
                await assetHandle;
                asset = assetHandle.AssetObject as T;
            }
            
            return asset;
        }

        public byte[] LoadRawFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<byte[]> LoadRawFileAsync(string path)
        {
            byte[] asset;
            if (m_RawFileHandle.ContainsKey(path))
            {
                var handle = m_RawFileHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                }
                asset = handle.GetRawFileData();
            }
            else
            {
                var handle = m_Package.LoadRawFileAsync(path);
                m_RawFileHandle.Add(path, handle);
                await handle;
                asset = handle.GetRawFileData();
            }

            return asset;
        }

        public UniTask LoadSceneAsync()
        {
            throw new System.NotImplementedException();
        }

        UniTaskVoid IAssetManager.Initialize()
        {
            throw new System.NotImplementedException();
        }

        public async UniTaskVoid ReleaseAsset(string path)
        {
            if(m_AssetHandle.ContainsKey(path))
            {
                var handle = m_AssetHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                    await UniTask.Delay(100);
                }
                m_AssetHandle[path].Release();
                m_AssetHandle.Remove(path);
            }
        }

        public async UniTaskVoid ReleaseRawFile(string path)
        {
            if (m_RawFileHandle.ContainsKey(path))
            {
                var handle = m_RawFileHandle[path];
                if (!handle.IsDone)
                {
                    await handle;
                    await UniTask.Delay(100);
                }
                handle.Release();
                m_RawFileHandle.Remove(path);
            }
        }

        public void UnloadAssets()
        {
            throw new System.NotImplementedException();
        }
    }
}
