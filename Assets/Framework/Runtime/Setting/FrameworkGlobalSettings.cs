using EGFramework.Runtime.Asset;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Setting
{
    [CreateAssetMenu(fileName = "FrameworkGlobalSettings", menuName = "Framework/Global Settings", order = 40)]
    public class FrameworkGlobalSettings : ScriptableObject
    {
        
        [Header("EncryptionMode")]
        [NonSerialized]
        public static string FrameworkWorkPath = "Assets/Framework";

        [Header("Asset")]
        [SerializeField]
        [Tooltip("资源更新模式")]
        private AssetMode m_RuntimeAssetMode = AssetMode.HostPlay;
        public AssetMode runtimeAssetMode { get => m_RuntimeAssetMode; }

        [SerializeField]
        [Tooltip("资源加密模式")]
        private EncryptionMode m_AssetEncryption = EncryptionMode.None;
        public EncryptionMode assetEncryption { get => m_AssetEncryption; }

        [SerializeField]
        [Tooltip("默认资源包")]
        public string m_DefaultAssetsPackage;
        public string defaultAssetsPackage => m_DefaultAssetsPackage;

        [SerializeField]
        [Tooltip("资源包信息")]
        private List<YooAssetPackageInfo> m_AssetsPackageInfo;
        public List<YooAssetPackageInfo> assetsPackageInfo => m_AssetsPackageInfo;
    }

}