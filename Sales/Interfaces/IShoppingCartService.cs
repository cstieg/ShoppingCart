using Cstieg.Sales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cstieg.Sales.Interfaces
{
    /// <summary>
    /// Interface for shopping cart service
    /// </summary>
    public interface IShoppingCartService
    {
        /// <summary>
        /// Gets the shopping cart saved for this shopping cart service from database, or to save a database call, from the instance variable if it already exists
        /// </summary>
        /// <returns>The current shopping cart</returns>
        Task<ShoppingCart> GetShoppingCartAsync();

        /// <summary>
        /// Gets an order detail from database by id
        /// </summary>
        /// <param name="id">The id of the order detail to get</param>
        /// <returns>The order detail</returns>
        Task<OrderDetail> GetOrderDetailAsync(int id);

        /// <summary>
        /// Gets an order detail from database by product
        /// </summary>
        /// <param name="product">The product which is contained in the order detail</param>
        /// <returns>The order detail</returns>
        Task<OrderDetail> GetOrderDetailAsync(Product product);

        /// <summary>
        /// Gets a product from the database by id
        /// </summary>
        /// <param name="id">The id of the product to get</param>
        /// <returns>The project object</returns>
        Task<Product> GetProductAsync(int id);

        /// <summary>
        /// Adds a product to the shopping cart as an order detail
        /// </summary>
        /// <param name="product">The product to add to the shopping cart</param>
        /// <returns>The order detail added</returns>
        Task<OrderDetail> AddProductAsync(Product product);
        Task<OrderDetail> AddProductAsync(int productId);

        /// <summary>
        /// Removes a product order detail from the shopping cart
        /// </summary>
        /// <param name="product">The product to remove from the shopping cart</param>
        /// <param name="saveChanges">Whether to save changes to the database.  Default: true</param>
        Task RemoveProductAsync(Product product, bool saveChanges);
        Task RemoveProductAsync(int productId, bool saveChanges);

        /// <summary>
        /// Clears all order details from the shopping cart
        /// </summary>
        Task DeleteShoppingCartAsync();

        /// <summary>
        /// Increases the quantity of a product in the shopping cart by 1
        /// </summary>
        /// <param name="product">The product to increment</param>
        /// <returns>The incremented order detail</returns>
        Task<OrderDetail> IncrementProductAsync(Product product);
        Task<OrderDetail> IncrementProductAsync(int productId);

        /// <summary>
        /// Decreases the quantity of a product in the shopping cart by 1
        /// </summary>
        /// <param name="product">The product to decrement</param>
        /// <returns>The decremented order detail</returns>
        Task<OrderDetail> DecrementProductAsync(Product product);
        Task<OrderDetail> DecrementProductAsync(int productId);

        /// <summary>
        /// Removes shipping charges from a single order detail
        /// </summary>
        /// <param name="orderDetail">The order detail to remove charges from</param>
        /// <param name="saveChanges">Whether to save changes to database.  Default: true</param>
        /// <returns>The order detail with shipping charges removed</returns>
        Task<OrderDetail> RemoveShippingChargesAsync(OrderDetail orderDetail, bool saveChanges);

        /// <summary>
        /// Removes shipping charges from all order details in an order
        /// </summary>
        /// <returns>The shopping cart with shipping charges removed</returns>
        Task<ShoppingCart> RemoveAllShippingChargesAsync();

        /// <summary>
        /// Updates shipping charges on an order detail according to a shipping scheme, or resets to default shipping if no shipping scheme is available
        /// </summary>
        /// <param name="orderDetail">The order detail whose shipping charges to update</param>
        /// <param name="saveChanges">Whether to save changes to database. Default: true</param>
        /// <returns>The order detail with shipping charges updated</returns>
        Task<OrderDetail> UpdateShippingChargesAsync(OrderDetail orderDetail, bool saveChanges);

        /// <summary>
        /// Updates shipping charges on all the order details in an order according to a shipping scheme, 
        /// or resets to default shipping if no shipping scheme is available
        /// </summary>
        /// <returns>Shipping cart with shipping charges updated</returns>
        Task<ShoppingCart> UpdateAllShippingChargesAsync();

        /// <summary>
        /// Looks up and adds a promocode and updates the shopping cart accordingly
        /// </summary>
        /// <param name="code">The code to add</param>
        /// <returns>The shopping cart with the promocode added</returns>
        Task<ShoppingCart> AddPromoCodeAsync(string code);

        /// <summary>
        /// Adds a promocode and updates the shopping cart accordingly
        /// </summary>
        /// <param name="promoCode">The code object to add</param>
        /// <returns>The shopping cart with the promocode added</returns>>
        Task<ShoppingCart> AddPromoCodeAsync(PromoCode promoCode);

        /// <summary>
        /// Removes all promocodes added from database and shopping cart
        /// </summary>
        /// <param name="saveChanges">Whether to save changes to database. Default: true</param>
        /// <returns>The shopping cart with the promocodes removed</returns>
        Task<ShoppingCart> RemoveAllPromoCodesAsync(bool saveChanges);

        /// <summary>
        /// Makes sure all promocodes are up to date by removing them and adding them again if they are eligible.
        /// </summary>        
        /// <param name="saveChanges">Whether to save changes to database. Default: true</param>
        /// <returns>The shopping cart with the promocodes updated</returns>
        Task<ShoppingCart> UpdatePromoCodesAsync(bool saveChanges);

        /// <summary>
        /// Sets the country of the shopping cart for shipping purposes, and updates shipping charges accordingly
        /// </summary>
        /// <param name="countryCode">The 2 letter ISO code of the country</param>
        /// <returns>The shopping cart with the country set</returns>
        Task<ShoppingCart> SetCountryAsync(string countryCode);

        /// <summary>
        /// Verifies the order details at checkout to ensure the user has not maliciously changed the JSON payment data passed to the payment processor
        /// </summary>
        /// <param name="OrderDetails">The order details received from the user</param>
        Task VerifyOrderDetailsAsync(List<OrderDetail> OrderDetails, decimal total);

        /// <summary>
        /// Verifies the country at checkout to ensure the country of the shipping address matches the country used for calculating shipping charges
        /// </summary>
        /// <param name="countryCode">The country code of the shipping address received from the user</param>
        Task VerifyCountryAsync(string countryCode);

        /// <summary>
        /// Finalizes the order in the database at checkout, saving customer and address to the database and clearing the shopping cart
        /// </summary>
        /// <param name="shipToAddress">The ship to address received from the user at checkout</param>
        /// <param name="billToAddress">The bill to address received from the user at checkout</param>
        /// <param name="customer">The customer object containing customer name and email received from the user at checkout</param>
        /// <param name="cartId">The shopping cart id for the order received from the payment processor</param>
        /// <returns>The updated and saved order object</returns>
        Task<Order> CheckoutAsync(IAddress shipToAddress, IAddress billToAddress, Customer customer, string cartId);
    }
}
