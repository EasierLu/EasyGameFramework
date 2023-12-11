using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using EGFramework.Runtime.Util;
using Hotfix.Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public static AssetManager AssetManager;
    public static Tables Config;

    void Start()
    {
        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        await InitAsset();
        await UniTask.Delay(2000);
        await InitTable();
    }

    private async UniTask InitAsset()
    {
        AssetManager = new AssetManager();
        await AssetManager.Initialize();

        var go = await AssetManager.LoadAssetAsync<GameObject>("Assets/AssetsPackage/UI/Prefab/Test/Image.prefab");
        Instantiate(go);
    }

    private async UniTask InitTable()
    {
        Config = new Tables();
        Config.AssetManager = AssetManager;
        await Config.InitAsync();
        //for (int i = 0; i < 10; i++)
        //{
        //    await Config.ReloadAsync();
        //    await UniTask.Delay(500);
        //}

        Log.Info(Config.TbGlobalConfig.BagMax);
    }
}
