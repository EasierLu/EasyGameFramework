using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset.Editor;

namespace EGFramework.Editor.Asset.YooAsset.FilterRule
{
    [DisplayName("指定扩展名收集(逗号分隔)")]
    public class ExtensionCollecteRule : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (data.UserData != null)
            {
                HashSet<string> excludeExt = new HashSet<string>(data.UserData.Split(','));
                var fileExt = Path.GetExtension(data.AssetPath).ToLower();
                return excludeExt.Contains(fileExt);
            }
            return true;
        }
    }
}
