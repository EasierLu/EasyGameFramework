using EGFramework.Runtime.Setting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
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
                    Debug.LogError($"不能有多个 {assetType}. 路径: {UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath)}");
                }
                throw new Exception($"不能有多个 {assetType}");
            }
#endif
            T customGlobalSettings = Resources.Load<T>(assetsPath);
            if (customGlobalSettings == null)
            {
                //TODO Logger
                Debug.LogError($"没找到 {assetType} asset，自动创建创建一个:{assetsPath}.");
                return null;
            }

            return customGlobalSettings;
        }
    }
}
