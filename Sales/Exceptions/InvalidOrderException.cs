using System;

namespace Cstieg.Sales.Exceptions
{
    public class InvalidOrderException : Exception
    {
        public InvalidOrderException() : base() { }

        public InvalidOrderException(string message) : base(message) { }
    }
}
