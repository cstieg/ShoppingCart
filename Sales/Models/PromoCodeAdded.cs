using Cstieg.Sales.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class PromoCodeAdded : ISalesEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }
        [JsonIgnore]
        public virtual ShoppingCart ShoppingCart { get; set; }

        [ForeignKey("PromoCode")]
        public int PromoCodeId { get; set; }
        [JsonIgnore]
        public virtual PromoCode PromoCode { get; set; }
    }
}
