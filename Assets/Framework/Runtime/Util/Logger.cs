using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime.Util
{
    public static class Log
    {
        public enum LogLevel
        { 
            Debug, 
            Info, 
            Warning, 
            Error, 
            Fatal
        }

        private static LogLevel m_LogLevel = LogLevel.Debug;

        public static LogLevel Level { get => m_LogLevel; set => m_LogLevel = value; }


        private static bool m_IsEnabled = true;
        public static bool IsEnabled { get => m_IsEnabled; set => m_IsEnabled = value; }


        public static void Debug(object msg)
        {
            InnerLog(LogLevel.Debug, msg);
        }

        public static void DebugFormat(string fmt, params object[] args)
        {
            InnerLogFormat(LogLevel.Debug, fmt, args);
        }

        public static void Info(object msg)
        {
            InnerLog(LogLevel.Info, msg);
        }

        public static void InfoFormat(string fmt, params object[] args)
        {
            InnerLogFormat(LogLevel.Info, fmt, args);
        }

        public static void Warning(object msg)
        {
            InnerLog(LogLevel.Warning, msg);
        }

        public static void WarningFormat(string fmt, params object[] args)
        {
            InnerLogFormat(LogLevel.Warning, fmt, args);
        }

        public static void Error(object msg)
        {
            InnerLog(LogLevel.Error, msg);
        }

        public static void ErrorFormat(string fmt, params object[] args)
        {
            InnerLogFormat(LogLevel.Error, fmt, args);
        }

        public static void Fatal(object msg)
        {
            InnerLog(LogLevel.Error, msg);
        }

        public static void FatalFormat(string fmt, params object[] args)
        {
            InnerLogFormat(LogLevel.Fatal, fmt, args);
        }

        private static void InnerLog(LogLevel level ,object msg)
        {
            if (!IsEnabled || level < m_LogLevel)
            {
                return;
            }

            msg = AddLabel(level, msg);
            switch (level)
            {
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    UnityEngine.Debug.LogError(msg);
                    break;
                case LogLevel.Debug:
                case LogLevel.Info:
                default:
                    UnityEngine.Debug.Log(msg);
                    break;
            }
        }

        private static void InnerLogFormat(LogLevel level, string fmt, params object[] args)
        {
            if (!IsEnabled || level < m_LogLevel)
            {
                return;
            }
            string msg = Utility.Text.Format(fmt, args);
            InnerLog(level, msg);
        }

        private static string AddLabel(LogLevel level, object msg) 
        {
#if UNITY_EDITOR
            string labelFmt = "<color={2}>[{0}]</color>{1}";
            string color = level <= LogLevel.Info ? "gray" : level <= LogLevel.Warning ? "yellow" : "red";
            return Utility.Text.Format(labelFmt, level.ToString(), msg, color);
#else
            string labelFmt = "[{0}]{1}";
            return Utility.Text.Format(labelFmt, level.ToString(), msg);
#endif
        }
    }
}
