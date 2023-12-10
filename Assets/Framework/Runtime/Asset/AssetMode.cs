using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public enum AssetMode
    {
        /// <summary> 编辑器模拟模式 </summary>
        Editor,
        /// <summary> 单机无热更模式 </summary>
        Offline,
        /// <summary> 热更模式 </summary>
        HostPlay,
        /// <summary> WebGL运行模式 </summary>
        WebGL
    }
}
