using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using AutoMapper;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class HomeController : Controller
    { 

        private IProduct _iProduct;
        private Imaster _imaster;
        private IChild _ichild;
        private IPurchasedHistory _iPurchasedhistory;
        private IVendorServices _iVendor;
        private IMasterPOServices _iMasterPo;
        private IProfileServices _iProfile;
        public HomeController(IProduct iProduct,
            IProfileServices iProfile,
            Imaster imaster, 
            IChild ichild,
            IMasterPOServices iMasterPo,
            IVendorServices iVendorServices,
            IPurchasedHistory iPurchased)
        {
            _iProduct = iProduct;
            _imaster = imaster;
            _ichild = ichild;
            _iPurchasedhistory = iPurchased;
            _iVendor = iVendorServices;
            _iMasterPo = iMasterPo;
            _iProfile = iProfile;
        }

        public IActionResult Index()
        {
            DateTime dt = DateTime.Now.AddMonths(3);
            var Products = _iProduct.GetAll(User.Identity.Name);
            var ProductExp = Products.Where(x => x.expDate >= dt && x.expDate <= dt).ToList();

            ViewBag.outOfStock = Products.Where(x => x.openingStock <= 10).Count();
            ViewBag.TotalExpMedicine = ProductExp.Count();

            ViewBag.OpenedPO = _iMasterPo.GetAll(User.Identity.Name).Where(x => x.isActive == true).Count();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpPost]
        public IActionResult topSellingMedicine()
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

        [HttpPost]
        public IActionResult topVendorList()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime valdt = DateTime.Now.AddMonths(-3);
                var InvList = (from m in _iPurchasedhistory.GetAll(User.Identity.Name).Where(x => x.purchasedDated > valdt)                               
                               select new { m.vendorID} into x
                               group x by new { x.vendorID } into g
                               select new
                               {
                                   VID = g.Key.vendorID,
                                   Total = g.Count()
                               }).ToList();

                graphModelVM graph = null;
                foreach (var item in InvList.OrderByDescending(x => x.Total).Take(10))
                {
                    try
                    {
                        graph = new graphModelVM();
                        int vendorId = Convert.ToInt32(item.VID);
                        if (vendorId == 0)
                        {
                            graph.Name = "Self";
                            graph.Value = item.Total;
                        }
                        else
                        {
                            var vInfo = _iVendor.GetById(vendorId);
                            graph.Name = vInfo.vendorName + ", " + vInfo.vendorCompnay;
                            graph.Value = item.Total;
                        }
                        gList.Add(graph);
                    }
                    catch 
                    {

                       
                    }

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

        public IActionResult Profile()
        {
            var model = _iProfile.GetByUserName(User.Identity.Name);            
            var modelVM = Mapper.Map<UserProfileVM>(model);
            return View(modelVM);
        }

        [HttpPost]
        public IActionResult Profile(UserProfileVM Obj)
        {
            UserProfile medicn = _iProfile.GetByUserName(User.Identity.Name);
         
            Mapper.Map(Obj,medicn);
            var a = medicn;
           
            _iProfile.Commit();
            TempData["msg"] = "Successfully updated";
            var laste= _iProfile.GetByUserName(User.Identity.Name);
            return RedirectToAction("Profile");
        }
    }
}
