using Cstieg.Sales.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cstieg.Sales
{
    public partial class ShoppingCartService : IShoppingCartService
    {      
        public async Task<OrderDetail> RemoveShippingChargesAsync(OrderDetail orderDetail, bool saveChanges = true)
        {
            orderDetail.Shipping = 0;
            _context.Entry(orderDetail).State = EntityState.Modified;
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return orderDetail;
        }

        public async Task<ShoppingCart> RemoveAllShippingChargesAsync()
        {
            var shoppingCart = await GetShoppingCartAsync();
            foreach (var orderDetail in shoppingCart.Order.OrderDetails)
            {
                await RemoveShippingChargesAsync(orderDetail, false);
            }
            await _context.SaveChangesAsync();
            return shoppingCart;
        }

        public async Task<OrderDetail> UpdateShippingChargesAsync(OrderDetail orderDetail, bool saveChanges = true)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var shippingScheme = orderDetail.Product.ShippingScheme;

            // if no shipping scheme is specified, simply set shipping charges to default
            if (shippingScheme == null || shoppingCart.Country == null)
            {
                orderDetail.Shipping = orderDetail.Product.Shipping;
                return orderDetail;
            }

            // otherwise, find shipping country rules
            var shippingCountries = shippingScheme.ShippingCountries;
            var shippingCountry = shippingCountries.Find(s => s.Country.IsoCode2 == _shoppingCart.Country
                               && (s.MinQty == null || orderDetail.Quantity >= s.MinQty)
                               && (s.MaxQty == null || orderDetail.Quantity <= s.MaxQty));
            // if country is not found, look for "--" representing "others"
            shippingCountry = shippingCountry ?? shippingCountries.Find(s => s.Country.IsoCode2 == "--"
                                                           && (s.MinQty == null || orderDetail.Quantity >= s.MinQty)
                                                           && (s.MaxQty == null || orderDetail.Quantity <= s.MaxQty));

            // add additional shipping to base shipping
            if (shippingCountry != null)
            {
                if (shippingCountry.FreeShipping)
                {
                    orderDetail.Shipping = 0.00M;
                }
                else
                {
                    decimal baseShipping = shippingCountry.BaseShippingIsPerItem
                        ? orderDetail.Product.Shipping * orderDetail.Quantity
                        : orderDetail.Product.Shipping;
                    decimal additionalShipping = shippingCountry.AdditionalShippingIsPerItem
                        ? shippingCountry.AdditionalShipping * orderDetail.Quantity
                        : shippingCountry.AdditionalShipping;

                    orderDetail.Shipping = baseShipping + additionalShipping;
                }

                _context.Entry(orderDetail).State = EntityState.Modified;
                if (saveChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return orderDetail;
        }

        public async Task<ShoppingCart> UpdateAllShippingChargesAsync()
        {
            var shoppingCart = await GetShoppingCartAsync();
            foreach (var orderDetail in shoppingCart.Order.OrderDetails)
            {
                await UpdateShippingChargesAsync(orderDetail, false);
            }
            await _context.SaveChangesAsync();
            return shoppingCart;
        }
        
    }
}
