using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Pool
{
    /// <summary>
    /// 引用接口。
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 清理引用。
        /// </summary>
        void Clear();
    }
}
