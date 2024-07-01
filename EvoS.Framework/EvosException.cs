using System;

namespace EvoS.Framework
{
    public class EvosException : Exception
    {
        public EvosException() : base() {}
        public EvosException(String message) : base(message) {}
        public EvosException(String message, Exception innerException) : base(message, innerException) { }
    }
}
