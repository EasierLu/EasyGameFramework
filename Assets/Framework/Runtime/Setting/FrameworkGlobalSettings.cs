using EGFramework.Runtime.Asset;
using System;
using System.Collections;
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
        [Header("EncryptionMode")]
        [SerializeField]
        [Tooltip("资源加密模式")]
        private EncryptionMode m_AssetEncryptionMode = EncryptionMode.None;
        public EncryptionMode assetEncryptionMode { get => m_AssetEncryptionMode; }

    }

}