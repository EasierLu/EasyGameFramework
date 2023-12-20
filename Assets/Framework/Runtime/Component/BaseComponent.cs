using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Base;
using EGFramework.Runtime.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class BaseComponent : FrameworkComponent
    {
        public override int Priority => FrameworkComponentPriority.Base;

        public override UniTask Init()
        {
            Log.Info($"Framework version: {Version.FrameworkVersion}");
            return base.Init();
        }

        private void Update()
        {
            FrameworkCore.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        public override void ComponentUpdate(float elapseSeconds, float realElapseSeconds)
        {
            return;
        }

        public override async UniTask Shutdown()
        {
            await UniTask.Yield();
        }
    }
}
