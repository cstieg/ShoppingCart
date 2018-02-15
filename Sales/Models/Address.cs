using Cstieg.Sales.Interfaces;
using Cstieg.ObjectHelpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class Address : ISalesEntity, IAddress
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey("Customer")]
        public virtual int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public virtual string Recipient { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public virtual string Address1 { get; set; }

        [StringLength(50)]
        public virtual string Address2 { get; set; }

        [StringLength(50)]
        public virtual string City { get; set; }

        [StringLength(50)]
        public virtual string State { get; set; }

        [StringLength(15)]
        public virtual string PostalCode { get; set; }

        [StringLength(50)]
        public virtual string Country { get; set; }

        [StringLength(25)]
        public virtual string Phone { get; set; }

        public override string ToString()
        {
            string returnString = Recipient == "" ? "" : Recipient + ", ";
            returnString += Address1 + ", ";
            returnString += Address2 == "" ? "" : Address2 + ", ";
            returnString += City + ", ";
            returnString += State == "" ? "" : State + " ";
            returnString += Country;
            return returnString;
        }

        /// <summary>
        /// Determines whether an address is equivalent to this (does not consider phone).
        /// Is case sensitive, but does not consider differences in leading or trailing spaces
        /// </summary>
        /// <param name="address">The address to compare to the present address</param>
        /// <returns>True if the two addresses are equivalent, false if not</returns>
        public virtual bool IsSame(IAddress address)
        {
            if (address == null) return false;
            
            // set null strings to empty strings to avoid null reference error on Trim
            this.SetNullStringsToEmpty();
            address.SetNullStringsToEmpty();

            return Address1.Trim() == address.Address1.Trim() &&
                Address2.Trim() == address.Address2.Trim() &&
                City.Trim() == address.City.Trim() &&
                State.Trim() == address.State.Trim() &&
                PostalCode.Trim() == address.PostalCode.Trim();
        }

    }
}
