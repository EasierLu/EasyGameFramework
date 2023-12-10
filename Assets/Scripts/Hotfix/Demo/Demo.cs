using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public static AssetManager AssetManager;

    void Start()
    {
        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        await InitAsset();
    }

    private async UniTask InitAsset()
    {
        AssetManager = new AssetManager();
        await AssetManager.Initialize();

        var go = await AssetManager.LoadAssetAsync<GameObject>("Assets/AssetsPackage/UI/Prefab/Test/Image.prefab");
        Instantiate(go);
    }
}
