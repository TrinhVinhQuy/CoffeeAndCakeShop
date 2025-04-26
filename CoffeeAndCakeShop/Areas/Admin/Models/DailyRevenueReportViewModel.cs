namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    public class DailyRevenueReportViewModel
    {
        public DateTime OrderDate { get; set; }
        public double TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }

}
