namespace CoffeeAndCakeShop.Models
{
    public class ProductView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public int SoldItem { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
