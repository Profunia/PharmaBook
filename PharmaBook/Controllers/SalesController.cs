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
        [HttpGet]
        public IActionResult InvInbox()
        {
            return View();
        }



        public SalesController(Imaster master, 
            IChild child,
            IPurchasedHistory iPurchasedHistory,
            IProduct product)
        {
            _imaster = master;
            _ichild = child;
            _iProduct = product;
            _iPurchasedHistory = iPurchasedHistory;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Invoice()
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            MasterInvoice mstrobj = _imaster.getlastproduct();
            var chldinvoice = _ichild.GetById(mstrobj.Id);
            var lst = Mapper.Map<IEnumerable<InvcChildVmdl>>(chldinvoice);
            slsvwmdl.invcchld = lst;
            slsvwmdl.masterinvc = Mapper.Map<InvcMstrVmdl>(mstrobj);
            return View(slsvwmdl);
        }
        public JsonResult AddMasterInvc([FromBody]SalesViewModel slsmodel)
        {
            string msg = string.Empty;
            try
            {
                var childinvoice = slsmodel.childinvc;
                slsmodel.masterinvc.UserName = User.Identity.Name;
                var msterinvoice = slsmodel.masterinvc;
                MasterInvoice obj = Mapper.Map<MasterInvoice>(msterinvoice);
                obj.InvCrtdate = DateTime.Now;
                obj.InvId = commonServices.getDynamicId();
                _imaster.Add(obj);
                _imaster.Commit();
                MasterInvoice mstrobj = _imaster.getlastproduct();
                foreach (var i in childinvoice)
                {
                    ChildInvoice chldinvc = Mapper.Map<ChildInvoice>(i);
                    chldinvc.MasterInvID = mstrobj.Id;
                    _ichild.Add(chldinvc);
                    _ichild.Commit();
                }
            }
            catch
            {

            }
            return Json(msg);
        }
        public JsonResult GetInvoice([FromHeader] int id)
        {
            SalesViewModel slsvwmdl = new SalesViewModel();
            MasterInvoice mstrobj = _imaster.getlastproduct();
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
                    var product = _iProduct.GetById(item.prodID);
                    product.openingStock = product.openingStock + item.qty;
                    _iProduct.Commit();

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

    }
}
