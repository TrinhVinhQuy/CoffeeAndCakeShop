using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CoffeeAndCakeShop.Areas.Admin.Models
{
    public class EditProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // Sử dụng CategoryName để show ra dropdown
        public string CategoryName { get; set; }

        // Load danh sách categories
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public List<string> ExistingImages { get; set; } = new List<string>();
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
    }

}
