/*
using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;
using ________________________.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ________________________.Controllers.ModelControllers
{
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    [RoutePrefix("edit/store")]
    [Route("{action}/{id?}")]
    public class StoreController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Store/Edit
        [Route("")]
        public async Task<ActionResult> Edit()
        {
            Store store = await db.Stores.FirstOrDefaultAsync();
            if (store == null)
            {
                store = new Store();
                db.Stores.Add(store);
                await db.SaveChangesAsync();
            }

            return View(store);
        }

        // POST: Store/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Store store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Redirect("/edit");
            }
            return View(store);
        }


    }
}
*/