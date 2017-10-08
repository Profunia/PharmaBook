using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;

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
                    var create = new Vendor();
                    create.vendorName = obj.vedorName;
                    create.vendorAddress = obj.vedorAddress;
                    create.vendorMobile = obj.vedorMobile;
                    create.vendorCompnay = obj.vedorCompnay;
                    create.cusUserName = obj.cusUserName;

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
                    
                    create.vendorName = obj.vedorName;
                    create.vendorAddress = obj.vedorAddress;
                    create.vendorMobile = obj.vedorMobile;
                    create.vendorCompnay = obj.vedorCompnay;
                    create.cusUserName = obj.cusUserName;

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
            IEnumerable<Vendor> vendorlist = new List<Vendor>();
            List<VendorDtl> lst = new List<VendorDtl>();
            vendorlist = _iVendorServices.GetAll();
            foreach(var i in vendorlist)
            {
                VendorDtl obj = new VendorDtl();
                obj.Id = i.Id;
                obj.vedorName = i.vendorName;
                obj.vedorAddress = i.vendorAddress;
                obj.vedorCompnay = i.vendorCompnay;
                obj.vedorMobile = i.vendorMobile;
                obj.cusUserName = i.cusUserName;
                lst.Add(obj);
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