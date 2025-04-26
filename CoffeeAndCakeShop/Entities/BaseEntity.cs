using System.ComponentModel.DataAnnotations;

namespace CoffeeAndCakeShop.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
