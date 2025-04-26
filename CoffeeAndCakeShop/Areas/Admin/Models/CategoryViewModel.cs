using System.ComponentModel.DataAnnotations;

namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    //public class CategoryViewModel
    //{
    //    public Guid Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string MetaImage { get; set; }
    //    public bool IsActive { get; set; }
    //    public DateTime CreateOn { get; set; }
    //}
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string MetaImage { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateOn { get; set; }
        public IFormFile? ImageFile { get; set; } // Thêm dòng này
    }
}


