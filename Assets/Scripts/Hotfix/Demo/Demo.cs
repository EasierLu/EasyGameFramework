using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using EGFramework.Runtime.Base;
using EGFramework.Runtime.Util;
using Hotfix.Common.Data;
using Hotfix.Framework.Runtime;
using System.Threading;
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
        FrameworkCore.Init();
        await FrameworkCore.AddComponent<BaseComponent>();
        AssetManager = await FrameworkCore.AddComponent<AssetManager>();
        //await InitAsset();
        await InitTable();
        //TestWindow window = new TestWindow();
        //Debug.Log(TestWindow.PrefabPath);

        //string key = "1234567890123456";
        //var result = Utility.Security.AESEncryption(Encoding.UTF8.GetBytes("123456"), key, key);
        //Debug.Log(Encoding.UTF8.GetString(result));

        //Debug.Log(Encoding.UTF8.GetString(Utility.Security.AESDecryption(result, key, key)));

        //string key2 = "2";
        //EventController e = new EventController();
        //e.AddEventListener(key, AsyncFunc);
        //e.AddEventListener(key, Func);
        //e.AddEventListener<int>(key2, AsyncFunc2);
        //e.AddEventListener<int>(key2, Func2);
        //e.AddEventListener<int>(key2, AsyncFunc3);

        //await e.AsyncTriggerEvent(key);
        //await e.AsyncTriggerEvent(key2, 1);

        //e.RemoveEventListener(key, AsyncFunc);
        //e.TriggerEvent(key);

        //e.RemoveEventListener<int>(key2, Func2);
        //e.TriggerEvent(key2, 3);

        //Log.Info("TriggerEvent Success");

        
    }

    private async UniTask InitAsset()
    {
        //AssetManager = new AssetManager();
        //await AssetManager.Initialize();

        //var go = await AssetManager.LoadAssetAsync<GameObject>("Assets/AssetsPackage/UI/Prefab/Test/Image.prefab");
        //Instantiate(go);
    }

    private async UniTask InitTable()
    {
        Config = new Tables();
        Config.AssetManager = AssetManager;
        await UniTask.Delay(1000);
        //Config.Init();
        await Config.InitAsync();
        //for (int i = 0; i < 10; i++)
        //{
        //    await Config.ReloadAsync();
        //    await UniTask.Delay(500);
        //}

        Log.Info(Config.TbGlobalConfig.BagMax);
    }

    private async UniTask AsyncFunc(CancellationToken ct)
    {
        for (int i = 0; i < 10; i++)
        {
            await UniTask.Delay(500);
            Log.Info("AsyncFunc1:" + i);
        }
    }

    private void Func()
    {
        Log.Info("Func1");
    }

    private async UniTask AsyncFunc2(int offset, CancellationToken ct)
    {
        for (int i = 0; i < 10; i++)
        {
            await UniTask.Delay(500);
            Log.Info("AsyncFunc2:" + (i + offset));
        }
    }

    private void Func2(int offset)
    {
        Log.Info("Func2:" + offset);
    }

    private async UniTask AsyncFunc3(int offset, CancellationToken ct)
    {
        for (int i = 0; i < 10; i++)
        {
            await UniTask.Delay(500);
            Log.Info("AsyncFunc3:" + (i + offset));
        }
    }
}
