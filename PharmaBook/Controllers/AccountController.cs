using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Entities;
using PharmaBook.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using PharmaBook.Services;

namespace PharmaBook.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _singInManager;
        private UserManager<User> _userManager;
        private IErrorLogger _iErrorLogger;
        public AccountController(UserManager<User> userManager, IErrorLogger iError, SignInManager<User> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;
            _iErrorLogger = iError;

        }
      
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePwdViewModel usermodel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ChangePasswordAsync(user, usermodel.oldPassword, usermodel.newPassword);
                if (result.Succeeded)
                {
                    TempData["msg"] = "Password has been successfully changed";
                }
                else
                {
                    ModelState.AddModelError("", "Sorry!! not able to update password.");
                }
                
               
            }
            else
            {
                ModelState.AddModelError("", "New & Confirm password not matched.");
            }
            return View();
        }

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
                ModelState.AddModelError("", "Invalid UserName or Password.");
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