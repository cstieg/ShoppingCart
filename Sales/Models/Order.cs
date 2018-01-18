using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    /// <summary>
    /// Model of an order containing one or more items
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        public string Cart { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime? DateOrdered { get; set; }

        [ForeignKey("ShipToAddress")]
        public int? ShipToAddressId { get; set; }
        public virtual ShipToAddress ShipToAddress { get; set; }

        [ForeignKey("BillToAddress")]
        public int? BillToAddressId { get; set; }
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

        public string Description
        {
            get
            {
                string description = "";
                switch (OrderDetails.Count)
                {
                    case 0:
                        description = "Order with no products yet";
                        break;
                    case 1:
                        description = OrderDetails[0].Product.Name + " - Qty: " + OrderDetails[0].Quantity;
                        break;
                    default:
                        description = "Multiple products";
                        break;
                }
                return description;
            }
        }

        /// <summary>
        /// Sets the shipping amount for each order detail in the order, based upon the country
        /// </summary>
        /// <param name="countryCode">The 2 digit ISO country code where the order will be shipped to</param>
        public void SetShippingByCountry(string countryCode)
        {
            foreach (var orderDetail in OrderDetails)
            {
                orderDetail.SetShippingByCountry(countryCode);
            }
        }
    }
}
