namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

}
