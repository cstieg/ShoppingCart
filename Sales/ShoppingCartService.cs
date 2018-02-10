using Cstieg.ObjectHelpers;
using Cstieg.Sales.Exceptions;
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
    /// <summary>
    /// Service that models shopping cart functionality.
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        protected ShoppingCart _shoppingCart;
        protected string _ownerId;
        protected ISalesDbContext _context;

        /// <summary>
        /// Constructor for ShoppingCartService
        /// </summary>
        /// <param name="context">Database context for saving the shopping cart</param>
        /// <param name="ownerId">Cookie text identifying the shopping cart owner</param>
        public ShoppingCartService(ISalesDbContext context, string ownerId)
        {
            _ownerId = ownerId;
            _context = context;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync()
        {
            // Don't make another database call if a populated shopping cart object is in the service instance
            if (_shoppingCart != null)
            {
                return _shoppingCart;
            }

            // Get the shopping cart object from the repository including order and order details
            _shoppingCart = await _context.ShoppingCarts.Include(o => o.Order).Include(o => o.Order.OrderDetails).Include(o => o.Order.OrderDetails)
                .Include(o => o.Order.OrderDetails.Select(p => p.Product))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme.ShippingCountries))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme.ShippingCountries.Select(q => q.Country)))
                .Include(o => o.PromoCodesAdded).Include(o => o.PromoCodesAdded.Select(p => p.PromoCode))
                .FirstOrDefaultAsync(s => s.OwnerId == _ownerId);

            // Return newly saved shopping cart if not already existing in repository
            if (_shoppingCart == null)
            {
                _shoppingCart = new ShoppingCart
                {
                    OwnerId = _ownerId
                };
                _context.ShoppingCarts.Add(_shoppingCart);
                await _context.SaveChangesAsync();
            }

            return _shoppingCart;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync()
        {
            var shoppingCart = await GetShoppingCartAsync();
            if (shoppingCart.Order == null)
            {
                return new List<OrderDetail>();
            }
            return shoppingCart.Order.OrderDetails;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            try
            {
                return await _context.Products.FirstAsync(p => p.Id == id);
            }
            catch
            {
                throw new NotFoundException("Product not found in database.  Check id!");
            }
        }
        
        public async Task<OrderDetail> GetOrderDetailAsync(int id)
        {
            return await _context.OrderDetails.FirstAsync(o => o.Id == id);
        }

        public async Task<OrderDetail> GetOrderDetailAsync(IProduct product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            if (shoppingCart.Order == null)
            {
                return null;
            }
            return shoppingCart.Order.OrderDetails.Find(o => o.ProductId == product.Id);
        }

        public async Task<OrderDetail> AddProductAsync(IProduct product)
        {
            ShoppingCart shoppingCart = await GetShoppingCartAsync();
            await AddOrderIfNullAsync(shoppingCart);

            if (shoppingCart.GetOrderDetails().Any(o => o.ProductId == product.Id))
            {
                throw new ProductAlreadyInShoppingCartException("Product is already in shopping cart");
            }

            var orderDetail = new OrderDetail()
            {
                OrderId = shoppingCart.Order.Id,
                Product = (Product)product,
                ProductId = product.Id,
                PlacedInCart = DateTime.Now,
                Quantity = 1,
                Shipping = product.Shipping,
                UnitPrice = product.Price
            };
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return orderDetail;
        }

        public async Task<OrderDetail> AddProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await AddProductAsync(product);
        }

        public async Task RemoveProductAsync(IProduct product, bool saveChanges = true, bool update = true)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.Order.OrderDetails.Find(o => o.ProductId == product.Id);
            shoppingCart.Order.OrderDetails.Remove(orderDetail);

            _context.OrderDetails.Remove(orderDetail);

            if (update)
            {
                await UpdatePromoCodesAsync(false);
                await DeleteOrderIfEmptyAsync(shoppingCart);
            }
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

        }

        public async Task RemoveProductAsync(int productId, bool saveChanges = true, bool update = true)
        {
            var product = await GetProductAsync(productId);
            await RemoveProductAsync(product, saveChanges, update);
        }

        public async Task ClearShoppingCartAsync()
        {
            var shoppingCart = await GetShoppingCartAsync();
            await RemoveAllPromoCodesAsync(false);
            foreach (var orderDetail in shoppingCart.Order.OrderDetails.ToArray())
            {
                _context.OrderDetails.Remove(orderDetail);
            }
            _context.Orders.Remove(shoppingCart.Order);
            await _context.SaveChangesAsync();

            shoppingCart.Order = null;
            shoppingCart.OrderId = null;
        }

        public async Task<OrderDetail> IncrementProductAsync(IProduct product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.Order.OrderDetails.Find(o => o.ProductId == product.Id);
            orderDetail.Quantity++;
            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> IncrementProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await IncrementProductAsync(product);
        }

        public async Task<OrderDetail> DecrementProductAsync(IProduct product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.Order.OrderDetails.Find(o => o.ProductId == product.Id);
            orderDetail.Quantity--;
            _context.Entry(orderDetail).State = EntityState.Modified;
            await UpdatePromoCodesAsync();
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> DecrementProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await DecrementProductAsync(product);
        }

        private async Task<ShoppingCart> ResetPricesAsync(bool saveInDb = true)
        {
            var shoppingCart = await GetShoppingCartAsync();
            foreach (var orderDetail in shoppingCart.GetOrderDetails())
            {
                orderDetail.UnitPrice = orderDetail.Product.Price;
                _context.Entry(orderDetail).State = EntityState.Modified;
            }

            if (saveInDb)
            {
                await _context.SaveChangesAsync();
            }

            return shoppingCart;
        }

        public async Task<ShoppingCart> SetCountryAsync(string countryCode)
        {
            var shoppingCart = await GetShoppingCartAsync();
            shoppingCart.Country = countryCode;
            await UpdateAllShippingChargesAsync();
            return shoppingCart;
        }

        public async Task VerifyOrderDetailsAsync(List<OrderDetail> orderDetails, decimal total)
        {
            var shoppingCart = await GetShoppingCartAsync();
            if (orderDetails.Count != shoppingCart.Order.OrderDetails.Count)
            {
                throw new InvalidOrderException("Your shopping cart has been changed! Please try again.");
            }

            foreach (var orderDetail in orderDetails)
            {
                var shoppingCartOrderDetail = shoppingCart.Order.OrderDetails.Find(o => o.ProductId == orderDetail.ProductId);
                if (shoppingCartOrderDetail == null
                    || shoppingCartOrderDetail.UnitPrice != orderDetail.UnitPrice
                    || shoppingCartOrderDetail.Quantity != orderDetail.Quantity
                    || shoppingCartOrderDetail.Shipping != orderDetail.Shipping
                    || shoppingCart.GrandTotal != total)
                {
                    throw new InvalidOrderException("Your shopping cart has been changed! Please try again.");
                }
            }
        }

        public async Task VerifyCountryAsync(string countryCode)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var countries = await _context.Countries.ToListAsync();
            if (shoppingCart.Country == "--" && countries != null)
            {
                // if country in shopping cart is listed as other, make sure actual country is not in list of shopping cart countries
                if (countries.Exists(c => c.IsoCode2 == countryCode))
                {
                    throw new InvalidOrderException("Please select your country from the list");
                }
            }
            else
            {
                // make sure shopping cart country matches actual country
                if (shoppingCart.Country != countryCode)
                {
                    throw new InvalidOrderException("Your country does not match the country selected!");
                }
            }
        }

        public async Task<Order> CheckoutAsync(IAddress shipToAddress, IAddress billToAddress, ICustomer customer, string cartId)
        {
            var shoppingCart = await GetShoppingCartAsync();
            DateTime currentTime = DateTime.Now;

            // Check to see if customer already exists.  If so, update, otherwise create new.
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == customer.EmailAddress);
            var isNewCustomer = customerInDb == null;

            customerInDb = customerInDb ?? (Customer)customer;
            if (isNewCustomer)
            {
                customerInDb.Registered = currentTime;
                _context.Customers.Add(customerInDb);
            }
            else
            {
                ObjectHelper.CopyProperties(customer, customerInDb, new List<string>() { "Id" });
                customerInDb.Registered = currentTime;
                _context.Entry(customerInDb).State = EntityState.Modified;
            }
            customerInDb.LastVisited = currentTime;
            customerInDb.TimesVisited++;


            // Check to see if addresses already exist.  If so, update, otherwise create new.
            Address shipToAddressInDb = null;
            Address billToAddressInDb = null;
            var addressesForCustomer = new List<Address>();
            if (!isNewCustomer)
            {
                addressesForCustomer = await _context.Addresses.Where(x => x.CustomerId == customerInDb.Id).ToListAsync();
            }

            shipToAddressInDb = addressesForCustomer.Find(x => x.IsSame(shipToAddress));
            
            if (shipToAddressInDb == null)
            {
                shipToAddressInDb = new Address();
                ObjectHelper.CopyProperties(shipToAddress, shipToAddressInDb, new List<string>() { "Id" });
                shipToAddressInDb.Customer = customerInDb;
                _context.Addresses.Add(shipToAddressInDb);
            }
            else
            {
                ObjectHelper.CopyProperties(shipToAddress, shipToAddressInDb, new List<string>() { "Id" });
                _context.Entry(shipToAddressInDb).State = EntityState.Modified;
            }

            // if billto address is null or same as shipto, set it equal to shipto and save only once in addresses table
            if (billToAddress == null || billToAddress.IsSame(shipToAddress))
            {
                billToAddressInDb = shipToAddressInDb;
            }
            else
            {

                billToAddressInDb = addressesForCustomer.Find(x => x.IsSame(billToAddress));
                if (billToAddressInDb == null)
                {
                    // new billto address that is different from shipto
                    billToAddressInDb = new Address();
                    ObjectHelper.CopyProperties(billToAddress, billToAddressInDb, new List<string>() { "Id" });
                    billToAddressInDb.Customer = customerInDb;
                    _context.Entry(billToAddressInDb).State = EntityState.Modified;
                }
                else
                {
                    // old shipto address that is different from shipto
                    ObjectHelper.CopyProperties(billToAddress, billToAddressInDb, new List<string>() { "Id" });
                    _context.Entry(billToAddressInDb).State = EntityState.Modified;
                }
            }

            // Set customer name as recipient in address if recipient is empty
            if (shipToAddressInDb.Recipient.IsEmpty())
            {
                shipToAddressInDb.Recipient = customerInDb.CustomerName;
            }
            if (billToAddressInDb.Recipient.IsEmpty())
            {
                billToAddressInDb.Recipient = customerInDb.CustomerName;
            }

            // Reference customer in addresses
            shipToAddressInDb.Customer = customerInDb;
            billToAddressInDb.Customer = customerInDb;

            // Reference customer and addresses in order
            var orderInDb = shoppingCart.Order;
            orderInDb.Customer = customerInDb;
            orderInDb.ShipToAddress = shipToAddressInDb;
            orderInDb.BillToAddress = billToAddressInDb;

            // Update order time and cart id
            orderInDb.DateOrdered = currentTime;
            orderInDb.Cart = cartId;
            _context.Entry(orderInDb).State = EntityState.Modified;

            // Delete shopping cart and clear instance variable
            _context.ShoppingCarts.Remove(shoppingCart);


            try
            {
                await _context.SaveChangesAsync();
                _shoppingCart = shoppingCart = null;
            }
            catch (Exception e)
            {
                throw new InvalidOrderException(e.Message);
            }
            return orderInDb;
        }


        /// <summary>
        /// Adds an Order object to the shopping cart if the Order property in the shopping cart is null
        /// </summary>
        /// <param name="shoppingCart">The shopping cart to add the Order to</param>
        /// <returns>A reference to the new or existing Order object</returns>
        protected async Task<Order> AddOrderIfNullAsync(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Order == null)
            {
                shoppingCart.Order = new Order
                {
                    OrderDetails = new List<OrderDetail>()
                };
                _context.Orders.Add(shoppingCart.Order);
                await _context.SaveChangesAsync();
            }
            return shoppingCart.Order;
        }

        /// <summary>
        /// Deletes the Order if it is empty, so as not to save null orders to database
        /// </summary>
        /// <param name="shoppingCart">The shopping cart whose null order to delete</param>
        protected async Task DeleteOrderIfEmptyAsync(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Order.OrderDetails.Count == 0)
            {
                await ClearShoppingCartAsync();
            }
        }

    }
}
