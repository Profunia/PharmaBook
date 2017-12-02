using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IVendorServices _iVendorServices;
        public VendorController(IHostingEnvironment hostingEnvironment, IVendorServices iVendorServices)
        {
            _hostingEnvironment = hostingEnvironment;
            _iVendorServices = iVendorServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetVendorByID(int id)
        {
            var selectedVendor = _iVendorServices.GetById(id);
            return Json(selectedVendor);
        }
        public IActionResult VendorCreate([FromBody]VendorDtl obj)
        {
            string msg = string.Empty;
            try
            {
                if(ModelState.IsValid)
                {
                    obj.cusUserName = User.Identity.Name;
                    
                    var create = Mapper.Map<Vendor>(obj);
                    create.isActive = true;
                    _iVendorServices.Add(create);
                    _iVendorServices.Commit();
                    TempData["msg"] = "You have added vendor successfully !!.";
                     msg= "Vendor Added Successfully";
                }
                else
                {
                    return BadRequest("model validation fail");
                }
            }
            catch(Exception e)
            {
               return BadRequest(e.Message);
            }
            return Ok(msg);
        }
        public JsonResult UpdtVendor([FromBody]VendorDtl obj)
        {
            string msg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var create =_iVendorServices.GetById(obj.Id);
                    Mapper.Map(obj, create);
                    _iVendorServices.Commit();                   
                    msg = "You have Updated Vendor Details successfully !!.";
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return Json(msg);
        }
        public JsonResult GetAllVendor()
        {
            var lst = (object)null;
            try
            {
                var vendorlist = _iVendorServices.GetAll(User.Identity.Name).Where(x=>x.isActive==true).ToList();
                //lst = vendorlist;
                lst = Mapper.Map<IEnumerable<VendorDtl>>(vendorlist);
            }
            catch(Exception e)
            {
                string msg=e.Message;
            }
            return Json(lst);
        }
        public IActionResult VendorDlt([FromHeader]int id)
        {
            string msg = string.Empty;
            try
            {
                var vndrdlt = _iVendorServices.GetById(id);
                vndrdlt.isActive = false;
                _iVendorServices.Commit();
                msg = "You have successfully Deleted vendor "+vndrdlt.vendorName;
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(msg);
        }
    }
}