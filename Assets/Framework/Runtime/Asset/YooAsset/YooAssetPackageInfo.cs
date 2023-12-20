using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime.Asset
{
    [Serializable]
    public class YooAssetPackageInfo
    {
        public string name;
        public EDefaultBuildPipeline buildPipeline;

        public YooAssetPackageInfo(string name, EDefaultBuildPipeline buildPipeline)
        {
            this.name = name;
            this.buildPipeline = buildPipeline;
        }
    }
}
