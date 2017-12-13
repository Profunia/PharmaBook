using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PharmaBook.Entities;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaBook.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private Imaster _imaster;
        private IChild _ichild;
        private IProduct _iProduct;
        private IPurchasedHistory _iPurchasedHistory;
        private IProfileServices _iProfile;
        [HttpGet]
        public IActionResult InvInbox()
        {
            return View();
        }
        public SalesController(Imaster master, 
            IChild child,
            IPurchasedHistory iPurchasedHistory,
            IProfileServices iProfileServices,
            IProduct product)
        {
            _imaster = master;
            _ichild = child;
            _iProduct = product;
            _iPurchasedHistory = iPurchasedHistory;
            _iProfile = iProfileServices;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Invoice(int? invcid)
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            if ( invcid != null)
            {
                int id = Convert.ToInt32(invcid);
                var mtrobj = _imaster.GetAll(User.Identity.Name).Where(x => x.Id == id).FirstOrDefault();
                var cldinvoice = _ichild.GetById(mtrobj.Id);
                var lstitm = Mapper.Map<IEnumerable<InvcChildVmdl>>(cldinvoice);
                slsvwmdl.invcchld = lstitm;
                slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mtrobj);                
            }
            else
            {
                MasterInvoice mstrobj = _imaster.getlastproduct(User.Identity.Name);
                var chldinvoice = _ichild.GetById(mstrobj.Id);
                var lst = Mapper.Map<IEnumerable<InvcChildVmdl>>(chldinvoice);
                slsvwmdl.invcchld = lst;
                slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mstrobj);                
            }
            slsvwmdl.userProfile = _iProfile.GetByUserName(User.Identity.Name);
            return View(slsvwmdl);
        }
        public IActionResult AddMasterInvc([FromBody]SalesClientViewModel slsmodel)
        {
            string msg = string.Empty;
            try
            {
                var childinvoice = slsmodel.childinvc;
                slsmodel.masterinvc.UserName = User.Identity.Name;
                var msterinvoice = slsmodel.masterinvc;
                MasterInvoice obj = Mapper.Map<MasterInvoice>(msterinvoice);
                obj.InvCrtdate = DateTime.Now;
                obj.isActive = true;
                obj.InvId = commonServices.getDynamicId();
                _imaster.Add(obj);
                _imaster.Commit();
                MasterInvoice mstrobj = _imaster.getlastproduct(User.Identity.Name);
                foreach (var i in childinvoice)
                {
                    ChildInvoice chldinvc = Mapper.Map<ChildInvoice>(i);
                    chldinvc.MasterInvID = mstrobj.Id;
                    _ichild.Add(chldinvc);
                    _ichild.Commit();

                    var medicn = _iProduct.GetById(i.PrdId);
                    int openstk = medicn.openingStock;
                    medicn.lastUpdated = DateTime.Now.ToString();
                    medicn.isActive = true;
                    medicn.openingStock = (openstk - i.Qty);
                    _iProduct.Commit();                    
                }                
            }
            catch(Exception er)
            {
                return BadRequest(er.Message);
            }
            return Ok(msg);
        }
        public JsonResult GetInvoice([FromHeader] int id)
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            MasterInvoice mstrobj = _imaster.getlastproduct(User.Identity.Name);
            var chldinvoice = _ichild.GetAll();
            var lst = Mapper.Map<IEnumerable<InvcChildVmdl>>(chldinvoice);
            slsvwmdl.invcchld = lst;
            slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mstrobj);
            return Json(slsvwmdl);
        }

        public IActionResult GetAllInvoice()
        {
            try
            {
                MasterInv svm = null;
                List<MasterInv> sList = new List<MasterInv>();
                ChildInv cInv = null;
                List<ChildInv> cInvList = null;
                var InvList = _imaster.GetAll(User.Identity.Name).Where(x => x.UserName != null).ToList();
                foreach (var item in InvList)
                {
                    svm = new MasterInv();
                    svm.Id = item.Id;
                    svm.InvId = item.InvId;
                    svm.Patient = item.PatientName;
                    svm.Paddress = item.PatientAdres;
                    svm.DocName = item.DrName;
                    svm.DocRegi = item.RegNo;
                    svm.Discount = item.Discount;
                    svm.CreatedDate = item.InvCrtdate.ToString("dd/MM/yyyy");
                    var child = _ichild.GetAll().Where(x => x.MasterInvID == item.Id).ToList();
                    svm.NoOfMedicine = child.Count();
                    svm.BillingAmount = child.Sum(x => x.Amount).ToString();
                    cInvList = new List<ChildInv>();
                    foreach (var c in child)
                    {
                        cInv = new ChildInv();
                        cInv.Id = c.Id;
                        cInv.ProdID = c.MasterInvID;
                        cInv.Name = c.Description;
                        cInv.Mfg = c.Mfg;
                        cInv.ExpDate = c.ExpDt;
                        cInv.BatchNo = c.BatchNo;
                        cInv.MRP = c.Amount;
                        cInv.Qty = c.Qty;
                        cInvList.Add(cInv);
                    }
                    svm.childInv = cInvList;
                    sList.Add(svm);

                }
                return Ok(sList);
            }
            catch (Exception ep)
            {
                return BadRequest(ep.Message);
            }


        }

        public IActionResult ReturnMedicine([FromBody]IEnumerable<ReturnInv> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    // update InvChild
                    var child = _ichild.GetIdByPK(item.id);
                    child.Qty = child.Qty - item.qty;
                    _ichild.Commit();

                    // Update Stock
                    try
                    {
                        var product = _iProduct.GetById(child.PrdId);
                        product.openingStock = product.openingStock + item.qty;
                        _iProduct.Commit();
                    }
                    catch { }

                    // Update Purchased History
                    PurchasedHistory ph = new PurchasedHistory();
                    ph.MRP = Convert.ToString(child.Amount);
                    ph.ProductID = child.PrdId;
                    ph.cusUserName = User.Identity.Name;
                    ph.purchasedDated = DateTime.Now;
                    ph.qty = Convert.ToString(item.qty);
                    ph.vendorID = 0;
                    ph.BatchNo = child.BatchNo;
                    ph.ExpDate = child.ExpDt;
                    ph.Mfg = child.Mfg;
                    ph.Name = child.Description;
                    ph.Remark = string.IsNullOrEmpty(item.remarks) ?  "Inv No. " + item.mastInv + " Return Medicine" : item.remarks;

                    _iPurchasedHistory.Add(ph);
                    _iPurchasedHistory.Commit();


                }
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        public IActionResult SalesResport()
        {


            return View();
        }

        public IActionResult GridRecords()
        {
            try
            {
                Dictionary<string, object> dList = new Dictionary<string, object>();

                var InvList = _imaster.GetAll(User.Identity.Name).Where(x => x.UserName != null).ToList();
                var MonthlyResult = (from m in InvList
                                     join c in _ichild.GetAll() on m.Id equals c.MasterInvID
                                     select new { c.Amount, m.InvCrtdate, m.Id,c.MasterInvID } into x
                                     group x by new { date = new DateTime(x.InvCrtdate.Year, x.InvCrtdate.Month, 1) } into g
                                     select new
                                     {
                                         inv_date = g.Key.date,
                                         totalInv= g.Select(i => i.MasterInvID).Distinct().Count(),
                                         amount = g.Sum(x => x.Amount)
                                     }).ToList();

                var DailyResult = (from m in InvList
                                   join c in _ichild.GetAll() on m.Id equals c.MasterInvID
                                   select new { c.Amount, m.InvCrtdate,c.MasterInvID } into x
                                   group x by new { date = x.InvCrtdate.Date } into g
                                   select new
                                   {
                                       inv_date = g.Key.date,
                                       totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                       amount = g.Sum(x => x.Amount)
                                   }).ToList();

                var YearlyResult = (from m in InvList
                                    join c in _ichild.GetAll() on m.Id equals c.MasterInvID
                                    select new { c.Amount, m.InvCrtdate,c.MasterInvID } into x
                                    group x by new { date = x.InvCrtdate.Year } into g
                                    select new
                                    {
                                        inv_date = g.Key.date,
                                        totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                        amount = g.Sum(x => x.Amount)
                                    }).ToList();

                dList.Add("MonthlyResult", MonthlyResult);
                dList.Add("DailyResult", DailyResult);
                dList.Add("YearlyResult", YearlyResult);
                return Ok(dList);
            }
            catch (Exception ep)
            {

                return BadRequest(ep.Message);
            }
        }
    }
}
