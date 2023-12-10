using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using System;
using System.IO;
using YooAsset;

namespace EGFramework.Editor.Asset
{
    public class XOREncryption : IEncryptionServices
    {
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            return DoEncrypt(fileInfo);
        }

        public static EncryptResult DoEncrypt(EncryptFileInfo fileInfo)
        {

            int offset = XORDecryption.FILE_OFFSET;

            byte[] fileData = File.ReadAllBytes(fileInfo.FilePath);

            for (int i = 0; i < fileData.Length; i++)
            {
                fileData[i] ^= XORDecryption.XOR_KEY;
            }

            var encryptedData = new byte[fileData.Length + offset];
            Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);

            EncryptResult result = new EncryptResult();
            result.Encrypted = true;
            result.EncryptedData = encryptedData;
            return result;
        }

    }
}