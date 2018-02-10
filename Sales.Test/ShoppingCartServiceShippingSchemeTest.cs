using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cstieg.Sales.Interfaces;
using Cstieg.Sales.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cstieg.Sales.Test
{
    public partial class ShoppingCartServiceTest
    {
        Country countryUs = new Country() { Id = 1, Name = "United States", IsoCode2 = "US" };
        Country countryCa = new Country() { Id = 2, Name = "Canada", IsoCode2 = "CA" };
        ShippingCountry shippingCountryUs;
        ShippingCountry shippingCountryCa1;
        ShippingCountry shippingCountryCa2;
        ShippingScheme shippingScheme;

        public void InitializeShippingScheme()
        {
            // Arrange
            shippingScheme = new ShippingScheme() { Id = 1, Name = "Default", ShippingCountries = new List<IShippingCountry>() };

            shippingCountryUs = new ShippingCountry()
            {
                Id = 1,
                Country = countryUs,
                CountryId = countryUs.Id,
                AdditionalShipping = 0.00M,
                ShippingScheme = shippingScheme,
                ShippingSchemeId = 1
            };
            shippingCountryCa1 = new ShippingCountry()
            {
                Id = 2,
                Country = countryCa,
                CountryId = countryCa.Id,
                AdditionalShipping = 3.00M,
                MaxQty = 2,
                ShippingScheme = shippingScheme,
                ShippingSchemeId = 1
            };
            shippingCountryCa2 = new ShippingCountry()
            {
                Id = 3,
                Country = countryCa,
                CountryId = countryCa.Id,
                AdditionalShipping = 5.25M,
                MinQty = 3,
                ShippingScheme = shippingScheme,
                ShippingSchemeId = 1,
                AdditionalShippingIsPerItem = true,
                BaseShippingIsPerItem = true
            };

            shippingScheme.ShippingCountries.Add(shippingCountryUs);
            shippingScheme.ShippingCountries.Add(shippingCountryCa1);
            shippingScheme.ShippingCountries.Add(shippingCountryCa2);
        }

        [TestMethod]
        public async Task RemoveShippingChargesAsync()
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
            var orderDetail = await shoppingCartService.RemoveShippingChargesAsync(mockOrderDetail);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail is null.");
            Assert.AreEqual(0.00M, orderDetail.Shipping, "Shipping is not zero.");
        }

        [TestMethod]
        public async Task RemoveAllShippingChargesAsync()
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
            var mockOrderDetail2 = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct2,
                ProductId = sampleProduct2.Id,
                Quantity = 2
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail2);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.RemoveAllShippingChargesAsync();

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(0.00M, shoppingCart.TotalShipping, "Shipping is not zero.");
        }

        [TestMethod]
        public async Task UpdateShippingChargesAsyncUs()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                Country = "US",
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            sampleProduct.ShippingScheme = shippingScheme;
            sampleProduct.ShippingSchemeId = shippingScheme.Id;
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 1
            };
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var orderDetail = await shoppingCartService.UpdateShippingChargesAsync(mockOrderDetail);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail is null.");
            Assert.AreEqual(1.00M, orderDetail.Shipping, "Shipping amount is not correct.");
        }


        [TestMethod]
        public async Task UpdateShippingChargesAsyncCa()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                Country = "CA",
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            sampleProduct.ShippingScheme = shippingScheme;
            sampleProduct.ShippingSchemeId = shippingScheme.Id;
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 2
            };
            sampleProduct2.ShippingScheme = shippingScheme;
            sampleProduct2.ShippingSchemeId = shippingScheme.Id;

            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var orderDetail = await shoppingCartService.UpdateShippingChargesAsync(mockOrderDetail);

            // Assert
            Assert.IsNotNull(orderDetail, "OrderDetail is null.");
            Assert.AreEqual(4.00M, orderDetail.Shipping, "Shipping amount is not correct.");
        }

        [TestMethod]
        public async Task UpdateAllShippingChargesAsync()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                Country = "CA",
                OwnerId = ownerId,
                Order = new Order()
                {
                    OrderDetails = new List<OrderDetail>()
                }
            };
            sampleProduct.ShippingScheme = shippingScheme;
            sampleProduct.ShippingSchemeId = shippingScheme.Id;
            var mockOrderDetail = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 3
            };
            var mockOrderDetail2 = new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct2,
                ProductId = sampleProduct2.Id,
                Quantity = 1
            };
            sampleProduct2.ShippingScheme = shippingScheme;
            sampleProduct2.ShippingSchemeId = shippingScheme.Id;

            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail);
            mockShoppingCart.Order.OrderDetails.Add(mockOrderDetail2);
            context.ShoppingCarts.Add(mockShoppingCart);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.UpdateAllShippingChargesAsync();

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(25.98M, shoppingCart.TotalShipping, "Shipping amount is not correct.");
            Assert.AreEqual(18.75M, shoppingCart.Order.OrderDetails.Find(o => o.Product.Name == "Sample").Shipping, "Sample product 1 shipping is incorrect.");
            Assert.AreEqual(7.23M, shoppingCart.Order.OrderDetails.Find(o => o.Product.Name == "Sample 2").Shipping, "Sample product 2 shipping is incorrect.");
        }

    }
}
