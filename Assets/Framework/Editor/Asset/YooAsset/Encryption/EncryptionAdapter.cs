using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace EGFramework.Editor.Asset
{
    [DisplayName("使用框架设置")]
    public class EncryptionAdapter : IEncryptionServices
    {
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            switch (FrameworkSettingUtil.GlobalSettings.assetEncryption)
            {
                case EncryptionMode.Offset:
                    return FileOffsetEncryption.DoEncrypt(fileInfo);
                case EncryptionMode.XOR:
                    return XOREncryption.DoEncrypt(fileInfo);
                case EncryptionMode.None:
                default:
                    EncryptResult er = new EncryptResult();
                    er.Encrypted = false;
                    return er;
            }
        }
    }
}
