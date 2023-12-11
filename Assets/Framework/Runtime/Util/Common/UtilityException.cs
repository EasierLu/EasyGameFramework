using System;
using System.Runtime.Serialization;

namespace EGFramework.Runtime.Util
{
    public class UtilityException : Exception
    {
        public UtilityException() : base()
        {
        }

        public UtilityException(string message) : base(message)
        {
        }

        public UtilityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UtilityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
