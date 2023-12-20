using EGFramework.Runtime.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
//Gen use code stat
    public partial class TestWindow : UIWindow
    {
        public static string PrefabPath => "xxxxx";

        public override string UIName => typeof(TestWindow).Name;

        public override UILayer Layer => UILayer.Base;

        protected override void InitComponent()
        {
            throw new System.NotImplementedException();
        }
//Gen use code end
    }
}
