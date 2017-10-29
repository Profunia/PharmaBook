using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class HomeController : Controller
    { 

        private IProduct _iProduct;
        private Imaster _imaster;
        private IChild _ichild;
        public HomeController(IProduct iProduct, Imaster imaster, IChild ichild)
        {
            _iProduct = iProduct;
            _imaster = imaster;
            _ichild = ichild;            

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpPost]
        public IActionResult topSellingMedicone()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime valdt = DateTime.Now.AddMonths(-3);
                var InvList = (from m in _imaster.GetAll(User.Identity.Name).Where(x => x.InvCrtdate > valdt)
                               join c in _ichild.GetAll() on m.Id equals c.MasterInvID
                               select new { c.PrdId, c.Qty } into x
                               group x by new { x.PrdId } into g
                               select new
                               {
                                   PID = g.Key.PrdId,
                                   Total = g.Sum(i => i.Qty)
                               }).ToList();

                graphModelVM graph = null;
                foreach (var item in InvList.OrderByDescending(x => x.Total).Take(10))
                {
                    graph = new graphModelVM();
                    var prod = _iProduct.GetById(item.PID);
                    graph.Name = prod.name + ", " + prod.companyName;
                    graph.Value = item.Total;
                    gList.Add(graph);

                }
                return Ok(gList);
            }
            catch 
            {

                return BadRequest();
            }


            
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
