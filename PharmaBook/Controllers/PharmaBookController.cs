using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Entities;
using PharmaBook.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PharmaBook.Services;
using System;
using AutoMapper;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class PharmaBookController : Controller
    {
        private SignInManager<User> _singInManager;
        private UserManager<User> _userManager;
        private IProfileServices _iProfileServices;
        public PharmaBookController(UserManager<User> userManager,
            IProfileServices iPro,
            SignInManager<User> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;
            _iProfileServices = iPro;            

        }
        public IActionResult Admin()
        {
            if (!User.Identity.Name.Equals("admin@admin.com"))
            {
                return RedirectToAction("login", "account");
            }

            var users = _iProfileServices.GetAllforAdmin();
            return View(users);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!User.Identity.Name.Equals("admin@admin.com"))
            {
                return RedirectToAction("login", "account");
            }
            return View();
        }

        public IActionResult EditUser(int id)
        {
            if (!User.Identity.Name.Equals("admin@admin.com"))
            {
                return RedirectToAction("login", "account");
            }

            var model = _iProfileServices.GetById(id);
            var VM = Mapper.Map<UserProfileVM>(model);
            return View(VM);
        }
        [HttpPost]
        public IActionResult EditUser(UserProfileVM model, int id)
        {
            if (!User.Identity.Name.Equals("admin@admin.com"))
            {
                return RedirectToAction("login", "account");
            }

            UserProfile medicn = _iProfileServices.GetById(id);
             Mapper.Map(model, medicn);

            //medicn.AccountExpDt = model.AccountExpDt;
            //medicn.IsActive = model.IsActive;
          //  _iProfileServices.Update(medicn);
            _iProfileServices.Commit();
            TempData["msg"]="Successfully updated";
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserProfileVM model)
        {
            if (!User.Identity.Name.Equals("admin@admin.com"))
            {
                return RedirectToAction("login", "account");
            }

            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.userName };

                var createResult = await _userManager.CreateAsync(user, model.Pwd);
                if (createResult.Succeeded)
                {
                    await _singInManager.SignInAsync(user, false);
                    model.CreatedDt = DateTime.Now;
                    model.userName = model.userName;
                    model.IsActive = true;
                    model.lastLogin = DateTime.Now;
                    UserProfile objMap = Mapper.Map<UserProfile>(model);
                    _iProfileServices.Add(objMap);
                    _iProfileServices.Commit();
                    TempData["msg"] = "Successfully Created";


                    return RedirectToAction("Admin");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Please provide the valid information");
            }

            return View();
        }
    }
}