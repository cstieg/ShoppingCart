using Cstieg.Sales.Models;
using System;
using System.Collections.Generic;

namespace Cstieg.Sales.Interfaces
{
    public interface IOrder : ISalesEntity
    {
        string Cart { get; set; }

        int? CustomerId { get; set; }
        Customer Customer { get; set; }

        DateTime? DateOrdered { get; set; }

        int? ShipToAddressId { get; set; }
        Address ShipToAddress { get; set; }

        int? BillToAddressId { get; set; }
        Address BillToAddress { get; set; }

        List<OrderDetail> OrderDetails { get; set; }

        decimal Subtotal { get; }

        decimal Shipping { get; }

        decimal Tax { get; set; }

        decimal Total { get; }
        
        string Description { get; }

        string NoteToPayee { get; set; }
    }
}
