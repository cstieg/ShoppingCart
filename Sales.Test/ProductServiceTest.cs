using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cstieg.Sales.Test
{
    [TestClass]
    public class ProductServiceTest
    {
        SalesContext context = new SalesContext();
        private TransactionScope _transactionScope;
        private int _initialProductCount;
        private ProductService _productService;

        [TestInitialize]
        public virtual void Initialize()
        {
            _productService = new ProductService(context);
            _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var fakeProducts = new List<Product>()
            {
                new Product() { Id = 1, Name = "Product 1", DisplayOnFrontPage = true, DoNotDisplay = false },
                new Product() { Id = 2, Name = "Product 2", DisplayOnFrontPage = true, DoNotDisplay = false },
                new Product() { Id = 3, Name = "Product 3", DisplayOnFrontPage = false, DoNotDisplay = false },
                new Product() { Id = 4, Name = "Product 4", DisplayOnFrontPage = false, DoNotDisplay = false },
                new Product() { Id = 5, Name = "Product 5", DisplayOnFrontPage = false, DoNotDisplay = true }
            };
            _initialProductCount = fakeProducts.Count();

            context.Products.AddRange(fakeProducts);
            context.SaveChanges();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            Transaction.Current.Rollback();
            _transactionScope.Dispose();
        }

        [TestMethod]
        public async Task GetProductsAsync()
        {
            // Act
            var products = await _productService.GetProductsAsync();

            // Assert
            Assert.AreEqual(_initialProductCount, products.Count, "Wrong number of products returned");


        }

        [TestMethod]
        public async Task GetFrontPageProductsAsync()
        {
            // Act
            var products = await _productService.GetFrontPageProductsAsync();

            // Assert
            Assert.AreEqual(2, products.Count, "Wrong number of products returned");
            Assert.IsNull(products.Find(p => !p.DisplayOnFrontPage), "Only products with DisplayOnFrontPage as true should be returned");
        }

        [TestMethod]
        public async Task GetDisplayProductsAsync()
        {
            // Act
            var products = await _productService.GetDisplayProductsAsync();

            // Assert
            Assert.AreEqual(4, products.Count, "Wrong number of products returned");
            Assert.IsNull(products.Find(p => p.DoNotDisplay), "Product with DoNotDisplay as true should not be returned");
        }


        [TestMethod]
        public async Task GetProductAsync()
        {
            // Arrange
            int id = (await context.Products.FirstAsync(p => p.Name == "Product 1")).Id;

            // Act
            var product = await _productService.GetProductAsync(id);

            // Assert
            Assert.AreEqual("Product 1", product.Name);
        }

        [TestMethod]
        public void GetSortedWebImages()
        {
            // Arrange
            var webImages = new List<WebImage>()
            {
                new WebImage() { Id = 1, ImageUrl = "http://www.fakeurl.com", Order = 3 },
                new WebImage() { Id = 2, ImageUrl = "http://www.fakeurl.com", Order = 1 },
                new WebImage() { Id = 3, ImageUrl = "http://www.fakeurl.com", Order = 6 },
                new WebImage() { Id = 4, ImageUrl = "http://www.fakeurl.com", Order = 2 },
                new WebImage() { Id = 5, ImageUrl = "http://www.fakeurl.com", Order = 4 },
            };

            // Act
            webImages = _productService.GetSortedWebImages(webImages);

            // Assert
            Assert.IsTrue(webImages[0].Id == 2);
            Assert.IsTrue(webImages[1].Id == 4);
            Assert.IsTrue(webImages[2].Id == 1);
            Assert.IsTrue(webImages[3].Id == 5);
            Assert.IsTrue(webImages[4].Id == 3);
        }
    }
}
