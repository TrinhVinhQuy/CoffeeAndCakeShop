using CoffeeAndCakeShop.Areas.Admin.Models;
using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products
    .Include(pi => pi.ProductImages)
    .Join(
        _context.Categories,
        product => product.CategoryId,
        category => category.Id,
        (product, category) => new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryName = category.Name,
            IsActive = product.IsActive,
            ImageUrl = product.ProductImages.FirstOrDefault() != null
                        ? product.ProductImages.First().Image
                        : null

        })
    .ToList();



            return View(products);
        }

        public IActionResult Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Price = model.Price,
                Discount = model.Discount,
                Quantity = model.Quantity,
                Description = model.Description,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
                CreateOn = DateTime.UtcNow,
                SoldItem = 0
            };

            _context.Products.Add(product);

            // Lưu ảnh vào thư mục wwwroot/images/products
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            Directory.CreateDirectory(uploadPath);

            foreach (var file in model.Images)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn ảnh vào DB
                    var productImage = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Image = "images/products/" + fileName
                    };
                    _context.ProductImages.Add(productImage);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction("Index");
        }

        // GET: Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var productQuery = from p in _context.Products
                               join c in _context.Categories on p.CategoryId equals c.Id
                               where p.Id == id
                               select new
                               {
                                   Product = p,
                                   CategoryName = c.Name
                               };

            var result = await productQuery.FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            var images = await _context.ProductImages
                .Where(img => img.ProductId == result.Product.Id)
                .Select(img => img.Image)
                .ToListAsync();

            var categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Name
                }).ToListAsync();

            var model = new EditProductViewModel
            {
                Id = result.Product.Id,
                Name = result.Product.Name,
                Price = result.Product.Price,
                Description = result.Product.Description,
                CategoryName = result.CategoryName,
                ExistingImages = images,
                Categories = categories
            };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model, List<string> DeleteImages)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Name
                    }).ToListAsync();
                return View(model);
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.CategoryName);

            if (category == null)
            {
                ModelState.AddModelError("CategoryName", "Danh mục không hợp lệ");
                model.Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Name
                    }).ToListAsync();
                return View(model);
            }

            // Cập nhật sản phẩm
            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.CategoryId = category.Id;

            // Xóa hình ảnh nếu có chọn
            if (DeleteImages != null && DeleteImages.Any())
            {
                foreach (var img in DeleteImages)
                {
                    var imageEntity = await _context.ProductImages
                        .FirstOrDefaultAsync(pi => pi.ProductId == product.Id && pi.Image == img);

                    if (imageEntity != null)
                    {
                        _context.ProductImages.Remove(imageEntity);
                    }
                }
            }

            // Upload hình ảnh mới
            if (model.NewImages != null && model.NewImages.Any())
            {
                foreach (var file in model.NewImages)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var productImage = new ProductImage
                    {
                        Image = "images/products/" + fileName,
                        ProductId = product.Id
                    };

                    _context.ProductImages.Add(productImage);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> Lock(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = false; // Khóa sản phẩm
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sản phẩm đã được khoá.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = true; // Mở sản phẩm
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sản phẩm đã được mở.";
            return RedirectToAction(nameof(Index));
        }

    }

}
