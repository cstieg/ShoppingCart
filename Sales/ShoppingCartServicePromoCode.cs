using Cstieg.ObjectHelpers;
using Cstieg.Sales.Exceptions;
using Cstieg.Sales.Interfaces;
using Cstieg.Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cstieg.Sales
{
    public partial class ShoppingCartService : IShoppingCartService
    {
        public async Task<ShoppingCart> RemoveAllPromoCodesAsync(bool saveChanges = true)
        {
            var shoppingCart = await GetShoppingCartAsync();
            
            // remove promocodes 
            // copy to array to avoid modifying the actual collection
            foreach (var promoCodeAdded in shoppingCart.PromoCodesAdded.ToArray())
            {
                promoCodeAdded.ShoppingCart = null;
                _context.PromoCodesAdded.Remove(promoCodeAdded);
            }

            // remove promotional items added by promocodes
            foreach (var orderDetail in shoppingCart.GetOrderDetails().ToArray())
            {
                if (orderDetail.IsPromotionalItem)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
            }
            shoppingCart.GetOrderDetails().RemoveAll(o => o.IsPromotionalItem);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            shoppingCart.PromoCodesAdded.Clear();
            return shoppingCart;
        }

        public async Task<ShoppingCart> RemovePromoCodeAsync(string code, bool saveChanges = true)
        {
            var shoppingCart = await GetShoppingCartAsync();

            var promoCodeAdded = shoppingCart.PromoCodesAdded.Find(p => p.PromoCode.Code == code);
            var promoCode = promoCodeAdded.PromoCode;

            // remove product added by promocode
            if (promoCode.PromotionalItem != null)
            {
                await RemoveProductAsync(promoCode.PromotionalItem.Id, false);
            }

            shoppingCart.PromoCodesAdded.Remove(promoCodeAdded);

            _context.PromoCodesAdded.Remove(promoCodeAdded);

            shoppingCart = await UpdatePromoCodesAsync(false);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

            return shoppingCart;
        }

        public async Task<ShoppingCart> UpdatePromoCodesAsync(bool saveChanges = true)
        {
            var shoppingCart = await GetShoppingCartAsync();

            // make copy of promocodes added list
            var promoCodesAdded = new List<PromoCodeAdded>();
            ObjectHelper.CopyList(shoppingCart.PromoCodesAdded, promoCodesAdded);

            // clear the promocodes
            await RemoveAllPromoCodesAsync(false);
            await ResetPricesAsync(false);

            // re-add eligible codes
            foreach (var promoCodeAdded in promoCodesAdded)
            {
                try
                {
                    await AddPromoCodeAsync(promoCodeAdded.PromoCode);
                }
                catch
                {
                    // continue if promocode was invalid, just don't add
                    // if promo code was removed, refresh shopping cart
                    shoppingCart.NeedsRefresh = true;
                }
            }

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return shoppingCart;
        }

        public async Task<ShoppingCart> AddPromoCodeAsync(string code)
        {
            // look up promo code
            var promoCodes = await _context.PromoCodes.ToListAsync();
            var promoCode = promoCodes.Find(p => p.Code.ToLower().Trim() == code.ToLower().Trim());
            if (promoCode == null)
            {
                throw new InvalidPromoCodeException("Must enter a valid promoCode!");
            }
            return await AddPromoCodeAsync(promoCode);
        }

        public async Task<ShoppingCart> AddPromoCodeAsync(PromoCode promoCode)
        {
            if (promoCode == null)
            {
                throw new InvalidPromoCodeException("Must enter a valid promoCode!");
            }

            var shoppingCart = await GetShoppingCartAsync();

            // check constraints
            if (shoppingCart.PromoCodesAdded.Exists(p => p.PromoCode.Code == promoCode.Code))
            {
                throw new InvalidPromoCodeException("Promo code already added!");
            }

            if (promoCode.SpecialPriceItem != null 
                && !shoppingCart.GetOrderDetails().Exists(o => o.ProductId == promoCode.SpecialPriceItemId && !o.IsPromotionalItem))
            { 
                throw new InvalidPromoCodeException("Must purchase special item to receive discount: " + promoCode.SpecialPriceItem.Name);
            }

            if (promoCode.WithPurchaseOf != null 
                && !shoppingCart.GetOrderDetails().Exists(o => o.ProductId == promoCode.WithPurchaseOf.Id && !o.IsPromotionalItem))
            {
                throw new InvalidPromoCodeException("Must purchase qualifying item: " + promoCode.WithPurchaseOf.Name);
            }

            if (promoCode.MinimumQualifyingPurchase != null && shoppingCart.TotalExtendedPrice < promoCode.MinimumQualifyingPurchase)
            {
                throw new InvalidPromoCodeException("Must have a minimum purchase of $" + promoCode.MinimumQualifyingPurchase.ToString());
            }

            if (promoCode.CodeStart != null && DateTime.Now < promoCode.CodeStart)
            {
                DateTime codeStart = (DateTime)promoCode.CodeStart;
                throw new InvalidPromoCodeException("Code is not valid until " + codeStart.ToShortDateString());
            }

            if (promoCode.CodeEnd != null && DateTime.Now > promoCode.CodeEnd)
            {
                throw new InvalidPromoCodeException("Code is expired!");
            }

            // check for and apply promo code actions
            AddPromotionalItem(shoppingCart, promoCode);
            ApplySpecialPrice(shoppingCart, promoCode);
            ApplyPercentOffItem(shoppingCart, promoCode);
            ApplyPercentOffOrder(shoppingCart, promoCode);

            // add promo code to list in shopping cart
            var newPromoCodeAdded = new PromoCodeAdded() { ShoppingCart = shoppingCart, PromoCode = (PromoCode)promoCode };
            shoppingCart.PromoCodesAdded.Add(newPromoCodeAdded);
            _context.PromoCodesAdded.Add(newPromoCodeAdded);

            // save shopping cart
            await _context.SaveChangesAsync();
            return shoppingCart;
        }

        /// <summary>
        /// Adds a promotional item to the shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shopping cart to which to add the promo code</param>
        /// <param name="promoCode">The promo code object to add</param>
        private void AddPromotionalItem(ShoppingCart shoppingCart, PromoCode promoCode)
        {
            if (promoCode.PromotionalItem != null)
            {
                var orderDetail = new OrderDetail()
                {
                    Order = shoppingCart.Order,
                    OrderId = (int) shoppingCart.OrderId,
                    Product = promoCode.PromotionalItem,
                    UnitPrice = promoCode.PromotionalItemPrice ?? 0M,
                    Quantity = 1,
                    IsPromotionalItem = true,
                    PlacedInCart = DateTime.Now,
                    Shipping = 0M
                };
                shoppingCart.GetOrderDetails().Add(orderDetail);
            }
        }

        /// <summary>
        /// Applies a special price to an item in the shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shopping cart to which to add the promo code</param>
        /// <param name="promoCode">The promo code object to add</param>
        private void ApplySpecialPrice(ShoppingCart shoppingCart, PromoCode promoCode)
        {
            if (promoCode.SpecialPriceItem != null && promoCode.SpecialPrice != null)
            {
                OrderDetail item = shoppingCart.Order.OrderDetails.Find(o => o.Product.Id == promoCode.SpecialPriceItem.Id 
                    && !o.IsPromotionalItem);
                // special price may be zero for free item
                decimal specialPrice = (decimal) promoCode.SpecialPrice;
                decimal regularPrice = item.Product.Price;
                item.UnitPrice = specialPrice < regularPrice ? specialPrice : regularPrice;
            }
        }

        /// <summary>
        /// Applies a percentage off an item in the shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shopping cart to which to add the promo code</param>
        /// <param name="promoCode">The promo code object to add</param>
        private void ApplyPercentOffItem(ShoppingCart shoppingCart, PromoCode promoCode)
        {
            if (promoCode.PercentOffItem != null && promoCode.PercentOffItem > 0 && promoCode.SpecialPriceItem != null)
            {
                OrderDetail orderDetail = shoppingCart.Order.OrderDetails.Find(o => o.Product.Id == promoCode.SpecialPriceItem.Id 
                    && !o.IsPromotionalItem);
                if (orderDetail == null) throw new InvalidPromoCodeException("Can't find item to discount.");
                orderDetail.UnitPrice = PercentOff(orderDetail.Product.Price, promoCode.PercentOffItem ?? 0);
            }
        }

        /// <summary>
        /// Applies a percentage off all the items in the shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shopping cart to which to add the promo code</param>
        /// <param name="promoCode">The promo code object to add</param>
        private void ApplyPercentOffOrder(ShoppingCart shoppingCart, PromoCode promoCode)
        {
            if (promoCode.PercentOffOrder != null && promoCode.PercentOffOrder > 0)
            {
                foreach (var orderDetail in shoppingCart.Order.OrderDetails)
                {
                    orderDetail.UnitPrice = PercentOff(orderDetail.UnitPrice, (decimal) promoCode.PercentOffOrder);
                }
            }

        }

        /// <summary>
        /// Calculates the price of an item with a given percent discount
        /// </summary>
        /// <param name="originalPrice">The original price of the item</param>
        /// <param name="percentOff">The percentage to discount off the item</param>
        /// <returns>The discounted price</returns>
        private decimal PercentOff(decimal originalPrice, decimal percentOff)
        {
            if (percentOff == 100)
            {
                return 0;
            }
            return originalPrice * (100 - percentOff) / 100;
        }

        /// <summary>
        /// Calculates the price of an item with a given percent discount
        /// </summary>
        /// <param name="originalPrice">The original price of the item</param>
        /// <param name="percentOff">The percentage to discount off the item</param>
        /// <returns>The discounted price</returns>
        private decimal PercentOff(decimal originalPrice, int percentOff)
        {
            return PercentOff(originalPrice, (decimal)percentOff);
        }

    }
}
