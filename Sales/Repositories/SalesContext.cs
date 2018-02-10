using Cstieg.Sales.Models;
using System.Data.Common;
using System.Data.Entity;

namespace Cstieg.Sales.Repositories
{
    public class SalesContext : DbContext, ISalesDbContext
    {
        public SalesContext() : base() { }
        public SalesContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public SalesContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        public IDbSet<Store> Stores { get; set; }
        public IDbSet<ShoppingCart> ShoppingCarts { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<Country> Countries { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderDetail> OrderDetails { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<PromoCode> PromoCodes { get; set; }
        public IDbSet<PromoCodeAdded> PromoCodesAdded { get; set; }
        public IDbSet<ShippingCountry> ShippingCountries { get; set; }
        public IDbSet<ShippingScheme> ShippingSchemes { get; set; }
        public IDbSet<WebImage> WebImages { get; set; }
    }
}
