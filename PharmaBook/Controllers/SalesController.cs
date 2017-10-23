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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaBook.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private Imaster _imaster;
        private IChild _ichild;
        public SalesController(Imaster master, IChild child)
        {
            _imaster = master;
            _ichild = child;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult AddMasterInvc([FromBody]SalesViewModel slsmodel)
        {            
            string msg = string.Empty;
            string username = User.Identity.Name;
            try
            {
                var childinvoice = slsmodel.childinvc;
                var msterinvoice = slsmodel.masterinvc;
                MasterInvoice obj = Mapper.Map<MasterInvoice>(msterinvoice);
                obj.InvCrtdate = DateTime.Now;
                _imaster.Add(obj);
                _imaster.Commit();
                foreach (var i in childinvoice)
                {
                    ChildInvoice chldinvc = Mapper.Map<ChildInvoice>(i);                   
                    var id= _imaster.getlastproduct();
                     chldinvc.MasterInvID = _imaster.getlastproduct().Id; 
                    _ichild.Add(chldinvc);
                    _ichild.Commit();
                }                
            }
            catch
            {

            }
            return Json(msg);
        } 
    }
}
