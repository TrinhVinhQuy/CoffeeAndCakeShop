using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAndCakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var coupons = _db.Coupons.ToList();
            return View(coupons);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                coupon.Id = Guid.NewGuid();
                _db.Coupons.Add(coupon);
                _db.SaveChanges();
                TempData["Success"] = "🎉 Thêm mã giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}
