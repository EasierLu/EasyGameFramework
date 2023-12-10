using EGFramework.Runtime.Asset;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace EGFramework.Runtime.Asset
{
    public class XORDecryption : IDecryptionServices
    {
        public const int FILE_OFFSET = 3;
        public const byte XOR_KEY = 183;
        public const int MANAGED_READ_BUFFER_SIZE = 1024;

        public AssetBundle LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            bundleStream.BaseOffset = LoadFromFileOffset(fileInfo);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, MANAGED_READ_BUFFER_SIZE);
        }

        public AssetBundleCreateRequest LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            bundleStream.BaseOffset = LoadFromFileOffset(fileInfo);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, MANAGED_READ_BUFFER_SIZE);
        }

        public int LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return FileOffsetDecryption.GetFileOffset(fileInfo.BundleName, FILE_OFFSET);
        }

        private class BundleStream : FileStream
        {
            public int BaseOffset = 0; 
            public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
            {
            }
            public BundleStream(string path, FileMode mode) : base(path, mode)
            {
            }

            public override int Read(byte[] array, int offset, int count)
            {
                var index = base.Read(array, offset + BaseOffset, count);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] ^= XOR_KEY;
                }
                return index;
            }
        }
    }
}
