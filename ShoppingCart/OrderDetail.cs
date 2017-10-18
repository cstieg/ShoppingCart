﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.ShoppingCart
{ 
    /// <summary>
    /// A order detail containing a quantity of a given product
    /// </summary>
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        public virtual ProductBase Product { get; set; }

        [Required]
        [Display(Name = "Placed in Cart")]
        public DateTime PlacedInCart { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        public decimal Shipping { get; set; }

        [ForeignKey("Order")]
        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        [Display(Name = "Promotional Item")]
        public bool IsPromotionalItem { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Extended Price")]
        public decimal ExtendedPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }

        }

        [ReadOnly(true)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice
        {
            get 
            {
                return ExtendedPrice + Shipping;
            }
        }

        public override string ToString()
        {
            return Product.ToString() + " Qty " + Quantity.ToString();
        }
    }
}
