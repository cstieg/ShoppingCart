using Cstieg.Sales.Interfaces;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Cstieg.Sales
{
    public class ProductService : IProductService
    {
        private ISalesDbContext _context;

        public ProductService(ISalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetDisplayProductsAsync()
        {
            return await _context.Products.Where(p => !p.DoNotDisplay).ToListAsync();
        }

        public async Task<List<Product>> GetFrontPageProductsAsync()
        {
            return await _context.Products.Where(p => p.DisplayOnFrontPage).ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _context.Products.FirstAsync(p => p.Id == id);
        }

        public List<WebImage> GetSortedWebImages(List<WebImage> webImages)
        {
            return webImages.OrderBy(w => w.Order).ToList();
        }
    }
}
