using Cstieg.Sales.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class Order : ISalesEntity
    {
        public Order(DateTime created)
        {
            Created = Created ?? created;
            OrderDetails = OrderDetails ?? new List<OrderDetail>();
        }

        public Order()
        {
            OrderDetails = OrderDetails ?? new List<OrderDetail>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        [Index]
        public string Cart { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? DateOrdered { get; set; }

        [ForeignKey("ShipToAddress")]
        public int? ShipToAddressId { get; set; }
        public virtual Address ShipToAddress { get; set; }

        [ForeignKey("BillToAddress")]
        public int? BillToAddressId { get; set; }
        public virtual Address BillToAddress { get; set; }

        [InverseProperty("Order")]
        public virtual List<OrderDetail> OrderDetails { get; set; }

        public decimal Subtotal
        {
            get
            {
                decimal subtotal = 0;
                foreach(var orderDetail in OrderDetails)
                {
                    subtotal += orderDetail.ExtendedPrice;
                }
                return subtotal;
            }
        }

        public decimal Shipping
        {
            get
            {
                decimal shipping = 0;
                foreach(var orderDetail in OrderDetails)
                {
                    shipping += orderDetail.Shipping;
                }
                return shipping;
            }
        }

        public decimal Tax { get; set; }

        public decimal Total
        {
            get
            {
                return Subtotal + Shipping + Tax;
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

        [StringLength(255)]
        public string NoteToPayee { get; set; }

        public override string ToString()
        {
            return Description;
        }

    }
}
