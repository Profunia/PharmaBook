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
        private IErrorLogger _iErrorLogger;
        public VendorController(IHostingEnvironment hostingEnvironment,
            IErrorLogger iErrorLogger, IVendorServices iVendorServices)
        {
            _hostingEnvironment = hostingEnvironment;
            _iVendorServices = iVendorServices;
            _iErrorLogger = iErrorLogger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetVendorByID(int id)
        {
            var selectedVendor = await _iVendorServices.GetById(id);
            return Ok(selectedVendor);
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
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(e.Message);
            }
            return Ok(msg);
        }
        public async Task<IActionResult> UpdtVendor([FromBody]VendorDtl obj)
        {
            string msg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var create = await _iVendorServices.GetById(obj.Id);
                    Mapper.Map(obj, create);
                    create.isActive = true;
                    _iVendorServices.Commit();                   
                    msg = "You have Updated Vendor Details successfully !!.";
                }
            }

            catch (Exception e) {
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);
                msg = e.Message;
            }
            return Ok(msg);
        }
        public async Task<IActionResult> GetAllVendor()
        {
            var lst = (object)null;
            try
            {
                var vendorlist = await _iVendorServices.GetAll(User.Identity.Name);
                vendorlist= vendorlist.Where(x=>x.isActive==true).ToList();
                //lst = vendorlist;
                lst = Mapper.Map<IEnumerable<VendorDtl>>(vendorlist);
            }
            catch(Exception e)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);
                string msg=e.Message;
            }
            return Ok(lst);
        }
        public async Task<IActionResult> VendorDlt([FromHeader]int id)
        {
            string msg = string.Empty;
            try
            {
                var vndrdlt =  await _iVendorServices.GetById(id);
                vndrdlt.isActive = false;
                _iVendorServices.Commit();
                msg = "You have successfully Deleted vendor "+vndrdlt.vendorName;
            }
            catch(Exception e)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(e.Message);
            }
            return Ok(msg);
        }
    }
}