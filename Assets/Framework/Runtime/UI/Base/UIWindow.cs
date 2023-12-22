using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Util;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGFramework.Runtime.UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        public abstract UILayer layer { get; }

        protected UIManager m_UIManager;
        protected UIWindow m_ParentWindow;
        protected Dictionary<int, UIView> m_SubView;
        protected Dictionary<string, UIWindow> m_SubWindow;

        #region 生命周期
        protected abstract void InitComponent();

        public virtual async UniTask Init(UIManager manager, UIWindow parentWindow)
        {
            m_UIManager = manager;

            m_SubView = new Dictionary<int, UIView>();
            m_SubWindow = new Dictionary<string, UIWindow>();

            InitComponent();
            await OnInit();
        }

        protected abstract UniTask OnInit();

        public void SetData(params object[] data)
        {
            OnDataChanged(data);
        }

        protected virtual void OnDataChanged(params object[] data) { }

        private void OnEnable()
        {
            OnWindowEnable();
        }

        protected virtual void OnWindowEnable()
        { 
        }

        private void OnDisable()
        {
            ReleaseAllDynamicSubView();
            OnWindowDisable();
        }

        protected virtual void OnWindowDisable()
        {
        }

        private void OnDestroy()
        {
        }

        public void Release()
        {
            ReleaseAllDynamicSubView();
            OnWindowRelease();
        }

        protected virtual void OnWindowRelease()
        {
            
        }
        #endregion

        #region SubView
        protected async UniTask<T> LoadDynamicSubView<T>(RectTransform parent) where T : UIView
        {
            Type type = typeof(T);
            var attribute = type.GetAttribute<UIViewAttribute>();
            if (attribute != null)
            {
                var gameObject = await m_UIManager.LoadGameObjectAsync(attribute.path, parent);
                var view = gameObject.GetComponent<T>();
                if (view == null)
                {
                    view = gameObject.AddComponent<T>();
                }
                int id = m_UIManager.GetIncrementId();
                view.Init(this, id);
                view.transform.name = Utility.Text.Format("{0}_{1}", type.Name, id);
                m_SubView.Add(id, view);
                return view;
            }
            else
            { 
                Log.ErrorFormat("Load dynamic sub view error, type:{0} is not shared sub view.(Need add attribute 'UIView(prefabPath)')", type.Name);
                return null;
            }
        }

        protected void ReleaseDynamicSubView(int id)
        {
            if(m_SubView.TryGetValue(id, out var view))
            {
                view.Release();
                m_SubView.Remove(id);
            }
            else
            {
                Log.ErrorFormat("Release shared sub view error, id:{0} is not exist.", id);
            }
        }

        protected void ReleaseDynamicSubView<T>(T view) where T : UIView
        {
            if (view != null)
            {
                ReleaseDynamicSubView(view.id);
            }
            else
            {
                Log.ErrorFormat("Release shared sub view error, view is null.");
            }
        }

        protected void ReleaseAllDynamicSubView()
        {
            if (m_SubView != null)
            {
                var subViewIds = m_SubView.Keys.ToArray();
                foreach (var id in subViewIds)
                {
                    ReleaseDynamicSubView(id);
                }
                m_SubView.Clear();
                m_SubView = null;
            }
        }
        #endregion

        #region SubWindow
        public async UniTask<UIWindow> OpenSubWindow(string prefabPath)
        {
            await UniTask.Yield();
            return null;
        }
        #endregion
    }
}
