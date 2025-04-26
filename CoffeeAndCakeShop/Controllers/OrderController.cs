using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreateOn)
                .ToListAsync();

            var orderDtos = orders.Select(order => new OrderDto
            {
                OrderId = order.Id,
                CreateOn = order.CreateOn,
                TotalAmount = order.TotalAmount,
                StatusProcessing = order.StatusProcessing,
                Items = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ProductName = od.Product?.Name,
                    Quanlity = od.Quanlity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            }).ToList();

            return View(orderDtos);
        }
    }
}
