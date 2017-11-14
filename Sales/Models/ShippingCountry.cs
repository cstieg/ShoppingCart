using Cstieg.Geography;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class ShippingCountry
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ShippingScheme")]
        public int ShippingSchemeId { get; set; }
        [JsonIgnore]
        public virtual ShippingScheme ShippingScheme { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        public int? MinQty { get; set; }

        public int? MaxQty { get; set; }

        public decimal AdditionalShipping { get; set; }
    }
}
