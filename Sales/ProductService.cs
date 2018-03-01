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
            var products = await _context.Products.Where(p => !p.DoNotDisplay).ToListAsync();
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetFrontPageProductsAsync()
        {
            var products = await _context.Products.Where(p => p.DisplayOnFrontPage).ToListAsync();
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return SortAllWebImages(products);
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.FirstAsync(p => p.Id == id);
            return SortWebImages(product);
        }

        public List<Product> SortAllWebImages(List<Product> products)
        {
            foreach (var product in products)
            {
                SortWebImages(product);
            }
            return products;
        }

        public Product SortWebImages(Product product)
        {
            if (product.WebImages == null)
            {
                return product;
            }
            product.WebImages = GetSortedWebImages(product.WebImages);
            return product;
        }

        public List<WebImage> GetSortedWebImages(List<WebImage> webImages)
        {
            return webImages.OrderBy(w => w.Order).ToList();
        }
    }
}
