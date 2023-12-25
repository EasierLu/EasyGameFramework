using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using EGFramework.Runtime.Util;
using Luban;
using System;

namespace Hotfix.Common.Data
{
    public partial class Tables
    {
        public IAssetManager AssetManager;

        /// <summary>
        /// 同步初始化
        /// </summary>
        public void Init()
        {
            var startTime = DateTime.Now;

            Load(Loader);
            AssetManager.UnloadUnused();
            float time = (float)(DateTime.Now - startTime).TotalMilliseconds;
            Log.InfoFormat("配置加载完成...{0}ms", time);
        }

        /// <summary>
        /// 异步初始化加载
        /// </summary>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        public async UniTask InitAsync()
        {
            var startTime = DateTime.Now;

            await LoadAsync(AsyncLoader);
            AssetManager.UnloadUnused();
            float time = (float)(DateTime.Now - startTime).TotalMilliseconds;
            Log.InfoFormat("配置加载完成...{0}ms", time);
        }

        /// <summary>
        /// 同步重新加载
        /// </summary>
        public void Reload()
        {
            Dispose();
            GC.Collect();
            Init();
        }

        /// <summary>
        /// 异步重新加载
        /// </summary>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        public async UniTask ReloadAsync(IProgress<float> progress = null)
        {
            Dispose();
            GC.Collect();
            await InitAsync();
        }

        private async UniTask<ByteBuf> AsyncLoader(string name)
        {
            string path = ToAssetPath(name);
            var bytes = await AssetManager.LoadRawFileAsync(path, "RawFile");

            return new ByteBuf(bytes);
        }

        private ByteBuf Loader(string name)
        {
            string path = ToAssetPath(name);
            var bytes = AssetManager.LoadRawFile(path, "RawFile");

            return new ByteBuf(bytes);
        }

        private string ToAssetPath(string name)
        {
            return $"Assets/AssetsPackage/Data/{name}.bytes";
        }
    }
}
