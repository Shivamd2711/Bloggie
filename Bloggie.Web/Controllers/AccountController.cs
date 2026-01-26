using Bloggie.Web.Models.EmailModels;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Services;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IMailService = Bloggie.Web.Services.IMailService;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMailService mailService;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IMailService mailService)
        {
            this._userManager = userManager;
            _signInManager = signInManager;
            this._signInManager = signInManager;
            this.mailService = mailService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (result.Succeeded)
                {
                    EmailRequestModel emailModel = new EmailRequestModel();
                    emailModel.Subject = "Account created successfully";
                    emailModel.ToEmail = model.Email;
                    emailModel.Body = "Your Account has been successfully created. Thank you.";
                        
                    await mailService.SendEmailAsync(emailModel);
                    var roleAssignedResult = await _userManager.AddToRoleAsync(identityUser, "User");
                    if (roleAssignedResult.Succeeded)
                    {
                        return RedirectToAction("Register");
                    }
                }
                return View("Register");

            }
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LogInViewModel
            {
                returnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe, // Use RememberMe here
                    false);

                if (signInResult != null && signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.returnUrl))
                    {
                        return Redirect(model.returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
