using CoffeeAndCakeShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1, string? fromDate = null, string? toDate = null)
        {
            int pageSize = 1; // Số đơn hàng mỗi trang

            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.ApplicationUser)
                .AsQueryable();

            // Nếu có lọc theo ngày
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var from))
            {
                query = query.Where(o => o.CreateOn >= from);
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var to))
            {
                // Cộng thêm 1 ngày để lấy hết ngày đó
                to = to.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CreateOn <= to);
            }

            var orders = query.OrderByDescending(o => o.CreateOn).ToList();
            int totalItems = orders.Count;
            var items = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null) return NotFound();

            return PartialView("_OrderDetailsPartial", order);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.StatusProcessing = "Đã hoàn thành";
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
