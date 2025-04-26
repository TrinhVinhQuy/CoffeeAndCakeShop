using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var categories = _db.Categories
                .Where(x => x.IsActive == true).ToList();
            ViewBag.Categories = categories;

            var products = _db.Products
                .Where(p => p.IsActive)
                .Include(p => p.ProductImages)
                .ToList()
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    Price = (p.Price * (100 - p.Discount) / 100).ToString("#,0") + " VND",
                    p.Description,
                    p.Discount,
                    Image = p.ProductImages.Select(i => i.Image).First() // thêm dấu ? nếu cần
                })
                .ToList();
            ViewBag.Products = products;

            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
