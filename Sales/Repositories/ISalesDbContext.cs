using Cstieg.Sales.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Cstieg.Sales.Repositories
{
    /// <summary>
    /// Interface for database context that contains the tables needed for the shopping cart system
    /// </summary>
    public interface ISalesDbContext : IDbContext
    {
        DbSet<Store> Stores { get; set; }
        DbSet<ShoppingCart> ShoppingCarts { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<PromoCode> PromoCodes { get; set; }
        DbSet<PromoCodeAdded> PromoCodesAdded { get; set; }
        DbSet<ShippingCountry> ShippingCountries { get; set; }
        DbSet<ShippingScheme> ShippingSchemes { get; set; }
        DbSet<WebImage> WebImages { get; set; }
    }

}
