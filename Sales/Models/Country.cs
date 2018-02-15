using Cstieg.Sales.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Models
{
    public class Country : ISalesEntity, ICountry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(2)]
        public string IsoCode2 { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
