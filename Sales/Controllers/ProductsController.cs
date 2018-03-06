/*
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Cstieg.ControllerHelper;
using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;
using Cstieg.WebFiles.Controllers;
using Cstieg.WebFiles;
using ________________________.Models;

namespace ________________________.Controllers
{
    /// <summary>
    /// Controller to edit product model
    /// </summary>
    [ClearCache]
    [RoutePrefix("edit/products")]
    [Route("{action}/{id?}")]
    [Authorize(Roles = "Administrator")]
    // Allow HTML in ProductInfo field
    [ValidateInput(false)]
    public class ProductsController : BaseController
    {
        // GET: Products
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetAsync(id);
                return View(product);
            }
            catch
            {
                return HttpNotFound();
            }
            
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            // delete images that were previously saved to newly created product that was not ultimately saved
            foreach (var webImage in await _context.WebImages.Where(w => w.ProductId == null).ToListAsync())
            {
                // remove image files used by product
                _productImageManager.DeleteImageWithMultipleSizes(webImage.ImageUrl);

                _context.WebImages.Remove(webImage);
                await _context.SaveChangesAsync();
            }

            ViewBag.ShippingSchemeId = new SelectList(_context.ShippingSchemes, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            await BindProductExtension(Request, product);
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.AddAsync(product);

                    // connect images that were previously saved to product (id = null)
                    foreach (var webImage in await _context.WebImages.Where(w => w.ProductId == null).ToListAsync())
                    {
                        webImage.ProductId = product.Id;
                        _context.Entry(webImage).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return HttpNotFound(e.Message);
                }

            }

            ViewBag.ShippingSchemeId = new SelectList(_context.ShippingSchemes, "Id", "Name", product.ShippingSchemeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetAsync(id);

                ViewBag.ShippingSchemeId = new SelectList(_context.ShippingSchemes, "Id", "Name", product.ShippingSchemeId);
                return View(product);
            }
            catch
            {
                return HttpNotFound();
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(Product product)
        {
            await BindProductExtension(Request, product);
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.EditAsync(product);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return HttpNotFound(e.Message);
                }
            }
            ViewBag.ShippingSchemeId = new SelectList(_context.ShippingSchemes, "Id", "Name", product.ShippingSchemeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetAsync(id);
                return View(product);
            }
            catch
            {
                return HttpNotFound();
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
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
            int? maxImageOrderNo = 0;

            // Allow null id for newly created product
            if (id != null)
            {
                try
                {
                    var product = await _productService.GetAsync((int)id);
                }
                catch
                {
                    return this.JError(404, "Can't find product " + id.ToString());
                }

                // Newly added image should go at end of collection unless purposefully reordered
                maxImageOrderNo = await _context.WebImages.Where(w => w.ProductId == id).MaxAsync(w => w.Order) ?? 0;
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
                    ImageUrl = await _productImageManager.SaveFile(imageFile, 200, timeStamp),
                    ImageSrcSet = await _productImageManager.SaveImageMultipleSizes(imageFile, new List<int>() { 1600, 800, 400, 200 }, timeStamp),
                    Order = maxImageOrderNo + 1
                };
                _context.WebImages.Add(image);
                await _context.SaveChangesAsync();
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
            try
            {
                var product = await _productService.GetAsync(id);
            }
            catch
            {
                return this.JError(404, "Can't find product " + id.ToString());
            }

            int imageId = int.Parse(Request.Params.Get("imageId"));
            WebImage image = await _context.WebImages.FindAsync(imageId);
            if (image == null)
            {
                return this.JError(404, "Can't find image " + imageId.ToString());
            }

            // remove image files used by product
            _productImageManager.DeleteImageWithMultipleSizes(image.ImageUrl);

            _context.WebImages.Remove(image);
            await _context.SaveChangesAsync();
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
        /// <returns>A Json object indicating success status.  In case of error, returns object with data member containing the old product model,
        /// and the field causing the error if possible</returns>
        [HttpPost]
        public async Task<JsonResult> Update(int id)
        {
            Product existingProduct;
     
            try
            {
                existingProduct = await _productService.GetAsync(id);
            }
            catch
            {
                return this.JError(404, "Can't find this product to update!");
            }

            try
            {
                Product newProduct = JsonConvert.DeserializeObject<Product>(Request.Params.Get("data"));
                newProduct.Id = id;

                _context.Entry(existingProduct).State = EntityState.Detached;
                _context.Entry(newProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (JsonReaderException e)
            {
                var returnData = JsonConvert.SerializeObject(new { product = existingProduct, error = e.Message, field = e.Path });
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
            List<WebImage> webImages = await _context.WebImages.Where(w => w.ProductId == id).ToListAsync();

            List<string> imageOrder = JsonConvert.DeserializeObject<List<string>>(Request.Params.Get("imageOrder"));
            for (int i = 0; i < imageOrder.Count(); i++)
            {
                string imageId = imageOrder[i];
                WebImage webImage = await _context.WebImages.FindAsync(int.Parse(imageId));
                webImage.Order = i;
                _context.Entry(webImage).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return this.JOk();
        }

        private async Task BindProductExtension(HttpRequestBase request, Product product)
        {
            ProductExtension productExtension = await _context.ProductExtensions.SingleOrDefaultAsync(p => p.ProductId == product.Id)
                ?? new ProductExtension();
            productExtension.Product = product;
            productExtension.ProductId = product.Id;
            productExtension.Diameter = decimal.Parse(request.Unvalidated.Form.Get("Diameter"));

            product.ProductExtension = productExtension;
        }

    }
}
*/