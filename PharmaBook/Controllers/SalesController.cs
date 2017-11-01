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

        [HttpGet]
        public IActionResult InvInbox()
        {
            return View();
        }

        

        public SalesController(Imaster master, IChild child, IProduct product)
        {
            _imaster = master;
            _ichild = child;
            _iProduct = product;
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
                slsmodel.masterinvc.UserName= User.Identity.Name;
                var msterinvoice = slsmodel.masterinvc;
                MasterInvoice obj = Mapper.Map<MasterInvoice>(msterinvoice);
                obj.InvCrtdate = DateTime.Now;
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
                var InvList = _imaster.GetAll(User.Identity.Name).Where(x=>x.UserName!=null).ToList();
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
                    var child= _ichild.GetAll().Where(x => x.MasterInvID == item.Id).ToList();
                    svm.NoOfMedicine = child.Count();
                    svm.BillingAmount = child.Sum(x => x.Amount).ToString();

     
                }
                return Ok(sList);
            }
            catch (Exception ep)
            {
                return BadRequest(ep.Message);
            }

           
        }
    }
}
