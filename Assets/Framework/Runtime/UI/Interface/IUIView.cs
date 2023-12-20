using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.UI
{
    public interface IUIView
    {
        public UIWindow Window { get;}

        public UniTask Init();

        protected void SetData(params object[] data);
    }
}
