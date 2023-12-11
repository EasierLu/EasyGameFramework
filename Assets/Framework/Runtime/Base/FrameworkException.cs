using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace EGFramework.Runtime.Base
{
    public class FrameworkException : Exception
    {
        public FrameworkException() : base()
        {
        }

        public FrameworkException(string message) : base(message)
        {
        }

        public FrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}