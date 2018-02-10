﻿/*
using System.Data.Entity;
using System.Web.Mvc;
using ________________________.Models;
using System.Threading.Tasks;
using Cstieg.ControllerHelper.ActionFilters;
using Cstieg.Sales.Models;

namespace ________________________.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for PromoCodes
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [RoutePrefix("edit/promocodes")]
    [Route("{action}/{id?}")]
    [ClearCache]
    public class PromoCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PromoCodes
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var promoCodes = db.PromoCodes.Include(p => p.PromotionalItem).Include(p => p.WithPurchaseOf);
            return View(await promoCodes.ToListAsync());
        }

        // GET: PromoCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            return View(promoCode);
        }

        // GET: PromoCodes/Create
        public ActionResult Create()
        {
            ViewBag.PromotionalItemList = new SelectList(db.Products, "Id", "Name");
            ViewBag.WithPurchaseOfList = new SelectList(db.Products, "Id", "Name");
            ViewBag.SpecialPriceItemList = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: PromoCodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.PromoCodes.Add(promoCode);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PromotionalItemList = new SelectList(db.Products, "Id", "Name", promoCode.PromotionalItemId);
            ViewBag.WithPurchaseOfList = new SelectList(db.Products, "Id", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemList = new SelectList(db.Products, "Id", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.PromotionalItemList = new SelectList(db.Products, "Id", "Name", promoCode.PromotionalItemId);
            ViewBag.WithPurchaseOfList = new SelectList(db.Products, "Id", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemList = new SelectList(db.Products, "Id", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // POST: PromoCodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promoCode).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PromotionalItemId = new SelectList(db.Products, "Id", "Name");
            ViewBag.WithPurchaseOfId = new SelectList(db.Products, "Id", "Name", promoCode.WithPurchaseOfId);
            ViewBag.SpecialPriceItemId = new SelectList(db.Products, "Id", "Name", promoCode.SpecialPriceItemId);
            return View(promoCode);
        }

        // GET: PromoCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            PromoCode promoCode = await db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
            if (promoCode == null)
            {
                return HttpNotFound();
            }
            return View(promoCode);
        }

        // POST: PromoCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PromoCode promoCode = await db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
            db.PromoCodes.Remove(promoCode);
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