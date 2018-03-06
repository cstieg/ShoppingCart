/*
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cstieg.Sales.Models;
using _________________________.Models;
using Cstieg.ControllerHelper.ActionFilters;

namespace _________________________.Controllers.ModelControllers
{
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    [RoutePrefix("edit/shippingcountries")]
    [Route("{action}/{id?}")]

    public class ShippingCountriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShippingCountries
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var shippingCountries = db.ShippingCountries.Include(s => s.Country).Include(s => s.ShippingScheme);
            return View(await shippingCountries.ToListAsync());
        }

        // GET: ShippingCountries/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ShippingCountry shippingCountry = await db.ShippingCountries.FindAsync(id);
            if (shippingCountry == null)
            {
                return HttpNotFound();
            }
            return View(shippingCountry);
        }

        // GET: ShippingCountries/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name");
            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name");
            return View();
        }

        // POST: ShippingCountries/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ShippingCountry shippingCountry)
        {
            if (ModelState.IsValid)
            {
                db.ShippingCountries.Add(shippingCountry);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", shippingCountry.CountryId);
            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name", shippingCountry.ShippingSchemeId);
            return View(shippingCountry);
        }

        // GET: ShippingCountries/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ShippingCountry shippingCountry = await db.ShippingCountries.FindAsync(id);
            if (shippingCountry == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", shippingCountry.CountryId);
            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name", shippingCountry.ShippingSchemeId);
            return View(shippingCountry);
        }

        // POST: ShippingCountries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ShippingCountry shippingCountry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingCountry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", shippingCountry.CountryId);
            ViewBag.ShippingSchemeId = new SelectList(db.ShippingSchemes, "Id", "Name", shippingCountry.ShippingSchemeId);
            return View(shippingCountry);
        }

        // GET: ShippingCountries/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            ShippingCountry shippingCountry = await db.ShippingCountries.FindAsync(id);
            if (shippingCountry == null)
            {
                return HttpNotFound();
            }
            return View(shippingCountry);
        }

        // POST: ShippingCountries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingCountry shippingCountry = await db.ShippingCountries.FindAsync(id);
            db.ShippingCountries.Remove(shippingCountry);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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