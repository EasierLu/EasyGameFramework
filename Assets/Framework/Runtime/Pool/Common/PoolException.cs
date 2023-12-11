using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace EGFramework.Runtime.Pool
{
    public class PoolException : Exception
    {
        public PoolException() : base()
        {
        }

        public PoolException(string message) : base(message)
        {
        }

        public PoolException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PoolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
