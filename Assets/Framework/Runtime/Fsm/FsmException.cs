using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGFramework.Runtime
{
    public class FsmException : Exception
    {
        public FsmException(string message) : base(message)
        {
        }
    }
}
