using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Cstieg.Sales.Exceptions;
using Cstieg.Sales.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cstieg.Sales.Test
{
    public partial class ShoppingCartServiceTest
    {
        Product specialPriceProduct = new Product()
        {
            Id = 3,
            Name = "Special Price Item",
            Price = 40M,
            Shipping = 1.50M
        };
        Product bonusProduct = new Product()
        {
            Id = 4,
            Name = "Bonus Item",
            Price = 3.00M,
            Shipping = 0M
        };

        PromoCode promotionalItemWithPurchaseOf = new PromoCode()
        {
            Id = 1,
            Code = "PIWPO",
            CodeStart = DateTime.Now.Subtract(new TimeSpan(3, 0, 0)),
            CodeEnd = DateTime.Now.Add(new TimeSpan(3, 0, 0)),
            PromotionalItemPrice = 0.10M
        };
        PromoCode promotionalItemMinimumQualifyingPurchase = new PromoCode()
        {
            Id = 2,
            Code = "PIMQP",
            PromotionalItemPrice = 0M,
            MinimumQualifyingPurchase = 10.00M
        };
        PromoCode percentOffOrder = new PromoCode()
        {
            Id = 3,
            Code = "POO",
            PercentOffOrder = 50M,
        };
        PromoCode percentOffItem = new PromoCode()
        {
            Id = 4,
            Code = "POI",
            PercentOffItem = 25M
        };
        PromoCode specialPriceItem = new PromoCode()
        {
            Id = 5,
            Code = "SPI",
            SpecialPrice = 0.50M
        };
        PromoCode notAvailableYetPromo = new PromoCode()
        {
            Id = 6,
            Code = "NAYP",
            PercentOffOrder = 50M,
            CodeStart = DateTime.Now.Add(new TimeSpan(3, 0, 0))
        };
        PromoCode expiredPromoCode = new PromoCode()
        {
            Id = 7,
            Code = "EPC",
            PercentOffOrder = 50M,
            CodeEnd = DateTime.Now.Subtract(new TimeSpan(3, 0, 0))
        };


        public void InitializePromoCode()
        {
            // Arrange
            promotionalItemWithPurchaseOf.PromotionalItem = bonusProduct;
            promotionalItemWithPurchaseOf.WithPurchaseOf = sampleProduct;
            promotionalItemMinimumQualifyingPurchase.PromotionalItem = bonusProduct;
            percentOffItem.SpecialPriceItem = SampleProduct2;
            specialPriceItem.SpecialPriceItem = SampleProduct2;
        }

        [TestMethod]
        public async Task AddPromoCodeItemWithPurchaseOf()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping,
                Quantity = 1
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.PromoCodes.Add(promotionalItemWithPurchaseOf);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(promotionalItemWithPurchaseOf);

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(1, await context.OrderDetails.CountAsync(o => o.IsPromotionalItem && 
                o.Product.Name == "Bonus Item" && o.UnitPrice == 0.10M && o.Quantity == 1),
                "Must be 1 promotional item added.");
            Assert.AreEqual(1.11M, shoppingCart.GrandTotal, "Incorrect total.");
            Assert.AreEqual(1, shoppingCart.PromoCodesAdded.FindAll(p => p.PromoCode.Code == "PIWPO").Count, "Must be 1 promocode in shoppingCart.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPromoCodeException))]
        public async Task AddPromoCodeItemWithPurchaseOfWithoutProduct()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                Quantity = 1,
                UnitPrice = SampleProduct2.Price,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.PromoCodes.Add(promotionalItemWithPurchaseOf);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(promotionalItemWithPurchaseOf);

            // Assert
        }

        [TestMethod]
        public async Task AddPromoCodePromotionalItemMinimumQualifyingPurchase()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                Quantity = 1,
                UnitPrice = SampleProduct2.Price,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.PromoCodes.Add(promotionalItemMinimumQualifyingPurchase);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(promotionalItemMinimumQualifyingPurchase);

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(1, await context.OrderDetails.CountAsync(o => o.IsPromotionalItem &&
                o.Product.Name == "Bonus Item" && o.UnitPrice == 0M && o.Quantity == 1),
                "Must be 1 promotional item added.");
            Assert.AreEqual(133.23M, shoppingCart.GrandTotal, "Incorrect total.");
            Assert.AreEqual(1, shoppingCart.PromoCodesAdded.FindAll(p => p.PromoCode.Code == "PIMQP").Count, "Must be 1 promocode in shoppingCart.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPromoCodeException))]
        public async Task AddPromoCodePromotionalItemMinimumQualifyingPurchaseNotQualified()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                Quantity = 1,
                UnitPrice = sampleProduct.Price,
                Shipping = sampleProduct.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.PromoCodes.Add(promotionalItemMinimumQualifyingPurchase);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(promotionalItemMinimumQualifyingPurchase);

            // Assert
        }

        [TestMethod]
        public async Task AddPromoCodePercentOffOrder()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = sampleProduct,
                ProductId = sampleProduct.Id,
                UnitPrice = sampleProduct.Price,
                Quantity = 2,
                Shipping = sampleProduct.Shipping
            });
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                UnitPrice = SampleProduct2.Price,
                Quantity = 1,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.PromoCodes.Add(percentOffOrder);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(percentOffOrder);

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(69.74M, shoppingCart.GrandTotal, "Incorrect total.");
            Assert.AreEqual(1, shoppingCart.PromoCodesAdded.FindAll(p => p.PromoCode.Code == "POO").Count, "Must be 1 promocode in shoppingCart.");
        }

        [TestMethod]
        public async Task AddPromoCodePercentOffItem()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                UnitPrice = SampleProduct2.Price,
                Quantity = 2,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.Products.Add(SampleProduct2);
            context.PromoCodes.Add(percentOffItem);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(percentOffItem);

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(197.73M, shoppingCart.GrandTotal, "Incorrect total.");
            Assert.AreEqual(1, shoppingCart.PromoCodesAdded.FindAll(p => p.PromoCode.Code == "POI").Count, "Must be 1 promocode in shoppingCart.");
        }


        [TestMethod]
        public async Task AddPromoCodeSpecialPriceItem()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                Quantity = 2,
                UnitPrice = SampleProduct2.Price,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.Products.Add(SampleProduct2);
            context.PromoCodes.Add(specialPriceItem);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(specialPriceItem);

            // Assert
            Assert.IsNotNull(shoppingCart, "ShoppingCart is null.");
            Assert.AreEqual(5.23M, shoppingCart.GrandTotal, "Incorrect total.");
            Assert.AreEqual(1, shoppingCart.PromoCodesAdded.FindAll(p => p.PromoCode.Code == "SPI").Count, "Must be 1 promocode in shoppingCart.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPromoCodeException))]
        public async Task AddPromoCodeNotAvailableYetPromo()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                Quantity = 1,
                UnitPrice = SampleProduct2.Price,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.Products.Add(SampleProduct2);
            context.PromoCodes.Add(notAvailableYetPromo);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(notAvailableYetPromo);

            // Assert

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPromoCodeException))]
        public async Task AddPromoCodeExpiredPromo()
        {
            // Arrange
            var mockShoppingCart = new ShoppingCart()
            {
                OwnerId = ownerId,
                Order = new Order()
                {
                    Created = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                }
            };
            mockShoppingCart.Order.OrderDetails.Add(new OrderDetail()
            {
                PlacedInCart = DateTime.Now,
                Product = SampleProduct2,
                ProductId = SampleProduct2.Id,
                Quantity = 1,
                UnitPrice = SampleProduct2.Price,
                Shipping = SampleProduct2.Shipping
            });
            context.ShoppingCarts.Add(mockShoppingCart);
            context.Products.Add(SampleProduct2);
            context.PromoCodes.Add(expiredPromoCode);
            await context.SaveChangesAsync();

            // Act
            var shoppingCart = await shoppingCartService.AddPromoCodeAsync(expiredPromoCode);

            // Assert
        }

    }
}
