namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageIndex { get; set; } // Trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
        public int TotalItems { get; set; } // Tổng số item
    }
}
