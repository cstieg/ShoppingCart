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
        private ShoppingCart _shoppingCart;
        private string _ownerId;
        private ISalesDbContext _context;
        
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
            _shoppingCart = await _context.ShoppingCarts.Include(o => o.Order).Include(o => o.Order.OrderDetails)
                .Include(o => o.Order.OrderDetails.Select(p => p.Product))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme.ShippingCountries))
                .Include(o => o.Order.OrderDetails.Select(p => p.Product.ShippingScheme.ShippingCountries.Select(q => q.Country)))
                .Include(o => o.PromoCodesAdded).Include(o => o.PromoCodesAdded.Select(p => p.PromoCode))
                .FirstOrDefaultAsync(s => s.OwnerId == _ownerId);

            _shoppingCart = _shoppingCart ?? ShoppingCart.GetNewShoppingCart();

            _shoppingCart.OwnerId = _ownerId;

            return _shoppingCart;
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

        public async Task<OrderDetail> GetOrderDetailAsync(Product product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            if (shoppingCart.Order == null)
            {
                return null;
            }
            return shoppingCart.Order.OrderDetails.Find(o => o.ProductId == product.Id);
        }

        public async Task<OrderDetail> AddProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await AddProductAsync(product);
        }

        public async Task<OrderDetail> AddProductAsync(Product product)
        {
            ShoppingCart shoppingCart = await GetShoppingCartAsync();

            if (shoppingCart.GetOrderDetails().Any(o => o.ProductId == product.Id))
            {
                throw new ProductAlreadyInShoppingCartException("Product is already in shopping cart");
            }

            var orderDetail = new OrderDetail()
            {
                Order = shoppingCart.Order,
                Product = product,
                ProductId = product.Id,
                PlacedInCart = DateTime.Now,
                Quantity = 1,
                Shipping = product.Shipping,
                UnitPrice = product.Price
            };
            shoppingCart.GetOrderDetails().Add(orderDetail);

            _context.OrderDetails.Add(orderDetail);
            _context.Upsert(shoppingCart);
            _context.Upsert(shoppingCart.Order);
            await _context.SaveChangesAsync();

            return orderDetail;
        }

        public async Task RemoveProductAsync(int productId, bool saveChanges = true)
        {
            var product = await GetProductAsync(productId);
            await RemoveProductAsync(product, saveChanges);
        }

        public async Task RemoveProductAsync(Product product, bool saveChanges = true)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.GetOrderDetails().Find(o => o.ProductId == product.Id);
            shoppingCart.GetOrderDetails().Remove(orderDetail);

            _context.OrderDetails.Remove(orderDetail);

            if (saveChanges)
            {
                // UpdatePromoCodes must be only be called when saving changes, or else there will be a cycle
                await UpdatePromoCodesAsync(false);
                if (shoppingCart.IsEmpty())
                {
                    await DeleteShoppingCartAsync();
                    return;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteShoppingCartAsync()
        {
            var shoppingCart = await GetShoppingCartAsync();
            await RemoveAllPromoCodesAsync(false);
            foreach (var orderDetail in shoppingCart.GetOrderDetails().ToArray())
            {
                _context.OrderDetails.Remove(orderDetail);
            }
            _context.Orders.Remove(shoppingCart.Order);
            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetail> IncrementProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await IncrementProductAsync(product);
        }

        public async Task<OrderDetail> IncrementProductAsync(Product product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.GetOrderDetails().Find(o => o.ProductId == product.Id);

            orderDetail.Quantity++;

            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> DecrementProductAsync(int productId)
        {
            var product = await GetProductAsync(productId);
            return await DecrementProductAsync(product);
        }

        public async Task<OrderDetail> DecrementProductAsync(Product product)
        {
            var shoppingCart = await GetShoppingCartAsync();
            var orderDetail = shoppingCart.GetOrderDetails().Find(o => o.ProductId == product.Id);

            orderDetail.Quantity--;

            await UpdatePromoCodesAsync();

            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return orderDetail;
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
            if (orderDetails.Count != shoppingCart.GetOrderDetails().Count)
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

        public async Task<Order> CheckoutAsync(IAddress shipToAddress, IAddress billToAddress, Customer customer, string cartId)
        {
            try
            {
                var shoppingCart = await GetShoppingCartAsync();
                DateTime currentTime = DateTime.Now;

                // update customer
                var customerInDb = await CreateOrAddCustomerToContextAsync(customer);
                customerInDb.Registered = currentTime;
                customerInDb.LastVisited = currentTime;
                customerInDb.TimesVisited++;

                // update addresses
                var addressesForCustomer = await GetAddressesForCustomerInDbAsync(customerInDb);

                var shipToAddressInDb = CreateOrAddAddressToContext(shipToAddress, addressesForCustomer);

                // if billto address is null or same as shipto, set it equal to shipto and save only once in addresses table
                Address billToAddressInDb = null;
                if (billToAddress == null || billToAddress.IsSame(shipToAddress))
                {
                    billToAddressInDb = shipToAddressInDb;
                }
                else
                {
                    billToAddressInDb = CreateOrAddAddressToContext(billToAddress, addressesForCustomer);
                    billToAddressInDb.Customer = customerInDb;
                }

                SetAddressRecipientIfEmpty(shipToAddressInDb, customerInDb.CustomerName);
                SetAddressRecipientIfEmpty(billToAddressInDb, customerInDb.CustomerName);

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

                await _context.SaveChangesAsync();
                _shoppingCart = shoppingCart = null;

                return orderInDb;
            }
            catch (Exception e)
            {
                throw new InvalidOrderException(e.Message);
            }
        }

        /// <summary>
        /// Creates or adds customer from payment provider service to the database context.
        /// </summary>
        /// <param name="customer">Customer to create or add in database</param>
        /// <returns>Customer created or added to context</returns>
        private async Task<Customer> CreateOrAddCustomerToContextAsync(Customer customer)
        {
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == customer.EmailAddress);

            if (IsNewCustomer(customerInDb))
            {
                customerInDb = customer;
                _context.Customers.Add(customerInDb);
            }
            else
            {
                ObjectHelper.CopyProperties(customer, customerInDb, new List<string>() { "Id" });
                _context.Entry(customerInDb).State = EntityState.Modified;
            }
            return customerInDb;
        }

        /// <summary>
        /// Creates or add address from payment provider service to the database context
        /// </summary>
        /// <param name="address">Address to create or add in database</param>
        /// <param name="addressesForCustomer">List of addresses for the customer</param>
        /// <returns>Address created or added to context</returns>
        private Address CreateOrAddAddressToContext(IAddress address, List<Address> addressesForCustomer)
        {
            var addressInDb = addressesForCustomer.Find(x => x.IsSame(address));

            if (addressInDb == null)
            {
                addressInDb = new Address();
                _context.Addresses.Add(addressInDb);
            }
            else
            {
                _context.Entry(addressInDb).State = EntityState.Modified;
            }
            ObjectHelper.CopyProperties(address, addressInDb, new List<string>() { "Id" });
            return addressInDb;
        }

        /// <summary>
        /// Gets the list of addresses for the customer found in the database
        /// </summary>
        /// <param name="customer">The customer whose addresses to find</param>
        /// <returns>A list of the addresses for the customer.  If none, returns empty list.</returns>
        private async Task<List<Address>> GetAddressesForCustomerInDbAsync(Customer customer)
        {
            var addressesForCustomer = new List<Address>();
            if (!IsNewCustomer(customer))
            {
                addressesForCustomer = await _context.Addresses.Where(x => x.CustomerId == customer.Id).ToListAsync();
            }
            return addressesForCustomer;
        }

        /// <summary>
        /// Checks whether the customer object is new to the database (not found in query or newly added to context)
        /// </summary>
        /// <param name="customer">The customer to check</param>
        /// <returns>True if is a new customer</returns>
        private bool IsNewCustomer(Customer customer)
        {
            return customer == null || _context.Entry(customer).State == EntityState.Added;
        }

        /// <summary>
        /// Sets the address recipient field to a value if the recipient field is empty
        /// </summary>
        /// <param name="address">The address whose recipient field to set</param>
        /// <param name="recipient">The value to which the recipient field will be set</param>
        private void SetAddressRecipientIfEmpty(Address address, string recipient)
        {
            if (address.Recipient.IsEmpty())
            {
                address.Recipient = recipient;
            }
        }

    }
}
