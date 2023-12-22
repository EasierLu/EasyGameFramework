using EGFramework.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public abstract class UIView : MonoBehaviour
    {
        public int id { get; private set; }
        public UIWindow window { get; private set; }

        public void Init(UIWindow window, int id)
        {
            this.window = window;
            this.id = id;
            InitComponent();
        }

        protected abstract void InitComponent();

        public void SetData(params object[] data)
        {
            OnDataChanged(data);
        }

        protected abstract void OnDataChanged(params object[] data);

        public void Release()
        {
            id = -1;
            window = null;
        }

        protected virtual void OnRelease()
        { }
    }
}
