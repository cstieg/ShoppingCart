using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    /// <summary>
    /// Model of a customer
    /// </summary>
    public class Customer
    {
        public Customer()
        {
            Addresses = new List<ShipToAddress>();
        }

        [Key]
        public int Id { get; set; }

        private string customerName;
        public string CustomerName
        {
            get
            {
                if (customerName == null || customerName == "")
                {
                    return FirstName + " " + LastName;
                }
                return customerName;
            }
            set
            {
                customerName = value;
            }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Registered { get; set; }

        public DateTime LastVisited { get; set; }

        public int TimesVisited { get; set; }

        public string EmailAddress { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<ShipToAddress> Addresses { get; set; }

        public override string ToString()
        {
            return CustomerName;
        }
    }

}
