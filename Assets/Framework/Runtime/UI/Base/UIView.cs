using EGFramework.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public abstract class UIView : MonoBehaviour
    {
        public UIWindow Window { get; private set; }

        public void Init(UIWindow window)
        {
            Window = window;
            InitComponent();
        }

        protected abstract void InitComponent();

        public void SetData(params object[] data)
        {
            OnDataChanged(data);
        }

        protected abstract void OnDataChanged(params object[] data);
    }
}
