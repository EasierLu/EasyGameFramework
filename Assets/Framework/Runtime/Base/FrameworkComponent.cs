using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Base
{
    public class FrameworkComponent : MonoBehaviour
    {
        public virtual int Priority => FrameworkComponentPriority.Custom;

        public virtual async UniTask Init() 
        {
            await UniTask.Yield();
        }

        public virtual void ComponentUpdate(float elapseSeconds, float realElapseSeconds) { }

        public virtual async UniTask Shutdown()
        { 
            await UniTask.Yield();
        }

        public T GetFrameworkComponent<T>() where T : FrameworkComponent
        { 
            return FrameworkCore.GetComponent<T>();
        }
    }
}
