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
        private IErrorLogger _iErrorLogger;
        public HomeController(IProduct iProduct,
            IProfileServices iProfile,
            Imaster imaster,
            IChild ichild,
            IErrorLogger iErrorLogger,
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
            _iErrorLogger = iErrorLogger;
        }

        public async Task<IActionResult> Index()
        {
            var userProfile = await _iProfile.GetByUserName(User.Identity.Name);
            if (userProfile.IsActive == false)
            {
                TempData["err"] = "Account has been locked. Please contact to Administrator";
                return RedirectToAction("login", "Account");
            }
            else if (userProfile.AccountExpDt <= DateTime.Now)
            {
                TempData["err"] = "Account has been expired. Please contact to Administrator";
                return RedirectToAction("login", "Account");

            }
            else
            {
                int d = (userProfile.AccountExpDt - DateTime.Now).Days;
                if (d < 45)
                {
                    TempData["accErr"] = "Account has been expire next " + d + " day. Please contact to Administrator";
                }
            }


            DateTime StartDt = DateTime.Now.AddMonths(3);
            DateTime Enddt = DateTime.Now;
            var Products = await _iProduct.GetAll(User.Identity.Name);
            var ProductExp = Products.Where(x => x.expDate <= StartDt).ToList();

            ViewBag.outOfStock = Products.Where(x => x.openingStock <= 5).Count();
            ViewBag.TotalExpMedicine = ProductExp.Count();
            var po = await _iMasterPo.GetAll(User.Identity.Name);
            ViewBag.OpenedPO = po.Where(x => x.isActive == true).Count();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult SellingMedicineReport()
        {
            return View();
        }
        public async Task<IActionResult> getSellingMedicineReport()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime StartDt = DateTime.Now.AddMonths(-3);
                DateTime Enddt = DateTime.Now;
                var masterInv = await _imaster.GetAll(User.Identity.Name);
                masterInv = masterInv.Where(x => x.InvCrtdate.Date >= StartDt.Date
                          && x.InvCrtdate.Date <= Enddt.Date
                          && x.UserName != null).ToList();
                var childInv = await _ichild.GetAll();
                var InvList = (from m in masterInv
                               join c in childInv on m.Id equals c.MasterInvID
                               select new { c.PrdId, c.Qty } into x
                               group x by new { x.PrdId } into g
                               select new
                               {
                                   PID = g.Key.PrdId,
                                   Total = g.Sum(i => i.Qty)
                               }).ToList();
                graphModelVM graph = null;
                foreach (var item in InvList.OrderByDescending(x => x.Total))
                {
                    try
                    {
                        graph = new graphModelVM();
                        var prod = await _iProduct.GetById(item.PID);
                        graph.Name = prod.name + ", " + prod.companyName;
                        graph.Value = item.Total;
                        gList.Add(graph);
                    }
                    catch
                    {

                    }

                }
                return Ok(gList);
            }
            catch (Exception ep)
            {
                return BadRequest(ep.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> topSellingMedicine()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime StartDt = DateTime.Now.AddMonths(-3);
                DateTime Enddt = DateTime.Now;
                var masterInv = await _imaster.GetAll(User.Identity.Name);
                masterInv = masterInv.Where(x => x.InvCrtdate.Date >= StartDt.Date
                                && x.InvCrtdate.Date <= Enddt.Date
                                && x.UserName != null).ToList();

                var childInv = await _ichild.GetAll();
                var InvList = (from m in masterInv
                               join c in childInv on m.Id equals c.MasterInvID
                               select new { c.PrdId, c.Qty } into x
                               group x by new { x.PrdId } into g
                               select new
                               {
                                   PID = g.Key.PrdId,
                                   Total = g.Sum(i => i.Qty)
                               }).ToList();

                graphModelVM graph = null;
                foreach (var item in InvList.OrderByDescending(x => x.Total).Take(7))
                {
                    try
                    {
                        graph = new graphModelVM();
                        var prod = await _iProduct.GetById(item.PID);
                        graph.Name = prod.name + ", " + prod.companyName;
                        graph.Value = item.Total;
                        gList.Add(graph);
                    }
                    catch
                    {

                    }

                }
                return Ok(gList);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }



        }

        [HttpPost]
        public async Task<IActionResult> topVendorList()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime StartDt = DateTime.Now.AddMonths(-3);
                DateTime Enddt = DateTime.Now;
                var purchasedHis = await _iPurchasedhistory.GetAll(User.Identity.Name);
                purchasedHis = purchasedHis.Where(x => x.purchasedDated >= StartDt && x.purchasedDated <= Enddt
                               && x.cusUserName != null).ToList();

                var InvList = (from m in purchasedHis
                               select new { m.vendorID } into x
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
                        bool isAddToGraph = true;
                        graph = new graphModelVM();
                        int vendorId = Convert.ToInt32(item.VID);
                        if (vendorId == 0)
                        {
                            var b = gList.Where(x => x.Name.Equals("Self"));
                            if (b.Count() == 0)
                            {
                                graph.Name = "Self";
                                graph.Value = item.Total;
                            }
                            else
                            {
                                b.FirstOrDefault().Value += item.Total;
                                isAddToGraph = false;
                            }
                        }
                        else
                        {
                            var vInfo = await _iVendor.GetById(vendorId);
                            graph.Name = vInfo.vendorName + ", " + vInfo.vendorCompnay;
                            graph.Value = item.Total;
                        }
                        if (isAddToGraph)
                        {
                            gList.Add(graph);
                        }
                    }
                    catch
                    {

                    }


                }

                return Ok(gList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> DailySallingReport()
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime StartDt = DateTime.Now.AddDays(-10);
                DateTime Enddt = DateTime.Now;
                var InvList = await _imaster.GetAll(User.Identity.Name);
                InvList = InvList.Where(x => x.InvCrtdate >= StartDt && x.InvCrtdate <= Enddt
                       && x.UserName != null).ToList();

                var childInv = await _ichild.GetAll();
                var DailyResult = (from m in InvList
                                   join c in childInv on m.Id equals c.MasterInvID
                                   select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                   group x by new { date = x.InvCrtdate.Date } into g
                                   select new
                                   {
                                       inv_date = g.Key.date,
                                       totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                       amount = g.Sum(x => x.Amount),
                                       discount = g.Sum(x => x.Discount)
                                   }).ToList();


                var DailyDiscount = (from m in InvList
                                     select new { m.Discount, m.InvCrtdate } into x
                                     group x by new { date = x.InvCrtdate.Date } into g
                                     select new
                                     {
                                         inv_date = g.Key.date,
                                         discount = g.Sum(x => x.Discount)
                                     }).ToList();


                graphModelVM graph = null;
                for (int i = 0; i < 7; i++)
                {
                    try
                    {
                        graph = new graphModelVM();
                        DateTime dt = DateTime.Now.AddDays(-i);
                        graph.Name = dt.ToString("dd/MM/yyyy");

                        var Inv = DailyResult.Where(x => x.inv_date.Date == dt.Date).FirstOrDefault();
                        if (Inv != null)
                        {
                            double discount = 0.0;
                            var disCnt = DailyDiscount.Where(x => x.inv_date.Date == dt.Date).FirstOrDefault();
                            if (disCnt != null)
                            {
                                if (disCnt.discount != 0)
                                {
                                    discount = (double)disCnt.discount;
                                }
                            }
                            double temp1 = Convert.ToDouble(Inv.amount - discount);
                            string temp2 = String.Format("{0:0.00}", temp1);

                            graph.Value = Convert.ToDouble(temp2);

                        }
                        else
                        {
                            graph.Value = 0;
                        }
                        gList.Add(graph);

                    }
                    catch
                    {

                    }


                }

                return Ok(gList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

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
        public JsonResult CurUser()
        {
            var model = _iProfile.GetByUserName(User.Identity.Name);
            var modelVM = Mapper.Map<UserProfileVM>(model);
            return Json(modelVM);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileVM Obj)
        {
            UserProfile medicn = await _iProfile.GetByUserName(User.Identity.Name);

            Mapper.Map(Obj, medicn);
            var a = medicn;
            medicn.IsActive = true;
            _iProfile.Commit();
            TempData["msg"] = "Successfully updated";
            var laste = _iProfile.GetByUserName(User.Identity.Name);
            return RedirectToAction("Profile");
        }

        public IActionResult OutOfStockMedicine()
        {
            return View();
        }

        public async Task<IActionResult> getOutOfStockMedicine()
        {
            try
            {
                var Products = await _iProduct.GetAll(User.Identity.Name);
                var ProductExp = Products.Where(x => x.openingStock <= 5).ToList();
                return Ok(ProductExp.OrderBy(x => x.openingStock));

            }
            catch (Exception ep)
            {

                return BadRequest(ep.Message);
            }
        }
        public IActionResult TotalExpMedicine()
        {

            return View();
        }

        public async Task<IActionResult> getTotalExpMedicine()
        {
            try
            {
                DateTime StartDt = DateTime.Now.AddMonths(3);
                DateTime Enddt = DateTime.Now;
                var Products = await _iProduct.GetAll(User.Identity.Name);
                var ProductExp = Products.Where(x => x.expDate <= StartDt).ToList();
                return Ok(ProductExp.OrderByDescending(x => x.openingStock));
            }
            catch (Exception ep)
            {

                return BadRequest(ep.Message);
            }
        }
    }
}
