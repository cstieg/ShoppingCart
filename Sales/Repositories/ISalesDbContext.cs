using Cstieg.Sales.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Cstieg.Sales.Repositories
{
    /// <summary>
    /// Interface for database context that contains the tables needed for the shopping cart system
    /// </summary>
    public interface ISalesDbContext
    {
        IDbSet<Store> Stores { get; set; }
        IDbSet<ShoppingCart> ShoppingCarts { get; set; }
        IDbSet<Address> Addresses { get; set; }
        IDbSet<Country> Countries { get; set; }
        IDbSet<Customer> Customers { get; set; }
        IDbSet<Order> Orders { get; set; }
        IDbSet<OrderDetail> OrderDetails { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<PromoCode> PromoCodes { get; set; }
        IDbSet<PromoCodeAdded> PromoCodesAdded { get; set; }
        IDbSet<ShippingCountry> ShippingCountries { get; set; }
        IDbSet<ShippingScheme> ShippingSchemes { get; set; }
        IDbSet<WebImage> WebImages { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync();
    }

}
