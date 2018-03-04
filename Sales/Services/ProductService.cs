using Cstieg.Sales.Interfaces;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
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
            var products = await _context.Products.Include(p => p.WebImages).Where(p => !p.DoNotDisplay).ToListAsync();
            products = await IncludeExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetFrontPageProductsAsync()
        {
            var products = await _context.Products.Include(p => p.WebImages).Where(p => p.DisplayOnFrontPage).ToListAsync();
            products = await IncludeExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products.Include(p => p.WebImages).ToListAsync();
            products = await IncludeExtensions(products);
            return SortAllWebImages(products);
        }

        public async Task<Product> GetAsync(int id)
        {
            var product = await _context.Products.Include(p => p.WebImages).FirstAsync(p => p.Id == id);
            product = await IncludeExtension(product);
            return SortWebImages(product);
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            var product = await _context.Products.FirstAsync(p => p.Name == name);
            product = await IncludeExtension(product);
            return SortWebImages(product);
        }

        public async Task<Product> AddAsync(Product product)
        {
            AddExtension(product);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> EditAsync(Product product)
        {
            EditExtension(product);
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            product = await IncludeExtension(product);
            await DeleteAsync(product);
        }

        public async Task DeleteAsync(Product product)
        {
            foreach (var webImage in product.WebImages.ToList())
            {
                _context.WebImages.Remove(webImage);
            }
            DeleteExtension(product);

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

        private async Task<Product> IncludeExtension(Product product)
        {
            if (_productExtensionService != null)
            {
                return await _productExtensionService.GetProductExtensionAsync(product);
            }
            return product;
        }

        private async Task<List<Product>> IncludeExtensions(List<Product> products)
        {
            if (_productExtensionService != null)
            {
                return await _productExtensionService.GetProductExtensionsAsync(products);
            }
            return products;
        }

        private void DeleteExtension(Product product)
        {
            if (_productExtensionService != null)
            {
                _productExtensionService.DeleteProductExtension(product);
            }
        }

        private void AddExtension(Product product)
        {
            if (_productExtensionService != null)
            {
                _productExtensionService.AddProductExtension(product);
            }
        }

        private void EditExtension(Product product)
        {
            if (_productExtensionService != null)
            {
                _productExtensionService.EditProductExtension(product);
            }
        }

    }
}
