using Cstieg.Geography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    /// <summary>
    /// Model of an Address
    /// </summary>
    public class ShipToAddress : AddressBase
    {
        [Key]
        public override int Id { get; set; }

        [ForeignKey("Customer")]
        public int ?CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public override string Recipient { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public override string Address1 { get; set; }

        [StringLength(50)]
        public override string Address2 { get; set; }

        [StringLength(50)]
        public override string City { get; set; }

        [StringLength(50)]
        public override string State { get; set; }

        [StringLength(15)]
        public override string PostalCode { get; set; }

        [StringLength(50)]
        public override string Country { get; set; }

        [StringLength(25)]
        public override string Phone { get; set; }
    }
}