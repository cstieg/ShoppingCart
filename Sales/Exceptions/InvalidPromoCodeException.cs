using System;

namespace Cstieg.Sales.Exceptions
{
    public class InvalidPromoCodeException : Exception
    {
        public InvalidPromoCodeException() : base() { }

        public InvalidPromoCodeException(string message) : base(message) { }
    }
}
