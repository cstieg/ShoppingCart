using Cstieg.Image;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class WebImage : WebImageBase
    {
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        [JsonIgnore]
        public virtual ProductBase Product { get; set; }
    }
}