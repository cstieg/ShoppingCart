using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Cstieg.Sales.Models
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

        public virtual decimal Price { get; set; }

        public virtual decimal Shipping { get; set; }

        [ForeignKey("ShippingScheme")]
        public virtual int? ShippingSchemeId { get; set; }
        public virtual ShippingScheme ShippingScheme { get; set; }

        [Display(Name = "Product Images")]
        [InverseProperty("Product")]
        public virtual ICollection<WebImage> WebImages { get; set; }

        [StringLength(2000)]
        [AllowHtml]
        public string ProductInfo { get; set; }

        [Display(Name = "Featured")]
        public virtual bool DisplayOnFrontPage { get; set; }

        [Display(Name = "No Display")]
        public virtual bool DoNotDisplay { get; set; }
    }
}
