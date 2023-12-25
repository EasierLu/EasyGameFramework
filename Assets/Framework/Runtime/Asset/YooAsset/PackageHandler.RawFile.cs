using Cysharp.Threading.Tasks;
using EGFramework.Runtime.Util;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public partial class PackageHandler
    {
        #region LoadRawFile
        public byte[] LoadRawFile(string fileName)
        {
            var loadOperation = m_Package.LoadRawFileSync(fileName);
            if (loadOperation.Status == EOperationStatus.Succeed)
            {
                var data = loadOperation.GetRawFileData();
                loadOperation.Release();
                return data;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, fileName);
                return null;
            }
        }

        public async UniTask<byte[]> LoadRawFileAsync(string fileName, uint priority = 1000)
        { 
            var loadOperation = m_Package.LoadRawFileAsync(fileName, priority);
            await loadOperation;
            if (loadOperation.Status == EOperationStatus.Succeed)
            {
                var data = loadOperation.GetRawFileData();
                loadOperation.Release();
                return data;
            }
            else
            {
                Log.ErrorFormat("资源包{0}加载资源{1}失败", m_PackageName, fileName);
                return null;
            }
        }
        #endregion
    }
}
