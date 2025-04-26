using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeAndCakeShop.Entities
{
    public class ProductImage : BaseEntity
    {
        public string Image { get; set; }
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
