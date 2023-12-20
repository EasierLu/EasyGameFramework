using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    /// <summary>
    /// Usage it like material render queue(FrameworkComponentPriority.Asset + 1)
    /// </summary>
    public static class FrameworkComponentPriority
    {
        public static readonly int Base = 0;
        public static readonly int Asset = 100;
        public static readonly int Config = 200;
        public static readonly int UI = 300;
        public static readonly int Scene = 400;
        public static readonly int Network = 500;
        public static readonly int Procedure = 600;

        public static readonly int Custom = 100000;
    }
}
