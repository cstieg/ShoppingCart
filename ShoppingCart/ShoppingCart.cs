using Cstieg.SessionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cstieg.ShoppingCart
{
    /// <summary>
    /// A viewmodel container for an Order
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// Constructor that initializes objects to avoid null object errorss
        /// </summary>
        public ShoppingCart()
        {
            Order = new Order();
            PromoCodes = new List<PromoCode>();
        }

        public Order Order { get; set; }

        public string PayeeEmail { get; set; }

        public List<PromoCode> PromoCodes { get; set; }

        public decimal TotalExtendedPrice
        {
            get
            {
                return Order.OrderDetails.Sum(p => p.ExtendedPrice);
            }
        }

        public decimal TotalShipping
        {
            get
            {
                return Order.OrderDetails.Sum(p => p.Shipping);
            }
        }

        public decimal GrandTotal
        {
            get
            {
                return TotalExtendedPrice + TotalShipping;
            }
        }

        /// <summary>
        /// Returns list of item in the shopping cart
        /// </summary>
        /// <returns>List of OrderDetails in the shopping cart</returns>
        public List<OrderDetail> GetItems()
        {
            // TODO: return clone to prevent writing to the data outside the class?
            return Order.OrderDetails;
        }


        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="product">The Product to add</param>
        public void AddProduct(ProductBase product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.Id == product.Id);
            if (orderDetail == null)
            {
                orderDetail = new OrderDetail()
                {
                    Product = product,
                    PlacedInCart = DateTime.Now,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    Shipping = product.Shipping
                };
                Order.OrderDetails.Add(orderDetail);
            }
            else
            {
                throw new Exception("Product is already in shopping cart!");
            }
        }

        /// <summary>
        /// Increments the quantity of a product in the shopping cart
        /// </summary>
        /// <param name="product">The Product whose quantity to increment</param>
        public void IncrementProduct(ProductBase product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.Id == product.Id);
            if (orderDetail == null)
            {
                throw new Exception("Product is not in shopping cart!");
            }
            else
            {
                orderDetail.Quantity++;
            }
        }

        /// <summary>
        /// Decrements the quantity of a product in the shopping cart, removes if none remaining after decrement
        /// </summary>
        /// <param name="product">The Product whose quantity to decrement</param>
        public void DecrementProduct(ProductBase product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.Id == product.Id);
            if (!(orderDetail == null) && orderDetail.Quantity > 0)
            {
                orderDetail.Quantity--;
            }
            if (orderDetail.Quantity == 0)
            {
                RemoveProduct(product);
            }
        }

        /// <summary>
        /// Removes all quantities of a product in the shopping cart, clears all promocodes if no items remain
        /// </summary>
        /// <param name="product">The Product to remove</param>
        public void RemoveProduct(ProductBase product)
        {
            OrderDetail orderDetail = Order.OrderDetails.Find(p => p.Product.Id == product.Id);
            if (!(orderDetail == null))
            {
                Order.OrderDetails.Remove(orderDetail);
            }

            if (Order.OrderDetails.Count == 0)
            {
                PromoCodes.Clear();
            }
        }

        /// <summary>
        /// Removes shipping charges from an item in the shopping cart
        /// </summary>
        /// <param name="orderDetail"></param>
        public void RemoveShippingCharges(OrderDetail orderDetail)
        {
            orderDetail.Shipping = 0;
        }

        /// <summary>
        /// Removes all shipping charges from the shopping cart
        /// </summary>
        public void RemoveAllShippingCharges()
        {
            for (int i = 0; i < Order.OrderDetails.Count; i++)
            {
                RemoveShippingCharges(Order.OrderDetails[i]);
            }
        }

        /// <summary>
        /// Adds and processes a promo code
        /// </summary>
        /// <param name="promoCode"></param>
        public void AddPromoCode(PromoCode promoCode)
        {
            // check constraints
            if (PromoCodes.Exists(p => p.Code == promoCode.Code))
            {
                throw new Exception("Cannot enter same promo code twice!");
            }

            if (promoCode.SpecialPriceItem != null && !Order.OrderDetails.Exists(o => o.ProductId == promoCode.SpecialPriceItem.Id))
            {
                throw new Exception("Must purchase special item to receive discount: " + promoCode.SpecialPriceItem.Name);
            }

            if (promoCode.WithPurchaseOf != null && !Order.OrderDetails.Exists(o => o.ProductId == promoCode.WithPurchaseOf.Id))
            {
                throw new Exception("Must purchase qualifying item: " + promoCode.WithPurchaseOf.Name);
            }

            if (promoCode.MinimumQualifyingPurchase != null && TotalExtendedPrice < promoCode.MinimumQualifyingPurchase)
            {
                throw new Exception("Must have a minimum purchase of $" + promoCode.MinimumQualifyingPurchase.ToString());
            }

            if (promoCode.CodeStart != null && DateTime.Now < promoCode.CodeStart)
            {
                DateTime codeStart = (DateTime)promoCode.CodeStart;
                throw new Exception("Code is not valid until " + codeStart.ToShortDateString());
            }

            if (promoCode.CodeEnd != null && DateTime.Now > promoCode.CodeEnd)
            {
                throw new Exception("Code is expired!");
            }

            // Apply promotions
            if (promoCode.PromotionalItem != null)
            {
                Order.OrderDetails.Add(new OrderDetail()
                {
                    Product = promoCode.PromotionalItem,
                    UnitPrice = promoCode.PromotionalItemPrice ?? 0,
                    Quantity = 1,
                    IsPromotionalItem = true,
                    PlacedInCart = DateTime.Now,
                    Shipping = 0
                });
            }

            // special price may be zero; only apply if a percentage is not given
            if ((promoCode.PercentOffItem == null || promoCode.PercentOffItem == 0) && promoCode.SpecialPriceItem != null)
            {
                OrderDetail item = Order.OrderDetails.Find(o => o.Product.Id == promoCode.SpecialPriceItem.Id);
                item.UnitPrice = promoCode.SpecialPrice ?? 0;
            }

            // percent off item
            if (promoCode.PercentOffItem != null && promoCode.PercentOffItem > 0 && promoCode.SpecialPriceItem != null)
            {
                OrderDetail item = Order.OrderDetails.Find(o => o.Product.Id == promoCode.SpecialPriceItem.Id);
                item.UnitPrice = PercentOff(item.UnitPrice, promoCode.PercentOffItem ?? 0);
            }

            // percent off order
            if (promoCode.PercentOffOrder != null && promoCode.PercentOffOrder > 0)
            {
                for (int i = 0; i < Order.OrderDetails.Count; i++)
                {
                    OrderDetail item = Order.OrderDetails[i];
                    item.UnitPrice = PercentOff(item.UnitPrice, promoCode.PercentOffOrder ?? 0);
                }
            }

            // Add promocode to list
            PromoCodes.Add(promoCode);
        }

        /// <summary>
        /// Gets the shopping cart from the session
        /// </summary>
        /// <param name="context">The HttpContext object from the controller</param>
        /// <returns>The shopping cart object</returns>
        public static ShoppingCart GetFromSession(HttpContextBase context)
        {
            ShoppingCart shoppingCart = context.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            // Create new shopping cart if none is in session
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart();
            }
            return shoppingCart;
        }

        /// <summary>
        /// Saves the shopping cart to the session
        /// </summary>
        /// <param name="context">The HttpContext object from the controller</param>
        public void SaveToSession(HttpContextBase context)
        {
            context.Session.SetObjectAsJson("_shopping_cart", this);
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
            return PercentOff(originalPrice, (decimal) percentOff);
        }
        
    }
}
