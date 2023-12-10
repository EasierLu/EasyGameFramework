using Cysharp.Threading.Tasks;

namespace EGFramework.Runtime
{
    public interface IAssetManager
    {
        public string readOnlyPath { get; set; }
        public string readWritePath { get; set; }
        public AssetMode assetMode { get; set; }
        public string updataUrl { get; set; }

        public string version { get; }

        UniTaskVoid Initialize();

        UniTask LoadSceneAsync();

        T LoadAsset<T>(string path) where T : UnityEngine.Object;

        UniTask<T> LoadAssetSync<T>(string path)where T : UnityEngine.Object;

        UnityEngine.Object[] LoadAllAasset(string path);

        UniTask<UnityEngine.Object[]> LoadAllAssetAsync(string path);

        byte[] LoadRawFile(string path);

        UniTask<byte[]> LoadRawFileAsync(string path);
    }
}
