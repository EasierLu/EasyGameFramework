using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime.Asset
{
    public class FileOffsetDecryption : IDecryptionServices
    {
        private const int FILE_OFFSET = 2;

        public AssetBundle LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            int offset = GetFileOffset(fileInfo.BundleName, FILE_OFFSET);
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, (ulong)offset);
        }

        public AssetBundleCreateRequest LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            int offset = GetFileOffset(fileInfo.BundleName, FILE_OFFSET);
            managedStream = null;
            return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, (ulong)offset);
        }

        public static int GetFileOffset(string bundleName, int defaultOffset = 0)
        {
            if (defaultOffset != 0)
            { 
                return bundleName[0] + bundleName[1] + defaultOffset;
            }
            return defaultOffset;
        }
    }
}
