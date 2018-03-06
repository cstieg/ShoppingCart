/*
using _____________________.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _____________________.Controllers
{
    [OutputCache(CacheProfile = "CacheForAWeek")]
    public class HomeController : BaseController
    {
        // GET: /
        public ActionResult Index()
        {
            return View();
        }

        // GET: Products
        public async Task<ActionResult> Products()
        {
            var products = await _productService.GetDisplayProductsAsync();
            return View(products);
        }

        // GET: Product?circumference=10.8&unit=inches
        // GET: Product/1
        // GET: Product/StovepipeHeatSaver for 6" stove pipe
        /// <summary>
        /// Routes product page request to the appropriate controller method
        /// </summary>
        /// <param name="id">May be an integer id or string product name</param>
        public async Task<ActionResult> Product(string id)
        {
            try
            {
                return await ProductById(int.Parse(id));
            }
            catch
            {
                return await ProductByUrlName(id);
            }
        }

        // GET: Product/1
        public async Task<ActionResult> ProductById(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View("Product", product);
        }

        // GET: Product/StovepipeHeatSaver for 6" stove pipe
        public async Task<ActionResult> ProductByUrlName(string productName)
        {
            var product = await _productService.GetByUrlNameAsync(productName);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View("Product", product);
        }

        /// <summary>
        /// Displays list of links to model edit pages
        /// </summary>
        public ActionResult Edit()
        {
            string modelControllers = ConfigurationManager.AppSettings["modelControllers"];
            char[] delimiters = { ',' };
            string[] controllersArray = modelControllers.Split(delimiters);
            List<string> controllers = new List<string>(controllersArray);
            return View(controllers);
        }
    }
}
*/