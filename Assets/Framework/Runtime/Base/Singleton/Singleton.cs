using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public abstract class Singleton<T> where T : class
    {
        class Nested
        {
            // 创建模板类实例，参数2设为true表示支持私有构造函数
            internal static readonly T instance = Activator.CreateInstance(typeof(T), true) as T;
        }
        public static T Instance { get { return Nested.instance; } }
    }
}
