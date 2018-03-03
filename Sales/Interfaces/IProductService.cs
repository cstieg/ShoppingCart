using Cstieg.Sales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cstieg.Sales.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();

        Task<List<Product>> GetDisplayProductsAsync();

        Task<List<Product>> GetFrontPageProductsAsync();

        Task<Product> GetAsync(int id);

        List<WebImage> GetSortedWebImages(List<WebImage> webImages);
    }
}
