using System.Text;
using System;

namespace EGFramework.Runtime.Util
{
    public static partial class Utility
    {
        /// <summary>
        /// 字符相关的实用函数。
        /// </summary>
        public static partial class Text
        {
            [ThreadStatic]
            private static StringBuilder s_CachedStringBuilder = null;
            private const int STRING_BUILDER_CAPACITY = 1024;

            /// <summary>
            /// 获取格式化字符串
            /// </summary>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            /// <returns>格式化后的字符串</returns>
            /// <exception cref="UtilityException"></exception>
            public static string Format(string format, params object[] args)
            {
                if (format == null)
                {
                    throw new UtilityException("Format is invalid.");
                }

                CheckCachedStringBuilder();
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, args);
                return s_CachedStringBuilder.ToString();
            }

            private static void CheckCachedStringBuilder()
            {
                if (s_CachedStringBuilder == null)
                {
                    s_CachedStringBuilder = new StringBuilder(STRING_BUILDER_CAPACITY);
                }
            }
        }
    }
}