﻿/*
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cstieg.ControllerHelper;
using Cstieg.WebFiles.Controllers;
using Cstieg.WebFiles;
using ________________________.Models;
using Newtonsoft.Json;
using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;

namespace ________________________.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Products
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    [RoutePrefix("edit/products")]
    [Route("{action}/{id?}")]
    [ValidateInput(false)]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static string contentFolder = "/content";

        protected IFileService storageService;
        protected ImageManager imageManager;

        public ProductsController()
        {
            storageService = new FileSystemService(contentFolder);
            imageManager = new ImageManager("images/products", storageService);
        }

        // GET: Products
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var products = await db.Products.Include(p => p.ShippingScheme).ToListAsync();
            foreach (var product in products)
            {
                product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();
            }
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            // delete images that were previously saved to newly created product that was not ultimately saved
            foreach (var webImage in await db.WebImages.Where(w => w.ProductId == null).ToListAsync())
            {
                // remove image files used by product
                imageManager.DeleteImageWithMultipleSizes(webImage.ImageUrl);

                db.WebImages.Remove(webImage);
                await db.SaveChangesAsync();
            }

            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Creates a new Product model, saving an image to the default imageManager
        /// </summary>
        /// <param name="product">The Product model passed from the client</param>
        /// <returns>If valid POST, redirect to Product Index; otherwise rerender the Create form</returns>
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();

                // connect images that were previously saved to product (id = null)
                foreach (var webImage in await db.WebImages.Where(w => w.ProductId == null).ToListAsync())
                {
                    webImage.ProductId = product.Id;
                    db.Entry(webImage).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name", product.ShippingSchemeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Pass in list of images for product
            product.WebImages = product.WebImages ?? new List<WebImage>();
            product.WebImages = product.WebImages.OrderBy(w => w.Order).ToList();

            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name");
            return View(product);
        }

        /// <summary>
        /// Edits a Product model, saving a new image to the default imageManager, and deleting the old
        /// </summary>
        /// <param name="product">The Product model passed from the POST request</param>
        /// <returns>If valid POST, redirect to Product Index; otherwise rerender the Edit form</returns>
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name", product.ShippingSchemeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Deletes a Product model, along with the associated image files
        /// </summary>
        /// <param name="id">ID of Product model to be deleted</param>
        /// <returns>Redirect to Product Index</returns>
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

            // Delete images connected to this product
            foreach (var webImage in await db.WebImages.Where(w => w.ProductId == product.Id).ToListAsync())
            {
                // remove image files used by product
                imageManager.DeleteImageWithMultipleSizes(webImage.ImageUrl);

                db.WebImages.Remove(webImage);
                await db.SaveChangesAsync();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Adds an image to the product model
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Json result containing image id</returns>
        [HttpPost]
        public async Task<ActionResult> AddImage(int? id)
        {
            if (id != null)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return this.JError(404, "Can't find product " + id.ToString());
                }
            }

            // Check file is exists and is valid image
            HttpPostedFileBase imageFile = _ModelControllersHelper.GetImageFile(ModelState, Request, "", "file");

            // Save image to disk and store filepath in model
            try
            {
                string timeStamp = FileManager.GetTimeStamp();
                WebImage image = new WebImage
                {
                    ProductId = id,
                    ImageUrl = await imageManager.SaveFile(imageFile, 200, timeStamp),
                    ImageSrcSet = await imageManager.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200 }, timeStamp)
                };
                db.WebImages.Add(image);
                await db.SaveChangesAsync();

                return PartialView("_ProductImagePartial", image);
            }
            catch (Exception e)
            {
                return this.JError(400, "Error saving image: " + e.Message);
            }
        }

        /// <summary>
        /// Deletes an image from the product model
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Json result containing image id</returns>
        [HttpPost]
        public async Task<JsonResult> DeleteImage(int id)
        {
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return this.JError(404, "Can't find product " + id.ToString());
            }

            int imageId = int.Parse(Request.Params.Get("imageId"));
            WebImage image = await db.WebImages.FirstOrDefaultAsync(w => w.Id == imageId);
            if (image == null)
            {
                return this.JError(404, "Can't find image " + imageId.ToString());
            }

            // remove image files used by product
            imageManager.DeleteImageWithMultipleSizes(image.ImageUrl);

            db.WebImages.Remove(image);
            await db.SaveChangesAsync();
            return new JsonResult
            {
                Data = new
                {
                    success = "True",
                    imageId = image.Id
                }
            };
        }

        /// <summary>
        /// Updates the model from the Index table using EditIndex.js
        /// </summary>
        /// <param name="id">The Id of the model to update</param>
        /// <returns>A Json object indicating success status.  
        /// In case of error, returns object with data member containing the old product model,
        /// and the field causing the error if possible</returns>
        [HttpPost]
        public async Task<JsonResult> Update(int id)
        {
            Product existingProduct = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
            {
                return this.JError(404, "Can't find this product to update!");
            }

            try
            {
                Product newProduct = JsonConvert.DeserializeObject<Product>(Request.Params.Get("data"));
                newProduct.Id = id;

                db.Entry(existingProduct).State = EntityState.Detached;
                db.Entry(newProduct).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (JsonReaderException e)
            {
                var returnData = JsonConvert.SerializeObject(new { product = existingProduct, error = e.Message, field = e.Path});
                return this.JError(400, "Invalid data!", returnData);
            }
            catch (Exception e)
            {
                var returnData = JsonConvert.SerializeObject(new { product = existingProduct, error = e.Message });
                return this.JError(400, "Unable to save!", returnData);
            }
            return this.JOk();
        }

        /// <summary>
        /// Saves an image sort to database by numbering the Order field
        /// </summary>
        /// <param name="id">Id of the product whose images to sort</param>
        /// <returns>JSON object indicating success</returns>
        [HttpPost]
        public async Task<JsonResult> OrderWebImages(int? id)
        {
            List <WebImage> webImages= await db.WebImages.Where(w => w.ProductId == id).ToListAsync();

            List<string> imageOrder = JsonConvert.DeserializeObject<List<string>>(Request.Params.Get("imageOrder"));
            for (int i = 0; i < imageOrder.Count(); i++)
            {
                string imageId = imageOrder[i];
                WebImage webImage = await db.WebImages.FirstOrDefaultAsync(w => w.Id == int.Parse(imageId));
                webImage.Order = i;
                db.Entry(webImage).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return this.JOk();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
*/