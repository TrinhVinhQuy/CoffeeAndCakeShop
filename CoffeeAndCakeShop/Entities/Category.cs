namespace CoffeeAndCakeShop.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public DateTime CreateOn { get; set; }
        public string Description { get; set; }
        public string MetaImage { get; set; }
        public ICollection<Product> Products { get; set; }
        public bool IsActive { get; set; }
    }
}
