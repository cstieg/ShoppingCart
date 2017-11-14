using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class ShippingScheme
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [InverseProperty("ShippingScheme")]
        public virtual ICollection<ShippingCountry> ShippingCountries { get; set; }
    }
}
