using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace IdentityApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailSender _emailSender;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
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

                //if (!await _userManager.IsEmailConfirmedAsync(user))
                //{
                //    ModelState.AddModelError("", "Please confirm your email.");
                //    return View(model);
                //}

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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

                await _emailSender.SendEmailAsync(user.Email, "Email Confirmation", $"Please click the <a href='https://localhost:7038{url}'>link</a> for verify your email.");
                TempData["message"] = "Please verify your email.";

                return RedirectToAction("Login", "Account");
            }

            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    public async Task<IActionResult> ConfirmEmail(string id, string token)
    {
        if(id == null || token == null)
        {
            TempData["message"] = "Token is not valid.";
            return View();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                TempData["message"] = "Your email address is confirmed.";
                return RedirectToAction("Login", "Account");
            }
        }

        TempData["message"] = "User info could not found.";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["message"] = "Please type your email.";
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if(user == null)
        {
            TempData["message"] = "Email address is not registered.";         
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var url = Url.Action("ResetPassword", "Account", new { user.Id, token });
        await _emailSender.SendEmailAsync(email, "Reset Password", $"Please click the <a href='https://localhost:7038{url}'>link</a> for reset your password.");
        TempData["message"] = "Please check your email for reset password.";
        return View();
    }

    public IActionResult ResetPassword(string id, string token)
    {
        if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            return RedirectToAction("Login");

        var model = new ResetPasswordViewModel { Token = token };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if(!ModelState.IsValid) 
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["message"] = "User not found";
            return RedirectToAction("Login");
        }        

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["message"] = "Your password has been changed successfully.";
            return RedirectToAction("Login");
        }
        else
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
    }
}
