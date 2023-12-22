using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class GameObjectPool : FrameworkComponent
    {
        public override int Priority => FrameworkComponentPriority.Asset + 1;

        public override async UniTask Init()
        {
            await base.Init();
        }

        public override void ComponentUpdate(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }

        public override UniTask Shutdown()
        {
            throw new System.NotImplementedException();
        }
    }
}
