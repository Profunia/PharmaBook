using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmaBook.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private IProduct _iProduct;
        private IPurchasedHistory _iPurchased;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ProductController(IHostingEnvironment hostingEnvironment, IProduct product, IPurchasedHistory ipurchasedHistory)
        {
            _iProduct = product;
            _iPurchased = ipurchasedHistory;
            _hostingEnvironment = hostingEnvironment;
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
        public IActionResult Create([FromBody]ProductViewModel obj)
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

                    _iProduct.Add(objMap);
                    _iProduct.Commit();
                    msg = obj.name + " medicine has been successfully added!!";

                    // update product histry table
                    PurchasedHistory purchasedHistory = new PurchasedHistory();
                    var pId = _iProduct.GetAll(User.Identity.Name).OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    purchasedHistory.ProductID = pId;
                    purchasedHistory.vendorID = obj.vendorID; // Replace with Vendor ID
                    purchasedHistory.MRP = obj.MRP;
                    purchasedHistory.qty = Convert.ToString(obj.openingStock);
                    purchasedHistory.purchasedDated = DateTime.Now;
                    purchasedHistory.cusUserName = User.Identity.Name;
                    purchasedHistory.BatchNo = obj.batchNo;
                    purchasedHistory.ExpDate = obj.expDate.ToString();
                    purchasedHistory.Mfg = obj.companyName;
                    purchasedHistory.Name = obj.name;
                    purchasedHistory.Remark = string.IsNullOrEmpty(obj.Remarks) ? "Newly added" : obj.Remarks;

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
            
            return Ok(msg);
        }
        [HttpGet]
        public IActionResult BulkUpload()
        {
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult BulkUpload(IFormFile file)
        {
            if (file.Length > 0)
            {              
                string fileExtension = Path.GetExtension(file.FileName.Trim('"'));
                if (fileExtension.Equals(".xlsx"))
                {
                    //TODO
                    // File Upload
                    string sWebRootFolder = _hostingEnvironment.WebRootPath;
                    string sFileName = file.FileName;
                    string UnicFileName = Guid.NewGuid() + Path.GetExtension(file.FileName.Trim('"'));
                    using (var fileStream = new FileStream(Path.Combine(sWebRootFolder, UnicFileName), FileMode.Create))
                    {
                        
                        file.CopyToAsync(fileStream);
                    }

                    // Import 
                    FileInfo file1 = new FileInfo(Path.Combine(sWebRootFolder, UnicFileName));
                    using (ExcelPackage package = new ExcelPackage(file1))
                    {
                        List<string> columnName = new List<string>();
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;
                        bool bHeaderRow = true;
                        ProductViewModel productDetails = null;
                        for (int row = 1; row <= rowCount; row++)
                        {
                            productDetails = new ProductViewModel();
                            for (int col = 1; col <= ColCount; col++)
                            {
                               
                                if (bHeaderRow)
                                {
                                    string HeaderTitle = Convert.ToString(worksheet.Cells[row, col].Value);
                                    columnName.Add(HeaderTitle);

                                }
                                else
                                {                                    
                                    string cName = columnName[col - 1];
                                    string rowData = Convert.ToString(worksheet.Cells[row, col].Value);
                                    
                                    // assign excel data to product view model
                                    if(cName.Equals("MedicineName"))
                                    {
                                        productDetails.name = rowData;
                                    }
                                    else if (cName.Equals("BatchNo"))
                                    {
                                        productDetails.batchNo = rowData;
                                    }
                                    else if (cName.Equals("Stock"))
                                    {
                                        if (!string.IsNullOrEmpty(rowData))
                                        {
                                            productDetails.openingStock = Convert.ToInt32(rowData);
                                        }
                                        
                                    }
                                    else if (cName.Equals("Mfg"))
                                    {
                                        productDetails.companyName = rowData;
                                    }
                                    else if (cName.Equals("MRP"))
                                    {
                                        productDetails.MRP = rowData;
                                    }
                                    else if (cName.Equals("ExpDate"))
                                    {
                                        if (!string.IsNullOrEmpty(rowData))
                                        {  
                                            productDetails.expDate = commonServices.ConvertToDate(rowData);                                             
                                        }                                    
                                       
                                    }
                                    else if (cName.Equals("VendorID"))
                                    {
                                        if (!string.IsNullOrEmpty(rowData))
                                            productDetails.vendorID = Convert.ToInt32(rowData);
                                        else
                                            productDetails.vendorID = 0;
                                    }
                                    else if (cName.Equals("Remark"))
                                    {
                                        productDetails.Remarks = rowData;
                                    }
                                  

                                }
                            }

                            if (bHeaderRow == false)
                            {
                                var status = Create(productDetails);
                                
                            }

                            bHeaderRow = false;
                            

                        }

                    }
                }
                else
                {
                    string msg = "invalid file format";
                }
            }
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
