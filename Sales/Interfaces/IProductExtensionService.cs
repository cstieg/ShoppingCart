using Cstieg.Sales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cstieg.Sales.Interfaces
{
    public interface IProductExtensionService
    {
        Task<Product> GetProductExtension(Product product);
        Task<List<Product>> GetProductExtensions(List<Product> product);
    }

}
