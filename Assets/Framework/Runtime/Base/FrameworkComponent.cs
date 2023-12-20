using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Base
{
    public abstract class FrameworkComponent : MonoBehaviour
    {
        public virtual int Priority => FrameworkComponentPriority.Custom;

        public virtual async UniTask Init() 
        {
            await UniTask.Yield();
        }

        public abstract void ComponentUpdate(float elapseSeconds, float realElapseSeconds);

        public abstract UniTask Shutdown();
    }
}
