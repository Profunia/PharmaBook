using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;

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
        public IActionResult Create(ProductViewModel obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product objMap = new Product();
                    objMap.batchNo = obj.batchNo;
                    objMap.companyName = obj.companyName;
                    objMap.cusUserName = User.Identity.Name;
                    objMap.expDate = obj.expDate;
                    objMap.lastUpdated = DateTime.Now.ToString();                   
                    objMap.openingStock = obj.openingStock;
                    objMap.name = obj.name;
                    objMap.MRP = obj.MRP;
                    objMap.isActive = true;                    
                    _iProduct.Add(objMap);
                    _iProduct.Commit();
                    TempData["msg"] = obj.name + " medicine has been successfully added!!";

                    // update product histry table
                    PurchasedHistory purchasedHistory = new PurchasedHistory();
                    var pId= _iProduct.GetAll(User.Identity.Name).OrderByDescending(x=>x.Id).FirstOrDefault().Id;                    
                    purchasedHistory.ProductID = pId;
                    purchasedHistory.vendorID = 1; // Replace with Vendor ID
                    purchasedHistory.MRP = obj.MRP;
                    purchasedHistory.qty = Convert.ToString(obj.openingStock);
                    purchasedHistory.purchasedDated = DateTime.Now;
                    purchasedHistory.cusUserName = User.Identity.Name;
                    _iPurchased.Add(purchasedHistory);
                    _iPurchased.Commit();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["err"] = "something went wroung. Please try again";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ep)
            {

               
            }
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult BulkUpload()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
