using Cstieg.Sales.Models;
using System.Collections.Generic;

namespace Cstieg.Sales.Interfaces
{
    public interface IShoppingCart : ISalesEntity
    {
        string OwnerId { get; set; }

        int? OrderId { get; set; }
        Order Order { get; set; }

        string Country { get; set; }

        List<PromoCodeAdded> PromoCodesAdded { get; set; }

        decimal TotalExtendedPrice { get; }

        decimal TotalShipping { get; }

        decimal GrandTotal { get; }

        bool NeedsRefresh { get; set; }
    }
}
