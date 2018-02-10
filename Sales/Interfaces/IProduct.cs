using Cstieg.Sales.Models;
using System.Collections.Generic;

namespace Cstieg.Sales.Interfaces
{
    public interface IProduct : ISalesEntity
    {
        string Name { get; set; }
        
        string Sku { get; set; }

        string Gtin { get; set; }

        string UrlName { get; set; }

        string Brand { get; set; }

        string MetaDescription { get; set; }

        string Condition { get; set; }

        decimal Price { get; set; }

        decimal Shipping { get; set; }

        int? ShippingSchemeId { get; set; }
        ShippingScheme ShippingScheme { get; set; }

        List<WebImage> WebImages { get; set; }

        string ProductInfo { get; set; }

        string GoogleProductCategory { get; set; }

        bool DisplayOnFrontPage { get; set; }

        bool DoNotDisplay { get; set; }
    }
}
