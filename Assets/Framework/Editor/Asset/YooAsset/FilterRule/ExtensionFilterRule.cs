using System.Collections.Generic;
using System.IO;
using YooAsset.Editor;

namespace EGFramework.Editor.Asset.YooAsset.FilterRule
{
    [DisplayName("指定扩展名过滤(逗号分隔)")]
    public class ExtensionFilterRule : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (data.UserData != null)
            {
                HashSet<string> excludeExt = new HashSet<string>(data.UserData.Split(','));
                var fileExt = Path.GetExtension(data.AssetPath).ToLower();
                return !excludeExt.Contains(fileExt);
            }
            return true;
        }
    }
}
