using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using Luban;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 异步初始化加载
        /// </summary>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        public async UniTask InitAsync(IProgress<float> progress = null)
        {
            await LoadAsync(AsyncLoader, progress);
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
            var asset = await AssetManager.LoadAssetAsync<TextAsset>(path);
            var bytes = asset.bytes;
            AssetManager.ReleaseAsset(path);
            return new ByteBuf(bytes);
        }

        private ByteBuf Loader(string name)
        {
            throw new System.NotImplementedException();
        }

        private string ToAssetPath(string name)
        {
            return $"Assets/AssetsPackage/Data/{name}.bytes";
        }
    }
}
