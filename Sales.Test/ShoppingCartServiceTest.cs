using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cstieg.Sales.Exceptions;
using Cstieg.Sales.Models;
using Cstieg.Sales.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cstieg.Sales.Test
{
    [TestClass]
    public partial class ShoppingCartServiceTest
    {
        SqlConnection connection;
        SalesContext context;
        DbContextTransaction transaction;
        string ownerId = "abcdefgh12345";
        ShoppingCartService shoppingCartService;

        Product sampleProduct = new Product()
        {
            Id = 1,
            Name = "Sample",
            Price = 0.01M,
            Shipping = 1.00M
        };
        Product sampleProduct2 = new Product()
        {
            Id = 2,
            Name = "Sample 2",
            Price = 129.00M,
            Shipping = 4.23M
        };

        [TestInitialize]
        public void Initialize()
        {
            InitializeShippingScheme();
            InitializePromoCode();

            try
            {
                connection = LocalDb.GetLocalDb("SalesTest", true);
            }
            catch
            {
                connection = LocalDb.GetLocalDb("SalesTest");
            }

            context = new SalesContext(connection, true);
            transaction = context.Database.BeginTransaction();
            shoppingCartService = new ShoppingCartService(context, ownerId);
        }

        [TestCleanup]
        public void CloseData()
        {
            transaction.Rollback();
            transaction.Dispose();
            context.Dispose();
            context = null;
            connection.Close();
            connection.Dispose();
            connection = null;
        }

        [TestMethod]
        public async Task GetShoppingCartAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync();

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.IsNotNull(shoppingCart.Order, "ShoppingCart.Order is null.");
            Assert.IsNotNull(shoppingCart.Order.OrderDetails, "ShoppingCart.Order.OrderDetails is null.");
            Assert.AreEqual(sampleProduct.Id, shoppingCart.Order.OrderDetails.First().ProductId, "Product not returned in shopping cart.");
        }

        [TestMethod]
        public async Task AddProductAsync()
        {
            // Act
            var orderDetail = await shoppingCartService.AddProductAsync(sampleProduct);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail was not returned.");
            Assert.AreNotEqual(0, orderDetail.Id, "OrderDetail Id was 0.");
            Assert.AreEqual("Sample", orderDetail.Product.Name, "Correct product not referenced.");
            Assert.IsNotNull(orderDetail.Order, "Order was null.");
            Assert.IsTrue(await context.ShoppingCarts.AnyAsync(x => x.OwnerId == ownerId), "ShoppingCart was not found.");
        }

        [TestMethod]
        public async Task RemoveProductAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var orderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id
            };
            mockShoppingCart.Order.OrderDetails.Add(orderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            await shoppingCartService.RemoveProductAsync(sampleProduct);

            // Assert
            Assert.IsFalse(await context.OrderDetails.AnyAsync(x => x.Id == orderDetail.Id), "OrderDetail added still found.");
            Assert.IsFalse(await context.Orders.AnyAsync(x => x.Id == orderDetail.OrderId), "Order added still found.");
            Assert.IsFalse(await context.OrderDetails.AnyAsync(x => x.Id == x.Id), "Leftover OrderDetail");
            Assert.IsFalse(await context.Orders.AnyAsync(x => x.Id == x.Id), "Leftover Order");
        }

        [TestMethod]
        public async Task ClearShoppingCartAsync()
        {
            // Arrange
            var orderDetail = await shoppingCartService.AddProductAsync(sampleProduct);

            // Act
            await shoppingCartService.ClearShoppingCartAsync();

            // Assert
            var shoppingCart = await context.ShoppingCarts.Where(x => x.OwnerId == ownerId).FirstOrDefaultAsync();
            Assert.IsFalse(await context.OrderDetails.AnyAsync(x => x.Id == orderDetail.Id), "OrderDetail added still found.");
            Assert.IsFalse(await context.Orders.AnyAsync(x => x.Id == orderDetail.OrderId), "Order added still found.");
            Assert.IsNull(shoppingCart.Order, "Order should be null.");
        }

        [TestMethod]
        public async Task IncrementProductAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var orderDetail = await shoppingCartService.IncrementProductAsync(sampleProduct);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail is null.");
            Assert.AreEqual(5, orderDetail.Quantity, "OrderDetail quantity did not increment.");
        }

        [TestMethod]
        public async Task DecrementProductAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var orderDetail = await shoppingCartService.DecrementProductAsync(sampleProduct);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail is null.");
            Assert.AreEqual(3, orderDetail.Quantity, "OrderDetail quantity did not decrement.");
        }

        [TestMethod]
        public async Task SetCountryAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.SetCountryAsync("CA");

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual("CA", (await context.ShoppingCarts.FirstOrDefaultAsync(s => s.OwnerId == ownerId)).Country, "Country not set correctly.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOrderException))]
        public async Task VerifyOrderDetailsChangedAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            var paymentOrderDetails = new List<OrderDetail>();
            paymentOrderDetails.Add(new OrderDetail()
            {
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 4,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping - 0.01M
            });

            // Act
            await shoppingCartService.VerifyOrderDetailsAsync(paymentOrderDetails, mockShoppingCart.GrandTotal);

            // Assert
            // should throw exception
        }

        [TestMethod]
        public async Task VerifyOrderDetailsAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            var paymentOrderDetails = new List<OrderDetail>
            {
                new OrderDetail()
                {
                    Product = sampleProduct,
                    ProductId = sampleProduct.Id,
                    Quantity = 4,
                    UnitPrice = sampleProduct.Price,
                    Shipping = sampleProduct.Shipping
                }
            };

            // Act
            await shoppingCartService.VerifyOrderDetailsAsync(paymentOrderDetails, mockShoppingCart.GrandTotal);

            // Assert
            // passes if doesn't return exception
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOrderException))]
        public async Task VerifyCountryChangedAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Country = "CA",
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();
            
            // Act
            await shoppingCartService.VerifyCountryAsync("US");

            // Assert
            // should throw exception
        }

        [TestMethod]
        public async Task CheckoutAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping,
                Quantity = 4
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            var address = new Address()
            {
                Address1 = "abcd",
                City = "Detroit",
                State = "Michigan",
                PostalCode = "49401"
            };

            var customer = new Customer()
            {
                CustomerName = "chris",
                EmailAddress = "any@address.com",
            };

            string cart = "ABCDEFG";

            // Act
            var order = await shoppingCartService.CheckoutAsync(address, address, customer, cart);

            // Assert
            Assert.IsNotNull(order, "Order should not be null.");
            Assert.AreEqual(1, await context.Orders.CountAsync(o => o.Cart == cart));
            Assert.AreEqual(1, await context.Orders.CountAsync());
            Assert.AreEqual(1, (await context.Orders.FirstOrDefaultAsync(o => o.Cart == cart)).OrderDetails.Count);
            Assert.AreEqual(1, await context.OrderDetails.CountAsync());
            Assert.AreEqual(4, (await context.Orders.FirstOrDefaultAsync(o => o.Cart == cart)).OrderDetails.FirstOrDefault().Quantity);
            Assert.AreEqual(1, await context.Addresses.CountAsync());
            Assert.AreEqual("abcd", (await context.Addresses.FirstOrDefaultAsync()).Address1);
            Assert.IsNotNull((await context.Orders.FirstOrDefaultAsync(o => o.Cart == cart)).Customer);
            Assert.AreEqual("any@address.com", (await context.Customers.FirstOrDefaultAsync()).EmailAddress);


            // Arrange
            mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();
            var shoppingCarts = await context.ShoppingCarts.ToListAsync();
            string cart2 = "234099jf099j32";

            // Act
            var order2 = await shoppingCartService.CheckoutAsync(address, address, customer, cart2);

            // Assert
            Assert.AreEqual(1, await context.Addresses.CountAsync());
            Assert.AreEqual(1, await context.Customers.CountAsync());

        }

    }
}
