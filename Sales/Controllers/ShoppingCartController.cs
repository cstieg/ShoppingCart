/*
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Cstieg.Sales.Models;
using Cstieg.Sales.PayPal;
using ____________.Models;

namespace ____________.Controllers
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : BaseController
    {
        // if using ShoppingCart.PayPal, uncomment this next line and delete the following
        //ClientInfo ClientInfo = new PayPalApiClient().GetClientSecrets();
        object ClientInfo = null;

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public async Task<ActionResult> Index()
        {
            ViewBag.ClientInfo = ClientInfo;
            ViewBag.Countries = await db.Countries.ToListAsync();
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            return View(shoppingCart);
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        /// <summary>
        /// Gets the number of items in the shopping cart
        /// </summary>
        /// <returns>A JSON object containing the number of items in the shopping cart in the field shoppingCartCount</returns>
        public JsonResult ShoppingCartCount()
        {
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            object returnData = new
            {
                shoppingCartCount = shoppingCart.Order.OrderDetails.Count
            };
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a product to the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response if successful, error response if product already exists</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Add new order detail to session
            try
            {
                shoppingCart.AddProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(403, e.Message);
            }
        }

        /// <summary>
        /// Increases the quantity of a product in the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to add</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncrementItem(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Increment quantity and save shopping cart
            try
            {
                var orderDetail = shoppingCart.IncrementProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return Json(orderDetail);
                //return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(403, e.Message);
            }

        }

        /// <summary>
        /// Decreases the quantity of an item in the shopping cart
        /// </summary>
        /// <param name="id">ID of the Product model qty to decrement</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DecrementItem(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Decrement qty and update shopping cart in session
            try
            {
                var orderDetail = shoppingCart.DecrementProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return Json(orderDetail);
                //return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(403, e.Message);
            }
        }

        /// <summary>
        /// Removes a Product from the shopping cart
        /// </summary>
        /// <param name="id">ID of Product model to remove</param>
        /// <returns>JSON success response</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int id)
        {
            // look up product entity
            Product product = db.Products.SingleOrDefault(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve shopping cart from session
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);

            // Remove Product and update shopping cart in session
            try
            {
                shoppingCart.RemoveProduct(product);
                shoppingCart.SaveToSession(HttpContext);
                return this.JOk();
            }
            catch (Exception e)
            {
                return this.JError(403, e.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetCountry()
        {
            string country = Request.Params.Get("country");
            var cart = ShoppingCart.GetFromSession(HttpContext);
            cart.Country = country;
            cart.UpdateShippingCharges();
            cart.SaveToSession(HttpContext);
            return Json(cart);
        }
    }
}
*/