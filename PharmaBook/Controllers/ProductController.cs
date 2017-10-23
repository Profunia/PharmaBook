﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using AutoMapper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaBook.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private IProduct _iProduct;
        private IPurchasedHistory _iPurchased;
        public ProductController(IProduct product, IPurchasedHistory ipurchasedHistory)
        {
            _iProduct = product;
            _iPurchased = ipurchasedHistory;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = _iProduct.GetAll(User.Identity.Name);
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create([FromBody]ProductViewModel obj)
        {
            String msg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                   
                    Product objMap = Mapper.Map<Product>(obj);
                    objMap.lastUpdated = DateTime.Now.ToString();
                    objMap.isActive = true;
                    objMap.cusUserName = User.Identity.Name;

                    //objMap.batchNo = obj.batchNo;
                    //objMap.companyName = obj.companyName;                    
                    //objMap.expDate = obj.expDate;                                     
                    //objMap.openingStock = obj.openingStock;
                    //objMap.name = obj.name;
                    //objMap.MRP = obj.MRP;
                                        
                    _iProduct.Add(objMap);
                    _iProduct.Commit();
                    msg = obj.name + " medicine has been successfully added!!";

                    // update product histry table
                    PurchasedHistory purchasedHistory = new PurchasedHistory();
                    var pId= _iProduct.GetAll(User.Identity.Name).OrderByDescending(x=>x.Id).FirstOrDefault().Id;                    
                    purchasedHistory.ProductID = pId;
                    purchasedHistory.vendorID = obj.vendorID; // Replace with Vendor ID
                    purchasedHistory.MRP = obj.MRP;
                    purchasedHistory.qty = Convert.ToString(obj.openingStock);
                    purchasedHistory.purchasedDated = DateTime.Now;
                    purchasedHistory.cusUserName = User.Identity.Name;
                    _iPurchased.Add(purchasedHistory);
                    _iPurchased.Commit();
                    //return RedirectToAction("Index");
                }
                else
                {
                    msg = "something went wroung. Please try again";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ep)
            {

               
            }
            return Json(msg);
        }        
        public IActionResult BulkUpload()
        {
            return View();
        }
        
        public JsonResult GetAllMedicine()
        {
            string username = User.Identity.Name;
            var lst = (object)null;
            var productlist = _iProduct.GetAll(username);
            lst = Mapper.Map<IEnumerable<ProductViewModel>>(productlist);
            //List<ProductViewModel> lst = new List<ProductViewModel>();                     
            return Json(lst);
        }
        public JsonResult GetMedicnById([FromHeader] int id)
        {
            var productlist = _iProduct.GetById(id);           
            return Json(productlist);
        }
        public JsonResult UpdateMedicn([FromBody]ProductViewModel prdvwmdl)
        {
            string msg = string.Empty;
            try
            {
                var medicn = _iProduct.GetById(prdvwmdl.Id);
                Mapper.Map(prdvwmdl, medicn);
                medicn.lastUpdated = DateTime.Now.ToString();
                medicn.isActive = true;                        
                _iProduct.Update(medicn);
                _iProduct.Commit();
                msg = "Medicne Updated Successfulyy";
            }
            catch
            {
                msg = "Something went wrong";
            }
            return Json(msg);
        }
        public JsonResult DeleteMedicine([FromHeader]int id)
        {
            string msg = string.Empty;
            try
            {
                var dltmedicn = _iProduct.GetById(id);
                _iProduct.Delete(dltmedicn);
                _iProduct.Commit();
                msg = "Medicine has been deleted Succesfully .!!";
            }
            catch
            {
                msg = "Something went wrong . !!";
            }
            return Json(msg);
        }
    }
}
