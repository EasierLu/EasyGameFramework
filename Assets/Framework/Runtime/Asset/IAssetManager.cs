using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EGFramework.Runtime
{
    public interface IAssetManager
    {
        public string readOnlyPath { get; set; }
        public string readWritePath { get; set; }
        public AssetMode assetMode { get; set; }
        public string updataUrl { get; set; }
        public string version { get; }

        UniTask<Scene> LoadSceneAsync(string path, LoadSceneMode sceneMode, bool active = false, string packageName = null, uint priority = 0);

        T LoadAsset<T>(string path, string packageName = null) where T : Object;

        UniTask<T> LoadAssetAsync<T>(string path, string packageName = null, uint priority = 100) where T : Object;

        byte[] LoadRawFile(string path, string packageName = null);

        UniTask<byte[]> LoadRawFileAsync(string path, string packageName = null, uint priority = 100);

        void Unload(string path, string packageName = null);

        void TryUnloadUnused(string path, string packageName = null);

        void UnloadUnused();
    }
}
