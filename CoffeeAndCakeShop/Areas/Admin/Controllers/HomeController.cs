using CoffeeAndCakeShop.Areas.Admin.Models;
using CoffeeAndCakeShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAndCakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetRevenueData(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Where(o => o.StatusProcessing == "Đã hoàn thành");

            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreateOn.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreateOn.Date <= endDate.Value.Date);
            }

            var data = await query
                .GroupBy(o => o.CreateOn.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var result = data.Select(x => new
            {
                Date = x.Date.ToString("yyyy-MM-dd"),
                TotalRevenue = x.TotalRevenue
            });

            return Json(result);
        }


    }
}
