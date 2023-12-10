using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime.Asset
{
    public class AssetDecryptionUtil
    {
        public static IDecryptionServices AssetDecryptionServices
        {
            get
            {
                var mode = FrameworkSettingUtil.GlobalSettings.assetEncryptionMode;
                switch (mode)
                {
                    case EncryptionMode.Offset:
                        return new FileOffsetDecryption();
                    case EncryptionMode.XOR:
                        return new XORDecryption();
                    case EncryptionMode.None:
                    default:
                        return null;
                }
            }
        }
    }
}
