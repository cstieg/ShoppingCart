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

        public DbSet<Store> Stores { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<PromoCodeAdded> PromoCodesAdded { get; set; }
        public DbSet<ShippingCountry> ShippingCountries { get; set; }
        public DbSet<ShippingScheme> ShippingSchemes { get; set; }
        public DbSet<WebImage> WebImages { get; set; }
    }
}
