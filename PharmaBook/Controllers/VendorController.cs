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

namespace PharmaBook.Controllers
{
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
        public JsonResult VendorCreate([FromBody]VendorDtl obj)
        {
            string msg = string.Empty;
            try
            {
                if(ModelState.IsValid)
                {
                    obj.cusUserName = User.Identity.Name;
                    var create = Mapper.Map<Vendor>(obj);
                    //    new Vendor();
                    //create.vendorName = obj.vendorName;
                    //create.vendorAddress = obj.vendorAddress;
                    //create.vendorMobile = obj.vendorMobile;
                    //create.vendorCompnay = obj.vendorCompnay;
                    //create.cusUserName = obj.cusUserName;

                    _iVendorServices.Add(create);
                    _iVendorServices.Commit();
                    TempData["msg"] = "You have added vendor successfully !!.";
                     msg= "Vendor Added Successfully";
                }
            }
            catch(Exception e)
            {
                msg = e.Message;
            }
            return Json(msg);
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
                    //create.vendorName = obj.vendorName;
                    //create.vendorAddress = obj.vendorAddress;
                    //create.vendorMobile = obj.vendorMobile;
                    //create.vendorCompnay = obj.vendorCompnay;
                    //create.cusUserName = obj.cusUserName;

                    _iVendorServices.Update(create);
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
                var vendorlist = _iVendorServices.GetAll();
                //lst = vendorlist;
                lst = Mapper.Map<IEnumerable<VendorDtl>>(vendorlist);
            }
            catch(Exception e)
            {
                string msg=e.Message;
            }
            return Json(lst);
        }
        public JsonResult VendorDlt([FromHeader]int id)
        {
            string msg = string.Empty;
            try
            {
                var vndrdlt = _iVendorServices.GetById(id);
                _iVendorServices.Delete(vndrdlt);
                _iVendorServices.Commit();
                msg = "You have successfulyy Deleted vendor "+vndrdlt.vendorName;
            }
            catch(Exception e)
            {
                msg = e.Message;
            }
            return Json(msg);
        }
    }
}