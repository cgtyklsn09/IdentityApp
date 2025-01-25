using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                if(result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    return RedirectToAction("Index", "Home");
                }
                else if(result.IsLockedOut)
                {
                    var lockedOutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockedOutDate - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Your account is locked out. Please try again later after {timeLeft} minutes.");
                }
                else
                    ModelState.AddModelError("", "Password is not correct.");
            }
            else
                ModelState.AddModelError("", "Email or password is not correct.");
        }

        return View(model);
    }
}
