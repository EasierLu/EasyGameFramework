using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Base;
using EGFramework.Runtime.Util;
using EGFramework.Runtime.YooAsset;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace EGFramework.Runtime.Asset
{
    public class AssetManager : FrameworkComponent, IAssetManager
    {
        public string readOnlyPath { get; set; }
        public string readWritePath { get; set; }
        public string version => DefaultPackage.Version;
        public AssetMode assetMode { get; set; }
        public string updataUrl { get; set; }

        private PackageHandler DefaultPackage => m_Packages[m_DefaultPackageName];
        private Dictionary<string, PackageHandler> m_Packages;
        private string m_DefaultPackageName;

        #region Init & Dispose
        public override async UniTask Init()
        {
            var setting = Setting.FrameworkSettingUtil.GlobalSettings;
            m_DefaultPackageName = setting.defaultAssetsPackage;
            m_Packages = new Dictionary<string, PackageHandler>(setting.assetsPackageInfo.Count);
            assetMode = Application.isEditor ? AssetMode.Editor : setting.runtimeAssetMode;

            YooAssets.Initialize(new YooAssetLogger());

            foreach (var item in setting.assetsPackageInfo)
            {
                await AddPackage(item.name, item.buildPipeline, default);
            }
        }

        public async UniTask AddPackage(string packageName, EDefaultBuildPipeline buildPipeline, YooAssetUpdateData updateData)
        { 
            if(m_Packages.ContainsKey(packageName))
            {
                Log.ErrorFormat("资源包{0}已存在！", packageName);
                return;
            }
            var packageHandler = new PackageHandler(packageName, assetMode);
            bool success = await packageHandler.Initialize(buildPipeline, updateData);
            if (success)
            {
                m_Packages.Add(packageName, packageHandler);
            }
        }

        public void RemovePackage(string packageName)
        {
            if (m_Packages.ContainsKey(packageName))
            {
                m_Packages[packageName].Dispose();
                m_Packages.Remove(packageName);
            }
        }

        public override void ComponentUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        public override async UniTask Shutdown()
        {
            foreach (var item in m_Packages)
            {
                item.Value.Dispose();
            }
            await UniTask.Yield();
        }
        #endregion

        private PackageHandler GetPackageHandler(string packageName)
        {
            if (packageName == null)
            {
                packageName = m_DefaultPackageName;
            }

            if (m_Packages.ContainsKey(packageName))
            {
                return m_Packages[packageName];
            }
            else
            {
                throw new FrameworkException($"不存在资源包{packageName}");
            }
        }

        public async UniTask<Scene> LoadSceneAsync(string path, LoadSceneMode sceneMode, bool active = false, string packageName = null, uint priority = 0)
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                return await packageHandler.LoadSceneAsync(path, sceneMode, active, priority);
            }

            return default;
        }

        public T LoadAsset<T>(string path, string packageName = null) where T : Object
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                return packageHandler.LoadAsset<T>(path);
            }

            return null;
        }

        public async UniTask<T> LoadAssetAsync<T>(string path, string packageName = null, uint priority = 100) where T : Object
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                return await packageHandler.LoadAssetAsync<T>(path, priority);
            }

            return null;
        }

        public byte[] LoadRawFile(string path, string packageName = null)
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                return packageHandler.LoadRawFile(path);
            }

            return null;
        }

        public async UniTask<byte[]> LoadRawFileAsync(string path, string packageName = null, uint priority = 100)
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                return await packageHandler.LoadRawFileAsync(path, priority);
            }

            return null;
        }

        public void Unload(string path, string packageName = null)
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                packageHandler.UnloadAsset(path);
            }
        }

        public void TryUnloadUnused(string path, string packageName = null)
        {
            var packageHandler = GetPackageHandler(packageName);
            if (packageHandler != null)
            {
                packageHandler.TryUnloadUnusedAsset(path);
            }
        }

        public void UnloadUnused()
        {
            foreach (var item in m_Packages)
            {
                item.Value.UnloadUnusedAssets();
            }
        }

    }
}
