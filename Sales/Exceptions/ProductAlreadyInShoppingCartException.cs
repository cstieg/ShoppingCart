using System;

namespace Cstieg.Sales.Exceptions
{
    public class ProductAlreadyInShoppingCartException : Exception
    {
        public ProductAlreadyInShoppingCartException() : base() { }

        public ProductAlreadyInShoppingCartException(string message) : base(message) { }
    }
}
