using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public int Discount { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        public List<SelectListItem> Categories { get; set; }
    }
}
