using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int page = 1, int pageSize = 6, string keyword = "", Guid? categoryId = null)
        {
            var query = from p in _db.Products
                        join c in _db.Categories on p.CategoryId equals c.Id
                        where p.IsActive && c.IsActive
                        && (string.IsNullOrEmpty(keyword) || p.Name.Contains(keyword))
                        && (!categoryId.HasValue || p.CategoryId == categoryId)
                        select new
                        {
                            p.Id,
                            p.Name,
                            p.Price,
                            p.Description,
                            p.Discount,
                            p.Quantity,
                            CategoryName = c.Name, // lấy tên từ Category
                            Image = p.ProductImages.Select(i => i.Image).FirstOrDefault()
                        };

            var totalProducts = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Products = products;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = _db.Categories.Where(c => c.IsActive).ToList(); // danh mục còn hoạt động

            return View();
        }
        public IActionResult Detail(Guid id)
        {
            var product = _db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id && p.IsActive);

            if (product == null)
                return NotFound();

            var model = new ProductDetailView
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                Images = product.ProductImages.Select(i => i.Image).ToList(),
                Quantity = product.Quantity,
            };

            return View(model);
        }

    }
}
