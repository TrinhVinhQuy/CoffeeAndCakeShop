using CoffeeAndCakeShop.Areas.Admin.Models;
using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAndCakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5; // mỗi trang 5 mục
            var categories = _context.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MetaImage = c.MetaImage,
                    IsActive = c.IsActive,
                    CreateOn = c.CreateOn
                })
                .ToList();

            int totalItems = categories.Count;
            var items = categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }


        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = !category.IsActive; // Đảo ngược trạng thái
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = category.IsActive ? "Mở khoá danh mục thành công!" : "Khoá danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            string imagePath = "";
            if (model.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = "uploads/categories/" + fileName;
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                MetaImage = imagePath,
                IsActive = model.IsActive,
                CreateOn = DateTime.Now
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "🎉 Tạo danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(Guid id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                MetaImage = category.MetaImage,
                IsActive = category.IsActive,
                CreateOn = category.CreateOn
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            if (model.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Xóa file cũ nếu có
                if (!string.IsNullOrEmpty(category.MetaImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.MetaImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                category.MetaImage = "uploads/categories/" + fileName;
            }

            // Update các trường khác
            category.Name = model.Name;
            category.Description = model.Description;
            category.IsActive = model.IsActive;
            // Không update CreateOn

            _context.SaveChanges();

            TempData["SuccessMessage"] = "✅ Cập nhật danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

    }
}
