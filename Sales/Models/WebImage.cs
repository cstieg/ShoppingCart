using Cstieg.Sales.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class WebImage : IWebImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; }

        [StringLength(1000)]
        public string ImageSrcSet { get; set; }

        [StringLength(100)]
        public string Caption { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }

        // Order in which images are displayed
        public int? Order { get; set; }
    }
}
