using Cstieg.Sales.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Cstieg.Sales.Models
{
    public class Product : ISalesEntity
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public virtual string Name { get; set; }

        [Index]
        [StringLength(20)]
        public virtual string Sku { get;  set; }

        /// <summary>
        /// Global Trade Item Number (UPC)
        /// </summary>
        [Display(Name = "UPC")]
        [StringLength(14)]
        public virtual string Gtin { get; set; }

        [StringLength(70)]
        public virtual string Brand { get; set; }

        [Index]
        [StringLength(20)]
        public virtual string UrlName { get; set; }

        [StringLength(250)]
        public virtual string MetaDescription { get; set; }

        [StringLength(15)]
        public virtual string Condition { get; set; }

        public virtual decimal Price { get; set; }

        public virtual decimal Shipping { get; set; }

        [ForeignKey("ShippingScheme")]
        public virtual int? ShippingSchemeId { get; set; }
        public virtual ShippingScheme ShippingScheme { get; set; }

        [Display(Name = "Product Images")]
        [InverseProperty("Product")]
        public virtual List<WebImage> WebImages { get; set; }

        [StringLength(2000)]
        [AllowHtml]
        public virtual string ProductInfo { get; set; }

        [StringLength(500)]
        public virtual string GoogleProductCategory { get; set; }

        [Display(Name = "Featured")]
        public virtual bool DisplayOnFrontPage { get; set; }

        [Display(Name = "No Display")]
        public virtual bool DoNotDisplay { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
