using System.ComponentModel.DataAnnotations;

namespace CoffeeAndCakeShop.Models
{
    public class CheckoutView
    {
        [Required(ErrorMessage = "Vui lòng nhập Tỉnh/Thành phố")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Xã/Phường")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        public List<CartItem> CartItems { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; } // Số tiền được giảm

    }

}
