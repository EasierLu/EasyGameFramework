using EGFramework.Runtime.Base;
using EGFramework.Runtime.Util;
using UnityEngine;

namespace EGFramework.Runtime.Setting
{
    public class FrameworkSettingUtil
    {
        private static readonly string GlobalSettingsPath = $"Settings/FrameworkGlobalSettings";
        private static FrameworkGlobalSettings m_GlobalSettings;

        public static FrameworkGlobalSettings GlobalSettings
        {
            get
            {
                if (m_GlobalSettings == null)
                {
                    m_GlobalSettings = GetSingletonAssetsByResources<FrameworkGlobalSettings>(GlobalSettingsPath);
                }
                return m_GlobalSettings;
            }
        }

        private static T GetSingletonAssetsByResources<T>(string assetsPath) where T : ScriptableObject, new()
        {
            string assetType = typeof(T).Name;
#if UNITY_EDITOR
            string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
            if (globalAssetPaths.Length > 1)
            {
                foreach (var assetPath in globalAssetPaths)
                {
                    Log.ErrorFormat("不能有多个 {0}. 路径: {1}", assetType, UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath));
                }
                throw new FrameworkException($"不能有多个 {assetType}");
            }
#endif
            T customGlobalSettings = Resources.Load<T>(assetsPath);
            if (customGlobalSettings == null)
            {
                Log.ErrorFormat("没找到 {0} asset，自动创建创建一个:{1}.", assetType, assetsPath);
                return null;
            }

            return customGlobalSettings;
        }
    }
}
