using Cstieg.Sales.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cstieg.Sales.Models
{
    public class ShoppingCart : IShoppingCart
    {
        /// <summary>
        /// Constructor that initializes objects to avoid null object errors
        /// </summary>
        public ShoppingCart()
        {
            PromoCodesAdded = new List<PromoCodeAdded>();
        }

        [Key]
        public int Id { get; set; }

        // Set by Request.AnonymousId
        [StringLength(36)]
        [Index]
        public string OwnerId { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string Country { get; set; }

        [InverseProperty("ShoppingCart")]
        public virtual List<PromoCodeAdded> PromoCodesAdded { get; set; }

        public decimal TotalExtendedPrice
        {
            get
            {
                return GetOrderDetails().Sum(p => p.ExtendedPrice);
            }
        }

        public decimal TotalShipping
        {
            get
            {
                return GetOrderDetails().Sum(p => p.Shipping);
            }
        }

        public decimal GrandTotal
        {
            get
            {
                return TotalExtendedPrice + TotalShipping;
            }
        }

        public List<OrderDetail> GetOrderDetails()
        {
            if (Order == null || Order.OrderDetails == null)
            {
                return new List<OrderDetail>();
            }
            return Order.OrderDetails;
        }

        [NotMapped]
        public bool NeedsRefresh { get; set; }

        public override string ToString()
        {
            return Order == null ? "Empty ShoppingCart" : Order.ToString();
        }

    }
}
