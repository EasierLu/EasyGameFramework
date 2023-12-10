using EGFramework.Runtime;
using EGFramework.Runtime.Setting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGFramework.Editor.Setting
{
    public class FrameworkGlobalSettingsProvider : SettingsProvider
    {
        private static string m_DefaultSettingFilePath = FrameworkGlobalSettings.FrameworkWorkPath + "/Runtime/Resources/Settings/FrameworkGlobalSettings.asset";
        private const string m_HeaderName = "Framework/Global";
        private SerializedObject m_SettingData;

        public FrameworkGlobalSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {

        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            m_SettingData = GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            using var changeCheckScope = new EditorGUI.ChangeCheckScope();

            EditorGUILayout.PropertyField(m_SettingData.FindProperty("m_AssetEncryptionMode"));
            EditorGUILayout.LabelField("123");

            EditorGUILayout.Space(20);
            if (!changeCheckScope.changed) return;
            m_SettingData.ApplyModifiedPropertiesWithoutUndo();
        }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(m_DefaultSettingFilePath);
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(FrameworkSettingUtil.GlobalSettings);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new FrameworkGlobalSettingsProvider(m_HeaderName, SettingsScope.Project);
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<FrameworkGlobalSettings>();
                return provider;
            }
            return null;
        }
    }
}
