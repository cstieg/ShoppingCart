using Cstieg.Sales.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Cstieg.Sales.Models
{
    public class Store : ISalesEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string BaseUrl { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(2)]
        public string Country { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }
    }
}
