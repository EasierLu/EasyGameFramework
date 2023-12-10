using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace EGFramework.Editor.Asset
{
    public class FileOffsetEncryption : IEncryptionServices
    {
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            return DoEncrypt(fileInfo);
        }

        public static EncryptResult DoEncrypt(EncryptFileInfo fileInfo)
        {
            int offset = FileOffsetDecryption.GetFileOffset(fileInfo.BundleName);
            if (offset != 0)
            {
                byte[] fileData = File.ReadAllBytes(fileInfo.FilePath);
                var encryptedData = new byte[fileData.Length + offset];
                Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);
                EncryptResult result = new EncryptResult();
                result.Encrypted = true;
                result.EncryptedData = encryptedData;
                return result;
            }
            else
            {
                EncryptResult result = new EncryptResult();
                result.Encrypted = false;
                return result;
            }
        }
    }
}