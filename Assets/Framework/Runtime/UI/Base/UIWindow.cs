using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        public abstract string UIName { get; }
        public abstract UILayer Layer { get; }

        protected IUIGroup m_Group;
        public IUIGroup Group { get => m_Group; set => m_Group = value; }

        protected IUIView m_View;
        protected Dictionary<string, IUIView> m_DynamicView;

        protected abstract void InitComponent();

        public virtual async UniTask Init()
        {
            InitComponent();
            await OnInit();
        }

        protected abstract UniTask OnInit();

        public async UniTask<T> LoadDynamicView<T>(string prefabPath, RectTransform parent)where T: IUIView
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseDynamicView(string prefabPath)
        { 
            
        }
    }
}
