using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Entities;
using PharmaBook.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PharmaBook.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _singInManager;
        private UserManager<User> _userManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;

        }

        [HttpGet]
        public IActionResult changePassword()
        {
            return View();
        }
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> changePassword(changePassword cp)
        //{
        //    //var change=await _singInManager.PasswordSignInAsync
        //    // ApplicationUser user = await _singInManager.FindByIdAsync(usermodel.Id);
        //    //IdentityUser user = _singInManager.FindByNameAsync(obj.UserName).Result;
        //    return View();
        //}

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResults = await _singInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (loginResults.Succeeded)
                {

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Could not Login..");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}