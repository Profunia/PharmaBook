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
            var users = _iProfileServices.GetAll(User.Identity.Name);
            return View(users);
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }

        public IActionResult EditUser(int id)
        {
            var model = _iProfileServices.GetById(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult EditUser(UserProfile model, int id)
        {
            UserProfile medicn = _iProfileServices.GetById(id);
            // Mapper.Map(model, medicn);

            medicn.AccountExpDt = model.AccountExpDt;
            medicn.IsActive = model.IsActive;
          //  _iProfileServices.Update(medicn);
            _iProfileServices.Commit();
            TempData["msg"]="Successfullt updated";
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserProfile model)
        {
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
                    UserProfile objMap = Mapper.Map<UserProfile>(model);
                    _iProfileServices.Add(objMap);
                    _iProfileServices.Commit();
                    TempData["msg"] = "Successfullt Created";


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