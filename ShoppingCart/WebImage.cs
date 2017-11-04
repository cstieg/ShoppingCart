using Cstieg.Image;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.ShoppingCart
{
    public class WebImage : WebImageBase
    {
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        [JsonIgnore]
        public virtual ProductBase Product { get; set; }
    }
}