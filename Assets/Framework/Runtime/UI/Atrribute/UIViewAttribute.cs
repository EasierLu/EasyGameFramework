using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.UI
{
    public class UIViewAttribute : Attribute
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; private set; }

        /// <summary>
        /// 缓存
        /// </summary>
        public bool cache { get; private set; }

        public UIViewAttribute(string path, bool cache = true)
        {
            this.path = path;
            this.cache = cache;
        }
    }
}
