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
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.CreateOn)
                .ToListAsync();
            return View(orders);
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
