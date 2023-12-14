using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Event
{
    interface IEventHandler : IReference
    {
        public abstract string EventKey { set; get; }
        public abstract CappedArrayPool<UniTask> ArrayPool { get; set; }
    }
}
