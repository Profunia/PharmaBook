using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PharmaBook.Controllers
{
    public class PurchasedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePurchasedOrder()
        {
            return View();
        }
    }
}