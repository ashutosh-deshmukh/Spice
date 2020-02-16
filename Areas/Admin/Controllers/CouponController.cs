using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync()); ;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1= files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(coupons);
        }

        // Edit _-- Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // Edit -- Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (Coupon coupons)
        {
            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = await _db.Coupon.Where(m => m.Id == coupons.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                couponFromDb.Name = coupons.Name;
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.IsActive = coupons.IsActive;

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = p1;
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(coupons);

        }

        // Get _ Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(m => m.Id == id).SingleOrDefaultAsync();

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // Get -- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(m => m.Id == id).SingleOrDefaultAsync();

            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // Post -- Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var coupon = await _db.Coupon.Where(m => m.Id == id).SingleOrDefaultAsync();

            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}