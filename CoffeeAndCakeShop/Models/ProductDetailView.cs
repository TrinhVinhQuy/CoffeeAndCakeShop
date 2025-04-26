namespace CoffeeAndCakeShop.Models
{
    public class ProductDetailView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public List<string> Images { get; set; }
        public int Quantity { get; set; }
    }
}
