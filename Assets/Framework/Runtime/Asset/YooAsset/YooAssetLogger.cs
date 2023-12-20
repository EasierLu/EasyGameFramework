using System;
using YooAsset;

namespace EGFramework.Runtime.YooAsset
{
    public class YooAssetLogger : ILogger
    {
        public void Error(string message)
        {
            Util.Log.Error(message);
        }

        public void Exception(Exception exception)
        {
            Util.Log.Exception(exception);
        }

        public void Log(string message)
        {
            Util.Log.Info(message);
        }

        public void Warning(string message)
        {
            Util.Log.Warning(message);
        }
    }
}
