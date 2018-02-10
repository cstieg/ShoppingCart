using Cstieg.Sales.Models;
using System;

namespace Cstieg.Sales.Interfaces
{
    public interface IOrderDetail : ISalesEntity
    {
        int ProductId { get; set; }
        Product Product { get; set; }

        DateTime PlacedInCart { get; set; }

        int Quantity { get; set; }

        decimal UnitPrice { get; set; }

        decimal Shipping { get; set; }
        
        decimal Tax { get; set; }

        int OrderId { get; set; }
        Order Order { get; set; }

        bool IsPromotionalItem { get; set; }

        decimal ExtendedPrice { get; }

        decimal TotalPrice { get; }
    }
}
