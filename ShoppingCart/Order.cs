using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.ShoppingCart
{
    /// <summary>
    /// Model of an order containing one or more items
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        public Order()
        {
            ShipToAddress = new ShipToAddress();
            BillToAddress = new ShipToAddress();
            OrderDetails = new List<OrderDetail>();
        }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        public DateTime DateOrdered { get; set; }

        [ForeignKey("ShipToAddress")]
        public int ShipToAddressId { get; set; }
        public virtual ShipToAddress ShipToAddress { get; set; }

        [ForeignKey("BillToAddress")]
        public int BillToAddressId { get; set; }
        public virtual ShipToAddress BillToAddress { get; set; }

        [InverseProperty("Order")]
        public virtual List<OrderDetail> OrderDetails { get; set; }

        public decimal Subtotal {
            get
            {
                decimal subtotal = 0;
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    subtotal += OrderDetails[i].ExtendedPrice;
                }
                return subtotal;
            }
        }
        
        public decimal Shipping {
            get
            {
                decimal shipping = 0;
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    shipping += OrderDetails[i].Shipping;
                }
                return shipping;
            }
        }

        public decimal Total
        {
            get
            {
                return Subtotal + Shipping;
            }
        }
    }
}
