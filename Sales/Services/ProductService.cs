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
        private readonly ISalesDbContext _context;
        private readonly IProductExtensionService _productExtensionService;

        public ProductService(ISalesDbContext context, IProductExtensionService productExtensionService = null)
        {
            _context = context;
            _productExtensionService = productExtensionService;
        }

        public async Task<List<Product>> GetDisplayProductsAsync()
        {
            var products = await _context.Products.Where(p => !p.DoNotDisplay).ToListAsync();
            products = await AddExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetFrontPageProductsAsync()
        {
            var products = await _context.Products.Where(p => p.DisplayOnFrontPage).ToListAsync();
            products = await AddExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            products = await AddExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<Product> GetAsync(int id)
        {
            var product = await _context.Products.FirstAsync(p => p.Id == id);
            product = await AddExtension(product);
            return SortWebImages(product);
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            var product = await _context.Products.FirstAsync(p => p.Name == name);
            product = await AddExtension(product);
            return SortWebImages(product);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> EditAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            await DeleteAsync(product);
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
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

        private async Task<Product> AddExtension(Product product)
        {
            if (_productExtensionService != null)
            {
                return await _productExtensionService.GetProductExtension(product);
            }
            return product;
        }

        private async Task<List<Product>> AddExtensions(List<Product> products)
        {
            if (_productExtensionService != null)
            {
                return await _productExtensionService.GetProductExtensions(products);
            }
            return products;
        }

    }
}
