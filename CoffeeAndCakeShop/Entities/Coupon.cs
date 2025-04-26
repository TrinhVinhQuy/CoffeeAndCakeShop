namespace CoffeeAndCakeShop.Entities
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Code { get; set; } // Ví dụ: "SUMMER20"
        public decimal DiscountPercent { get; set; } // Ví dụ: 20% giảm
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

}
