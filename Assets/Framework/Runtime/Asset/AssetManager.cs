using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using static UnityEngine.UI.AspectRatioFitter;

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
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("ScriptableBuildPipeline", "DefaultPackage");
                return initParameters;
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
            throw new System.NotImplementedException();
        }

        public UniTask<Object[]> LoadAllAssetAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public T LoadAsset<T>(string path) where T : Object
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
            var assetHandle = m_Package.LoadAssetAsync(path);
            await assetHandle;
            return assetHandle.AssetObject as T;
        }

        public byte[] LoadRawFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<byte[]> LoadRawFileAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public UniTask LoadSceneAsync()
        {
            throw new System.NotImplementedException();
        }

        UniTaskVoid IAssetManager.Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
