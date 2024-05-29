using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ILogger<AdminTagsController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;


        public AdminUsersController(ILogger<AdminTagsController> logger, IUserRepository userRepository,
            UserManager<IdentityUser> userManager)
        {
            this._logger = logger;
            this._userRepository = userRepository;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userRepository.GetAllAsync();

            var userViewModel = new UserViewModel();
            foreach (var u in users)
            {
                userViewModel.Users.Add(new User { Id = Guid.Parse(u.Id), UserName = u.UserName, Email = u.Email });
            }

            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> List(UserViewModel model)
        {
            var identityUser = new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email,
            };
            var identityResult = await _userManager.CreateAsync(identityUser, model.Password);

            if(identityResult != null)
            {
                if(identityResult.Succeeded)
                {
                    var roles = new List<string> { "User" };
                    if (model.AdminRoleCheckbox)
                    {
                        roles.Add("Admin");
                    }
                   identityResult =  await _userManager.AddToRolesAsync(identityUser, roles);
                    if(identityResult!=null && identityResult.Succeeded)
                    {

                        return RedirectToAction("List", "AdminUsers");
                    }
                }
                return null;
            }

            else
            {
                return RedirectToAction("List", "AdminUsers");
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user !=null)
            {
              var result =  await _userManager.DeleteAsync(user);
                if(result != null && result.Succeeded)
                {
                    return RedirectToAction("List", "AdminUsers");
                }
            }
            return View();
        }
    }
}
