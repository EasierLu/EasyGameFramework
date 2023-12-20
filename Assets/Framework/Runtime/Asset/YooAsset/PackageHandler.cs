using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Util;
using System;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public partial class PackageHandler : IDisposable
    {
        public string PackageName => m_PackageName;
        public bool Initialized => m_Package.InitializeStatus == EOperationStatus.Succeed;
        public bool IsDefault => m_IsDefault;
        public string Version => m_Package.GetPackageVersion();

        private string m_PackageName;
        private ResourcePackage m_Package;
        private AssetMode m_AssetMode;
        private YooAssetUpdateData m_UpdateInfo;
        private bool m_IsDefault;
        private EDefaultBuildPipeline m_BuildPipeline;

        #region Init & Dispose
        public PackageHandler(string packageName, AssetMode assetMode, bool isDefault = false)
        {
            m_PackageName = packageName;
            m_AssetMode = assetMode;
            m_IsDefault = isDefault;
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new YooAssetLogger());
            }

            m_Package = YooAssets.CreatePackage(m_PackageName);
            if (isDefault)
            {
                YooAssets.SetDefaultPackage(m_Package);
            }
        }

        public async UniTask<bool> Initialize(EDefaultBuildPipeline buildPipeline, YooAssetUpdateData updateData)
        {
            m_BuildPipeline = buildPipeline;
            m_UpdateInfo = updateData;

            var initParameters = GetYooAssetInitializeParameters();
            var initOperation = m_Package.InitializeAsync(initParameters);

            await initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Log.InfoFormat("资源包{0}初始化成功！", m_PackageName);
            }
            else
            {
                Log.ErrorFormat("资源包{0}初始化失败：{1}", m_PackageName, initOperation.Error);
            }

            return initOperation.Status == EOperationStatus.Succeed;
        }

        private InitializeParameters GetYooAssetInitializeParameters()
        {
            if (m_AssetMode == AssetMode.Editor)
            {
#if UNITY_EDITOR
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(m_BuildPipeline, m_PackageName);
                return initParameters;
#else   
                return null;
#endif
            }
            else if (m_AssetMode == AssetMode.HostPlay)
            {
                var initParameters = new HostPlayModeParameters();
                //initParameters.QueryServices = new GameQueryServices();
                //initParameters.DecryptionServices = new GameDecryptionServices();
                //initParameters.RemoteServices = new RemoteServices(updateUrl, fallbackHostServer);
                return initParameters;
            }
            else if (m_AssetMode == AssetMode.WebGL)
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

        public void Dispose()
        {
            m_ScenecCache.Clear();
            YooAssets.DestroyPackage(m_PackageName);
        }
        #endregion

        #region Unload
        public void UnloadAsset(string assetName)
        {
            m_Package.TryUnloadUnusedAsset(assetName);
        }

        public void UnloadUnusedAssets()
        {
            m_Package.UnloadUnusedAssets();
        }
        #endregion
    }

    public struct YooAssetUpdateData
    {
        public string Url;
    }
}
