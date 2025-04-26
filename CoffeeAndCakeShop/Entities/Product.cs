using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeAndCakeShop.Entities
{
    public class Product : BaseEntity
    {
        public DateTime CreateOn { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public int SoldItem { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public bool IsActive { get; set; }
    }
}
