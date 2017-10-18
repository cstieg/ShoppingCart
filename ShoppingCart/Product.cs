using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cstieg.ShoppingCart
{
    /// <summary>
    /// Model of a Product to be sold in a shopping cart
    /// </summary>
    public class ProductBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Shipping { get; set; }

        [DisplayName("Upload image file")]
        public string ImageUrl { get; set; }
        public string ImageSrcSet { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [Display(Name = "Display on Front Page")]
        public bool DisplayOnFrontPage { get; set; }

        [Display(Name = "Do Not Display (Promotional Item)")]
        public bool DoNotDisplay { get; set; }
    }
}
