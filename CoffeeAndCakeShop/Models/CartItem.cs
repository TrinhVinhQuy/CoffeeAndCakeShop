namespace CoffeeAndCakeShop.Models
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
