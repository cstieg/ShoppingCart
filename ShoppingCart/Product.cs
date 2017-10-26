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
        public virtual int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public virtual string Name { get; set; }

        [StringLength(100)]
        public virtual string Description { get; set; }

        public virtual decimal Price { get; set; }

        public virtual decimal Shipping { get; set; }

        [DisplayName("Product Image")]
        public virtual string ImageUrl { get; set; }
        public virtual string ImageSrcSet { get; set; }
        
        [Display(Name = "Featured")]
        public virtual bool DisplayOnFrontPage { get; set; }

        [Display(Name = "No Display")]
        public virtual bool DoNotDisplay { get; set; }
    }
}
