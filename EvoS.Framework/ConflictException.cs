using System;

namespace EvoS.Framework
{
    public class ConflictException : Exception
    {
        public ConflictException(String message) : base(message) {}
    }
}
