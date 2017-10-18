using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Cstieg.ShoppingCart
{
    /// <summary>
    /// Controller to provide shopping cart view
    /// </summary>
    public class ShoppingCartController : Controller
    {
        //object ClientInfo = new PayPalApiClient().GetClientSecrets();
        object ClientInfo = new object() { };

        //private DbContext db = new DbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            ViewBag.ClientInfo = ClientInfo;
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            return View(shoppingCart);
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
        /*
        [HttpPost]
        public ActionResult AddPromoCode()
        {
            ShoppingCart shoppingCart = ShoppingCart.GetFromSession(HttpContext);
            try
            {
                string pc = Request.Params.Get("PromoCode");
                PromoCode promoCode = db.PromoCodes.Where(p => p.Code.ToLower() == pc.ToLower()).Single();

                shoppingCart.AddPromoCode(promoCode);

                shoppingCart.SaveToSession(HttpContext);
                return Redirect("Index");
            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: Invalid promo code");
                ViewBag.ClientInfo = ClientInfo;
                return View("Index", shoppingCart);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("PromoCodes", "Failed to add promocode: " + e.Message);
                ViewBag.ClientInfo = ClientInfo;
                return View("Index", shoppingCart);
            }
        }
        */
    }
}
