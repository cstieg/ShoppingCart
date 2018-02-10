using Cstieg.Sales.Models;
using System;

namespace Cstieg.Sales.Interfaces
{
    public interface IPromoCode : ISalesEntity
    {
        string Code { get; set; }

        string Description { get; set; }

        int? PromotionalItemId { get; set; }
        Product PromotionalItem { get; set; }

        decimal? PromotionalItemPrice { get; set; }

        int? WithPurchaseOfId { get; set; }
        Product WithPurchaseOf { get; set; }

        decimal? MinimumQualifyingPurchase { get; set; }

        decimal? PercentOffOrder { get; set; }

        decimal? PercentOffItem { get; set; }

        decimal? SpecialPrice { get; set; }

        int? SpecialPriceItemId { get; set; }
        Product SpecialPriceItem { get; set; }

        DateTime? CodeStart { get; set; }

        DateTime? CodeEnd { get; set; }
    }
}
