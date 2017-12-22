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
using System.Threading.Tasks;

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
        private IErrorLogger _iErrorLogger;
        [HttpGet]
        public IActionResult InvInbox()
        {
            return View();
        }
        public SalesController(Imaster master,
            IChild child,
            IPurchasedHistory iPurchasedHistory,
            IErrorLogger iErrorLogger,
        IProfileServices iProfileServices,
            IProduct product)
        {
            _imaster = master;
            _iErrorLogger = iErrorLogger;
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
        public async Task<IActionResult> Invoice(int? invcid)
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            try
            {
                if (invcid != null)
                {
                    int id = Convert.ToInt32(invcid);
                    var mtrobj1 = await _imaster.GetAll(User.Identity.Name);
                    var mtrobj = mtrobj1.Where(x => x.Id == id).FirstOrDefault();
                    var cldinvoice = await _ichild.GetById(mtrobj.Id);
                    var lstitm = Mapper.Map<IEnumerable<InvcChildVmdl>>(cldinvoice);
                    slsvwmdl.invcchld = lstitm;
                    slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mtrobj);
                }
                else
                {
                    var mInvList = await _imaster.GetAll(User.Identity.Name);
                    MasterInvoice mstrobj = mInvList.OrderByDescending(x => x.Id).FirstOrDefault();
                    var chldinvoice = await _ichild.GetById(mstrobj.Id);
                    var lst = Mapper.Map<IEnumerable<InvcChildVmdl>>(chldinvoice);
                    slsvwmdl.invcchld = lst;
                    slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mstrobj);
                }
                slsvwmdl.userProfile = await _iProfile.GetByUserName(User.Identity.Name);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
            }
            return View(slsvwmdl);
        }
        public async Task<IActionResult> AddMasterInvc([FromBody]SalesClientViewModel slsmodel)
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
                var masterInv = await _imaster.GetAll(User.Identity.Name);
                MasterInvoice mstrobj = masterInv.OrderByDescending(x => x.Id).FirstOrDefault();
                foreach (var i in childinvoice)
                {
                    ChildInvoice chldinvc = Mapper.Map<ChildInvoice>(i);
                    chldinvc.MasterInvID = mstrobj.Id;
                    chldinvc.Mrp = i.unitprice;
                    _ichild.Add(chldinvc);
                    _ichild.Commit();

                    var medicn = await _iProduct.GetById(i.PrdId);
                    int openstk = medicn.openingStock;
                    medicn.lastUpdated = DateTime.Now.ToString();
                    medicn.isActive = true;
                    medicn.openingStock = (openstk - i.Qty);
                    _iProduct.Commit();
                }
            }
            catch (Exception er)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(er, User.Identity.Name);
                _iErrorLogger.Add(El);

                return BadRequest(er.Message);
            }
            return Ok(msg);
        }
        public async Task<IActionResult> GetInvoice([FromHeader] int id)
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            try
            {
                var msterInv = await _imaster.GetAll(User.Identity.Name);
                MasterInvoice mstrobj = msterInv.OrderByDescending(x => x.Id).FirstOrDefault();
                var chldinvoice = await _ichild.GetAll();
                var lst = Mapper.Map<IEnumerable<InvcChildVmdl>>(chldinvoice);
                slsvwmdl.invcchld = lst;
                slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mstrobj);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
            }
            return Ok(slsvwmdl);
        }

        public async Task<IActionResult> GetAllInvoice(string fromDate, string toDate)
        {
            try
            {
                MasterInv svm = null;
                List<MasterInv> sList = new List<MasterInv>();
                ChildInv cInv = null;
                List<ChildInv> cInvList = null;
                DateTime frmDate = commonServices.ConvertToDate(fromDate);
                DateTime tDate = commonServices.ConvertToDate(toDate);
                var InvList = await _imaster.GetAll(User.Identity.Name);
                InvList= InvList.Where(x => x.InvCrtdate.Date >= frmDate.Date && x.InvCrtdate.Date <= tDate.Date && x.UserName != null).ToList();
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
                    var child = await _ichild.GetAll();
                        child= child.Where(x => x.MasterInvID == item.Id).ToList();
                    svm.NoOfMedicine = child.Count();
                    svm.BillingAmount = child.Sum(x => x.Amount).ToString();
                    cInvList = new List<ChildInv>();
                    //double billingAmt = 0.0;
                    foreach (var c in child)
                    {
                        cInv = new ChildInv();
                        cInv.Id = c.Id;
                        cInv.ProdID = c.MasterInvID;
                        cInv.Name = c.Description;
                        cInv.Mfg = c.Mfg;
                        cInv.ExpDate = c.ExpDt;
                        cInv.BatchNo = c.BatchNo;
                        cInv.MRP = commonServices.getDoubleValue(c.Mrp);
                        cInv.Qty = c.Qty;
                        // billingAmt += (cInv.MRP * cInv.Qty);
                        cInvList.Add(cInv);
                    }
                    //  svm.BillingAmount = Convert.ToString(billingAmt);
                    svm.childInv = cInvList;
                    sList.Add(svm);

                }
                return Ok(sList);
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }


        }

        public async Task<IActionResult> ReturnMedicine([FromBody]IEnumerable<ReturnInv> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    // update InvChild
                    var child = await _ichild.GetIdByPK(item.id);
                    child.Qty = child.Qty - item.qty;
                    child.Amount = commonServices.getDoubleValue(item.mrp) * child.Qty;
                    _ichild.Commit();

                    // Update Stock
                    try
                    {
                        var product = await _iProduct.GetById(child.PrdId);
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
                    ph.Remark = string.IsNullOrEmpty(item.remarks) ? "Inv No. " + item.mastInv + " Return Medicine" : item.remarks;

                    _iPurchasedHistory.Add(ph);
                    _iPurchasedHistory.Commit();


                }
                return Ok();
            }
            catch (Exception e)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);

                return BadRequest(e.Message);
            }
        }

        public IActionResult SalesResport()
        {


            return View();
        }

        public async Task<IActionResult> GridRecords()
        {
            try
            {
                Dictionary<string, object> dList = new Dictionary<string, object>();


                var InvList = await _imaster.GetAll(User.Identity.Name);
                   InvList= InvList.Where(x => x.UserName != null).ToList();

                var InvChild = await _ichild.GetAll();
                var DailyResult = (from m in InvList
                                   join c in InvChild on m.Id equals c.MasterInvID
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

                var MonthlyDiscount = (from m in InvList
                                       select new { m.Discount, m.InvCrtdate } into x
                                       group x by new { date = new DateTime(x.InvCrtdate.Year, x.InvCrtdate.Month, 1) } into g
                                       select new
                                       {
                                           inv_date = g.Key.date,
                                           discount = g.Sum(x => x.Discount)

                                       }).ToList();

                var MonthlyResult = (from m in InvList
                                     join c in InvChild on m.Id equals c.MasterInvID
                                     select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                     group x by new { date = new DateTime(x.InvCrtdate.Year, x.InvCrtdate.Month, 1) } into g
                                     select new
                                     {
                                         inv_date = g.Key.date,
                                         totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                         amount = g.Sum(x => x.Amount),
                                         discount = g.Sum(x => x.Discount)

                                     }).ToList();



                var YearlyResult = (from m in InvList
                                    join c in InvChild on m.Id equals c.MasterInvID
                                    select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                    group x by new { date = x.InvCrtdate.Year } into g
                                    select new
                                    {
                                        inv_date = g.Key.date,
                                        totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                        amount = g.Sum(x => x.Amount),
                                        discount = g.Sum(x => x.Discount)
                                    }).ToList();

                var YearlyDisCount = (from m in InvList
                                      select new { m.Discount, m.InvCrtdate } into x
                                      group x by new { date = x.InvCrtdate.Year } into g
                                      select new
                                      {
                                          inv_date = g.Key.date,
                                          discount = g.Sum(x => x.Discount)
                                      }).ToList();

                dList.Add("MonthlyDiscount", MonthlyDiscount);
                dList.Add("MonthlyResult", MonthlyResult);

                dList.Add("DailyResult", DailyResult);
                dList.Add("DailyDiscount", DailyDiscount);

                dList.Add("YearlyResult", YearlyResult);
                dList.Add("YearlyDisCount", YearlyDisCount);
                return Ok(dList);
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);

                return BadRequest(ep.Message);
            }
        }
    }
}
