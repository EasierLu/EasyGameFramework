using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Base;
using EGFramework.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class UIManager : FrameworkComponent
    {
        private const string UI_ROOT_PREFAB_PATH = "Assets/AssetsPackage/UI/Prefabs/Common/UIRoot.prefab";
        private Dictionary<UILayer, Stack<UIArgs>> m_UIStack;

        internal struct UIArgs
        {
            internal string name;
            internal object[] args;

            internal UIArgs(string _name, object[] _args)
            {
                name = _name;
                args = _args;
            }
        }

        private int m_IncrementId = 0;

        public override async UniTask Init()
        {

        }

        public override async UniTask Shutdown()
        {
            await UniTask.Yield();
        }

        public override void ComponentUpdate(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<GameObject> LoadGameObjectAsync(string path, Transform parent = null)
        { 
            
        }

        public int GetIncrementId()
        {
            return ++m_IncrementId;
        }
    }
}
