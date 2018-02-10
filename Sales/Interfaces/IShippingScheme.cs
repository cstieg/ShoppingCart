using Cstieg.Sales.Models;
using System.Collections.Generic;

namespace Cstieg.Sales.Interfaces
{
    public interface IShippingScheme : ISalesEntity
    {
        string Name { get; set; }

        string Description { get; set; }

        List<ShippingCountry> ShippingCountries { get; set; }
    }
}
