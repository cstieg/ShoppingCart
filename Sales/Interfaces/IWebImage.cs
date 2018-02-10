using Cstieg.Sales.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cstieg.Sales.Interfaces
{
    public interface IWebImage : ISalesEntity
    {
        string ImageUrl { get; set; }

        string ImageSrcSet { get; set; }

        string Caption { get; set; }

        [ForeignKey("Product")]
        int? ProductId { get; set; }
        Product Product { get; set; }

        int? Order { get; set; }
    }
}
