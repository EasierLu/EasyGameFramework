using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using EGFramework.Runtime.Base;
using Hotfix.Framework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix.Common
{
    public static class GameCore
    {
        public static IAssetManager AssetManager { get; private set; }
        
        public static async UniTask Initialize()
        {
            AssetManager = await FrameworkCore.AddComponent<AssetManager>();
        }
    }
}
