using Cstieg.Sales.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class ShippingScheme : IShippingScheme
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [InverseProperty("ShippingScheme")]
        public virtual List<ShippingCountry> ShippingCountries { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
