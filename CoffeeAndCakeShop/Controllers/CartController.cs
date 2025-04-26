using CoffeeAndCakeShop.Data;
using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Models;
using CoffeeAndCakeShop.VnPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace CoffeeAndCakeShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult AddToCart(Guid productId, int? quantity = 1)
        {
            var product = _db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
                return NotFound();

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == product.Id);

            int maxQuantity = product.Quantity;
            int addQuantity = quantity ?? 1;

            if (existingItem != null)
            {
                if (existingItem.Quantity + addQuantity <= maxQuantity)
                {
                    existingItem.Quantity += addQuantity;
                }
                else
                {
                    existingItem.Quantity = maxQuantity;
                    TempData["Message"] = $"⚠️ Số lượng {product.Name} trong giỏ đã đạt tối đa ({maxQuantity}).";
                }
            }
            else
            {
                if (addQuantity <= maxQuantity)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price * (100 - product.Discount) / 100,
                        Quantity = addQuantity,
                        Image = product.ProductImages.FirstOrDefault()?.Image
                    });
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price * (100 - product.Discount) / 100,
                        Quantity = maxQuantity,
                        Image = product.ProductImages.FirstOrDefault()?.Image
                    });
                    TempData["Message"] = $"⚠️ Bạn chỉ có thể thêm tối đa {maxQuantity} sản phẩm {product.Name}.";
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // === Helper methods ===
        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(sessionData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(sessionData);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        public IActionResult IncreaseQuantity(Guid productId)
        {
            var product = _db.Products.Where(x => x.Id == productId).First();
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (item.Quantity < product.Quantity)
                {
                    item.Quantity++;
                    TempData["Message"] = "Đã tăng số lượng.";
                }
                else
                {
                    TempData["Message"] = $"⚠️ Bạn chỉ có thể thêm tối đa {product.Quantity} sản phẩm {product.Name}.";
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantity(Guid productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(Guid productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            var cart = GetCart(); // Lấy giỏ hàng từ session
            if (cart == null || !cart.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
            }

            if (string.IsNullOrEmpty(couponCode))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            var subtotal = cart.Sum(item => item.Price * item.Quantity);
            couponCode = couponCode.Trim().ToUpper();

            var coupon = _db.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã ngừng hoạt động." });
            }

            if (DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc chưa bắt đầu." });
            }

            var discountAmount = (double)subtotal * (double)(coupon.DiscountPercent / 100);
            var finalTotal = (double)subtotal - discountAmount;

            return Json(new
            {
                success = true,
                discountAmount = discountAmount,
                finalTotal = finalTotal
            });
        }


        public IActionResult Checkout()
        {
            var cart = GetCart(); // lấy giỏ hàng từ session hoặc nơi lưu trữ
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutView
            {
                CartItems = cart,
            };

            return View(model);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Checkout(CheckoutView model)
        {
            var cart = GetCart(); // Lấy giỏ hàng từ session

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Không có sản phẩm nào trong giỏ hàng.";
                return RedirectToAction("Index", "Cart");
            }

            // Tính tổng tiền ban đầu
            var totalAmount = cart.Sum(c => c.Price * c.Quantity);

            // Nếu có mã giảm giá
            if (!string.IsNullOrEmpty(model.CouponCode))
            {
                var coupon = _db.Coupons.FirstOrDefault(c => c.Code == model.CouponCode && c.IsActive
                                                             && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);

                if (coupon != null)
                {
                    var discount = (totalAmount * coupon.DiscountPercent) / 100;
                    totalAmount -= discount;
                }
            }
            HttpContext.Session.SetString("Coupon", model.CouponCode ?? "không");

            if (model.PaymentMethod == "COD")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CreateOn = DateTime.Now,
                    Province = model.Province,
                    District = model.District,
                    Town = model.Town,
                    Address = model.Address,
                    TotalAmount = (double)totalAmount,
                    StatusProcessing = "Chờ xử lý",
                    UserId = userId,
                    CouponCode = model.CouponCode ?? "không",
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        UnitPrice = (decimal)item.Price,
                        Quanlity = item.Quantity
                    }).ToList()
                };

                _db.Orders.Add(order);
                _db.SaveChanges();
            }
            else if (model.PaymentMethod == "VnPay")
            {
                // Lưu thông tin địa chỉ vào session cho VnPay
                HttpContext.Session.SetString("Province", model.Province);
                HttpContext.Session.SetString("District", model.District);
                HttpContext.Session.SetString("Town", model.Town);
                HttpContext.Session.SetString("Address", model.Address);

                var vnp = new VnPayLibrary();
                var tick = DateTime.Now.Ticks.ToString();

                vnp.AddRequestData("vnp_Version", "2.1.0");
                vnp.AddRequestData("vnp_Command", "pay");
                vnp.AddRequestData("vnp_TmnCode", "IOXXRAP3");
                vnp.AddRequestData("vnp_Amount", ((int)(totalAmount * 100)).ToString()); // tổng tiền sau giảm giá
                vnp.AddRequestData("vnp_CurrCode", "VND");
                vnp.AddRequestData("vnp_TxnRef", tick);
                vnp.AddRequestData("vnp_OrderInfo", $"Đơn hàng {tick}");
                vnp.AddRequestData("vnp_OrderType", "other");
                vnp.AddRequestData("vnp_Locale", "vn");
                vnp.AddRequestData("vnp_ReturnUrl", "https://localhost:7143/Cart/CfVnPay");
                vnp.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
                vnp.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

                string paymentUrl = vnp.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "SXA9V57XQ9YM46CKPLSF3LCN20WIY6DC");

                return Redirect(paymentUrl);
            }

            ClearCart(); // Xóa session giỏ hàng sau khi thanh toán
            TempData["Success"] = "Đơn hàng của bạn đã được đặt thành công!";
            return RedirectToAction("Index", "Order");
        }

        //[HttpPost]
        //[Authorize]
        //public IActionResult Checkout(CheckoutView model)
        //{
        //    var cart = GetCart(); // lấy giỏ hàng từ session

        //    if (cart == null || !cart.Any())
        //    {
        //        TempData["Error"] = "Không có sản phẩm nào trong giỏ hàng.";
        //        return RedirectToAction("Index", "Cart");
        //    }

        //    if (model.PaymentMethod == "COD")
        //    {
        //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        var order = new Order
        //        {
        //            Id = Guid.NewGuid(),
        //            CreateOn = DateTime.Now,
        //            Province = model.Province,
        //            District = model.District,
        //            Town = model.Town,
        //            Address = model.Address,
        //            TotalAmount = (double)cart.Sum(c => c.Price * c.Quantity),
        //            StatusProcessing = "Chờ xử lý",
        //            UserId = userId,
        //            OrderDetails = cart.Select(item => new OrderDetail
        //            {
        //                Id = Guid.NewGuid(),
        //                ProductId = item.ProductId,
        //                UnitPrice = (decimal)item.Price,
        //                Quanlity = item.Quantity
        //            }).ToList()
        //        };

        //        _db.Orders.Add(order);
        //        _db.SaveChanges();


        //    }
        //    if (model.PaymentMethod == "VnPay")
        //    {
        //        HttpContext.Session.SetString("Province", model.Province);
        //        HttpContext.Session.SetString("District", model.District);
        //        HttpContext.Session.SetString("Town", model.Town);
        //        HttpContext.Session.SetString("Address", model.Address);

        //        var vnp = new VnPayLibrary();
        //        var tick = DateTime.Now.Ticks.ToString();

        //        vnp.AddRequestData("vnp_Version", "2.1.0");
        //        vnp.AddRequestData("vnp_Command", "pay");
        //        vnp.AddRequestData("vnp_TmnCode", "IOXXRAP3");
        //        vnp.AddRequestData("vnp_Amount", ((int)((double)cart.Sum(c => c.Price * c.Quantity) * 100)).ToString());
        //        vnp.AddRequestData("vnp_CurrCode", "VND");
        //        vnp.AddRequestData("vnp_TxnRef", tick);
        //        vnp.AddRequestData("vnp_OrderInfo", $"Đơn hàng {tick}");
        //        vnp.AddRequestData("vnp_OrderType", "other");
        //        vnp.AddRequestData("vnp_Locale", "vn");
        //        vnp.AddRequestData("vnp_ReturnUrl", "https://localhost:7143/Cart/CfVnPay");
        //        vnp.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
        //        vnp.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

        //        string paymentUrl = vnp.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "SXA9V57XQ9YM46CKPLSF3LCN20WIY6DC");

        //        return Redirect(paymentUrl);
        //    }
        //    ClearCart(); // xóa session giỏ hàng sau khi thanh toán
        //    TempData["Success"] = "Đơn hàng của bạn đã được đặt thành công!";
        //    return RedirectToAction("Index", "Home");
        //}
        private void ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("Checkout");
        }

        [Authorize]
        public async Task<IActionResult> CfVnPay([FromQuery] ConfirmVnPay cfvnp)
        {
            if (cfvnp.vnp_ResponseCode == "00" && cfvnp.vnp_TransactionStatus == "00")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var cart = GetCart();
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CreateOn = DateTime.Now,
                    Province = HttpContext.Session.GetString("Province"),
                    District = HttpContext.Session.GetString("District"),
                    Town = HttpContext.Session.GetString("Town"),
                    Address = HttpContext.Session.GetString("Address"),
                    TotalAmount = (double)cart.Sum(c => c.Price * c.Quantity),
                    StatusProcessing = "VnPay",
                    UserId = userId,
                    CouponCode = HttpContext.Session.GetString("Coupon"),
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        UnitPrice = (decimal)item.Price,
                        Quanlity = item.Quantity
                    }).ToList()
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                ClearCart(); // xóa session giỏ hàng sau khi thanh toán
                TempData["Success"] = "Đơn hàng của bạn đã được đặt thành công!";
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }
    }
}
