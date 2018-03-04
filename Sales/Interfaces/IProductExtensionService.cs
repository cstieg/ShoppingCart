using Cstieg.Sales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cstieg.Sales.Interfaces
{
    public interface IProductExtensionService
    {
        Task<Product> GetProductExtensionAsync(Product product);
        Task<List<Product>> GetProductExtensionsAsync(List<Product> product);
        void AddProductExtension(Product product);
        void EditProductExtension(Product product);
        void DeleteProductExtension(Product product);
    }

}
