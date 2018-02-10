using Cstieg.Sales.Models;

namespace Cstieg.Sales.Interfaces
{
    public interface IShippingCountry : ISalesEntity
    {
        int ShippingSchemeId { get; set; }
        ShippingScheme ShippingScheme { get; set; }

        int CountryId { get; set; }
        Country Country { get; set; }

        int? MinQty { get; set; }

        int? MaxQty { get; set; }

        decimal AdditionalShipping { get; set; }

        bool BaseShippingIsPerItem { get; set; }

        bool AdditionalShippingIsPerItem { get; set; }

        bool FreeShipping { get; set; }
    }
}
