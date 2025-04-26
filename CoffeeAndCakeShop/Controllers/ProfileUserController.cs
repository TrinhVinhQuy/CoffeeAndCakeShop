using CoffeeAndCakeShop.Entities;
using CoffeeAndCakeShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAndCakeShop.Controllers
{
    [Authorize]
    public class ProfileUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileUserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                Fullname = user.Fullname,
                MobilePhone = user.MobilePhone,
                Province = user.Province,
                District = user.District,
                Town = user.Town,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Fullname = model.Fullname;
            user.MobilePhone = model.MobilePhone;
            user.Province = model.Province;
            user.District = model.District;
            user.Town = model.Town;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Edit));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
