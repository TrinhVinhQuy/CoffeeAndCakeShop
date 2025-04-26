using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAndCakeShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Json("Username or password is invalid");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var remainingLockout = user.LockoutEnd.Value - DateTimeOffset.UtcNow;

                return Json($"Account is locked out. Please try again in {Math.Round(remainingLockout.TotalMinutes)} minutes");
            }

            if (!result.Succeeded)
            {
                return Json("Username or password is invalid");
            }

            if (user.AccessFailedCount > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return RedirectToAction("", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Gán role cho user
                await _userManager.AddToRoleAsync(user, "Buyer");

                // Có thể tự động đăng nhập sau đăng ký nếu muốn
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

    }
}
