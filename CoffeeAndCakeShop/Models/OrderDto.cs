namespace CoffeeAndCakeShop.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime CreateOn { get; set; }
        public double TotalAmount { get; set; }
        public string StatusProcessing { get; set; }
        public List<OrderDetailDto> Items { get; set; }
    }

    public class OrderDetailDto
    {
        public string ProductName { get; set; }
        public int Quanlity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
