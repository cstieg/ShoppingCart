using System;

namespace Cstieg.Sales.RSS.Exceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException() : base() { }

        public InvalidModelException(string message) : base(message) {}
    }
}
