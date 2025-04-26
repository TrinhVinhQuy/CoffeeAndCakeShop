using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeAndCakeShop.Entities
{
    public class Order : BaseEntity
    {
        public DateTime CreateOn { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public double TotalAmount { get; set; }
        public string StatusProcessing { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string CouponCode { get; set; }
    }
}
