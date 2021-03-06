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
        private IVendorServices _iVendor;
        private IErrorLogger _iErrorLogger;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ProductController(IHostingEnvironment hostingEnvironment,
            IVendorServices ivendor,
            IErrorLogger iErrorLogger,
        IProduct product, IPurchasedHistory ipurchasedHistory)
        {
            _iProduct = product;
            _iPurchased = ipurchasedHistory;
            _hostingEnvironment = hostingEnvironment;
            _iVendor = ivendor;
            _iErrorLogger = iErrorLogger;
        }
        // GET: /<controller>/
        //public IActionResult Index()
        //{
        //    var model = _iProduct.GetAll(User.Identity.Name);
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ProductViewModel obj)
        {
            String msg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var prodList = await _iProduct.GetAll(User.Identity.Name);
                    Product chk = prodList.Where(x => x.name.ToLower().Trim() == obj.name.ToLower().Trim()
                        && x.companyName == obj.companyName).FirstOrDefault();
                    var stockMRP = commonServices.getStockMRP((int)obj.stef, (int)obj.tabletsCapsule, obj.eachStefPrice);
                    obj.MRP = stockMRP.MRP;
                    obj.openingStock = stockMRP.openingStock;
                    Product objMap = commonServices.MapVMtoProduct(obj);

                    objMap.lastUpdated = DateTime.Now.ToString();
                    objMap.isActive = true;
                    objMap.cusUserName = User.Identity.Name;
                    if (chk != null)
                    {
                        var prd = await _iProduct.GetById(chk.Id);
                        if (prd.isActive == false)
                        {
                            prd.MRP = stockMRP.MRP;
                            prd.openingStock = stockMRP.openingStock;
                            prd.lastUpdated = DateTime.Now.ToString();
                            prd.isActive = true;
                            _iProduct.Commit();
                        }
                        else
                        {
                            return BadRequest(prd.name + " medicine seems to be duplicate");
                        }
                    }
                    if (chk == null)
                    {
                        _iProduct.Add(objMap);
                        _iProduct.Commit();
                    }

                    msg = obj.name + " medicine has been successfully added!!";

                    // update product histry table
                    PurchasedHistory purchasedHistory = new PurchasedHistory();
                    var tempProdList = await _iProduct.GetAll(User.Identity.Name);
                    var pId = tempProdList.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    purchasedHistory.ProductID = pId;
                    purchasedHistory.vendorID = obj.vendorID; // Replace with Vendor ID
                    purchasedHistory.stef = Convert.ToString(obj.stef);
                    purchasedHistory.tabletsCapsule = Convert.ToString(obj.tabletsCapsule);
                    purchasedHistory.eachStefPrice = Convert.ToString(obj.eachStefPrice);

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
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);

                return BadRequest(ep.Message);
            }

            return Ok(msg);
        }
        [HttpGet]
        public IActionResult BulkUpload()
        {
            SalesViewModel obj = new SalesViewModel();
            return View(obj);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            try
            {
                SalesViewModel obj = new SalesViewModel();
                if (file.Length > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName.Trim('"'));
                    if (fileExtension.Equals(".xlsx"))
                    {
                        //TODO
                        // File Upload
                        BulkFileUpload blk = new BulkFileUpload(_hostingEnvironment);
                        string UnicFileName = await blk.fileUpload(file);
                        string sWebRootFolder = _hostingEnvironment.WebRootPath;
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
                            obj.successlst = new List<ProductViewModel>();
                            obj.duplictlst = new List<Duplicatelist>();
                            obj.producterrlst = new List<ProductError>();
                            for (int row = 1; row <= rowCount; row++)
                            {
                                var productlst = await _iProduct.GetAll(User.Identity.Name);
                                string DplictrowData = string.Empty;
                                string rowData = string.Empty;
                                productDetails = new ProductViewModel();
                                ProductError producterr = new ProductError();
                                string medicine = string.Empty;
                                string mfg = string.Empty;
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
                                        rowData = Convert.ToString(worksheet.Cells[row, col].Value);

                                        // assign excel data to product view model
                                        if (cName.Equals("MedicineName"))
                                        {
                                            if (rowData != null && productlst.Any(x => x.name == rowData) != true)
                                            {
                                                productDetails.name = rowData;
                                            }
                                            else
                                            {
                                                if (productlst.Any(x => x.name == rowData) == true)
                                                {
                                                    medicine = rowData;
                                                }
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.name = "Medicine name required";
                                                }
                                            }
                                        }
                                        else if (cName.Equals("BatchNo"))
                                        {
                                            if (rowData != "")
                                            {
                                                productDetails.batchNo = rowData;
                                            }
                                            else
                                            {
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.batchNo = "BatchNo required";
                                                }
                                            }
                                        }
                                        else if (cName.Equals("Mfg"))
                                        {
                                            if (rowData != null && productlst.Any(x => x.companyName == rowData) != true)
                                            {
                                                productDetails.companyName = rowData;
                                            }
                                            else
                                            {
                                                if (productlst.Any(x => x.companyName == rowData) == true)
                                                {
                                                    mfg = rowData;
                                                }
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.companyName = "Mfg required";
                                                }
                                            }
                                        }
                                        else if (cName.Equals("ExpDate"))
                                        {
                                            if (rowData != null)
                                            {
                                                int month = 0;
                                                int days = 0;
                                                string yr = string.Empty;
                                                char[] c = new char[] { '/', '-' };
                                                string[] dt = rowData.Split(c);
                                                if (dt.Length == 3 && dt[0] != "" && dt[1] != "" && dt[2] != "")
                                                {
                                                    int chk = Convert.ToInt32(dt[1]);
                                                    if (chk > 12)
                                                    {
                                                        int chk2 = Convert.ToInt32(dt[0]);
                                                        int cnt = chk2.ToString().Length;
                                                        if (cnt >= 3)
                                                        {
                                                            yr = Convert.ToString(dt[0]);
                                                            days = Convert.ToInt32(dt[1]);
                                                            month = Convert.ToInt32(dt[2]);
                                                        }
                                                        else
                                                        {
                                                            month = Convert.ToInt32(dt[0]);
                                                            days = Convert.ToInt32(dt[1]);
                                                            yr = Convert.ToString(dt[2]);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        month = Convert.ToInt32(dt[1]);
                                                        days = Convert.ToInt32(dt[0]);
                                                        yr = Convert.ToString(dt[2]);
                                                    }
                                                    if (!string.IsNullOrEmpty(rowData))
                                                    {
                                                        int Mnthdays = System.DateTime.DaysInMonth(2001, month);
                                                        if (Mnthdays >= days)
                                                        {
                                                            char[] chr = new char[] { ' ' };
                                                            string[] yr2 = yr.Split(chr);
                                                            string dt2 = yr2[0] + "/" + month + "/" + days;
                                                            DateTime dt1 = Convert.ToDateTime(dt2);
                                                            productDetails.expDate = dt1.ToString("dd/MM/yyyy");
                                                        }
                                                        else
                                                        {
                                                            producterr.expDate = "Day is exceeding the limit";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    producterr.expDate = "Please insert correct Date";
                                                }
                                            }
                                            else
                                            {
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.expDate = "expDate required";
                                                }
                                            }

                                        }
                                        else if (cName.Equals("VendorID"))
                                        {
                                            if (!string.IsNullOrEmpty(rowData))
                                            {
                                                int vID = Convert.ToInt32(rowData);
                                                var vnDer = await _iVendor.GetAll(User.Identity.Name);
                                                bool isVidExits = vnDer.Any(x => x.Id == vID);
                                                productDetails.vendorID = isVidExits ? vID : 0;
                                            }
                                            else
                                                productDetails.vendorID = 0;
                                        }
                                        else if (cName.Equals("Remark"))
                                        {
                                            productDetails.Remarks = rowData;

                                        }
                                        else if (cName.Equals("Stef"))
                                        {
                                            if (!string.IsNullOrEmpty(rowData))
                                            {
                                                productDetails.stef = Convert.ToInt32(rowData);
                                            }
                                            else
                                            {
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.stef = "Stef required";
                                                }
                                            }
                                        }
                                        else if (cName.Equals("TabletsCapsule"))
                                        {
                                            if (!string.IsNullOrEmpty(rowData))
                                            {
                                                productDetails.tabletsCapsule = Convert.ToInt32(rowData);
                                            }
                                            else
                                            {
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.nooftablet = "No Of Tablets required";
                                                }
                                            }

                                        }
                                        else if (cName.Equals("StefPrice"))
                                        {
                                            if (!string.IsNullOrEmpty(rowData))
                                            {
                                                productDetails.eachStefPrice = Convert.ToDouble(rowData);
                                            }
                                            else
                                            {
                                                if (rowData == null || rowData == "")
                                                {
                                                    producterr.eachstefprice = "Price Per Stef required";
                                                }
                                            }

                                        }

                                    }
                                }
                                if (bHeaderRow == false)
                                {
                                    if ((!String.IsNullOrWhiteSpace(productDetails.name)) &&
                                        (!string.IsNullOrWhiteSpace(productDetails.batchNo)) &&
                                        (!string.IsNullOrWhiteSpace(productDetails.companyName)) &&
                                        (productDetails.stef.HasValue == true) &&
                                        (productDetails.eachStefPrice.HasValue == true) && (productDetails.tabletsCapsule.HasValue == true))
                                    {
                                        var status = Create(productDetails);
                                        obj.successlst.Add(productDetails);
                                    }
                                    else if (medicine != "" && mfg == "" && productDetails.expDate != null)
                                    {
                                        if (productDetails.name == null)
                                        {
                                            productDetails.name = medicine;
                                        }
                                        if (productDetails.companyName == null)
                                        {
                                            productDetails.companyName = mfg;
                                        }
                                        obj.successlst.Add(productDetails);
                                        var status = Create(productDetails);
                                    }
                                    else if (medicine == "" && mfg != "" && productDetails.expDate != null)
                                    {
                                        if (productDetails.name == null)
                                        {
                                            productDetails.name = medicine;
                                        }
                                        if (productDetails.companyName == null)
                                        {
                                            productDetails.companyName = mfg;
                                        }
                                        obj.successlst.Add(productDetails);
                                        var status = Create(productDetails);
                                    }
                                    else if (medicine != "" && mfg != "")
                                    {
                                        var tempProd = await _iProduct.GetAll(User.Identity.Name);
                                        var prodct = tempProd.Where(x => x.name.ToLower().Trim().Equals(medicine.ToLower().Trim())
                                        && x.companyName.ToLower().Trim().Equals(mfg.ToLower().Trim())).FirstOrDefault();
                                        if (prodct != null)
                                        {
                                            if (prodct.isActive == false)
                                            {
                                                productDetails.name = medicine;
                                                productDetails.companyName = mfg;
                                                var status = Create(productDetails);
                                                obj.successlst.Add(productDetails);
                                            }
                                            else
                                            {
                                                Duplicatelist dplctobj = new Duplicatelist();
                                                dplctobj.name = medicine;
                                                dplctobj.companyName = mfg;
                                                obj.duplictlst.Add(dplctobj);
                                            }
                                        }
                                        else
                                        {
                                            // Create new medicine 
                                            productDetails.name = medicine;
                                            productDetails.companyName = mfg;
                                            var status = Create(productDetails);
                                            obj.successlst.Add(productDetails);
                                        }
                                    }
                                    else if (producterr.name != null || producterr.batchNo != null || producterr.openingStock == null || producterr.companyName != null || producterr.expDate != null || producterr.stef != null || producterr.nooftablet != null || producterr.eachstefprice != null)
                                    {
                                        obj.producterrlst.Add(producterr);
                                    }

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
                return View(obj);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return View();
            }
            
        }
        public IActionResult PurchasedHistory()
        {
            return View();
        }
        public async Task<IActionResult> GetAllMedicine()
        {
            try
            {
                string username = User.Identity.Name;
                var prodList = await _iProduct.GetAll(username);
                var productlist = prodList.Where(x => x.isActive == true).OrderByDescending(x => x.Id).ToList();
                List<ProductViewModel> lst = (List<ProductViewModel>)commonServices.MapProductListToVM(productlist);
                //lst = Mapper.Map<IEnumerable<ProductViewModel>>(productlist);

                return Ok(lst);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
        }
        public async Task<IActionResult> GetMedicnById([FromHeader] int id)
        {
            try
            {
                var productlist = await _iProduct.GetById(id);
                ProductViewModel vm = commonServices.MapProductToVM(productlist);
                return Ok(vm);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
        }
        public async Task<IActionResult> UpdateMedicn([FromBody]ProductViewModel prdvwmdl)
        {
            string msg = string.Empty;
            try
            {
                var medicn = await _iProduct.GetById(prdvwmdl.Id);
                medicn.batchNo = prdvwmdl.batchNo;
                medicn.companyName = prdvwmdl.companyName;
                medicn.expDate = commonServices.ConvertToDate(prdvwmdl.expDate);
                medicn.name = prdvwmdl.name;
                medicn.batchNo = prdvwmdl.batchNo;
                if (prdvwmdl.stef != null && prdvwmdl.tabletsCapsule != null && prdvwmdl.eachStefPrice != null)
                {
                    var stockOpen = commonServices.getStockMRP((int)prdvwmdl.stef, (int)prdvwmdl.tabletsCapsule, prdvwmdl.eachStefPrice);
                    medicn.MRP = stockOpen.MRP;
                    medicn.tabletsCapsule = prdvwmdl.tabletsCapsule;
                    medicn.eachStefPrice = prdvwmdl.eachStefPrice;
                }
                medicn.lastUpdated = DateTime.Now.ToString();
                medicn.isActive = true;
                _iProduct.Commit();
                msg = "Medicne Updated Successfully";
            }
            catch(Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                msg = ep.Message;
            }
            return Ok(msg);
        }
        public async Task<IActionResult> DeleteMedicine([FromHeader]int id)
        {
            string msg = string.Empty;
            try
            {
                var dltmedicn = await _iProduct.GetById(id);
                dltmedicn.isActive = false;
                _iProduct.Commit();
                msg = "Medicine has been deleted Succesfully .!!";
            }
            catch(Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                msg = ep.Message;
            }
            return Ok(msg);
        }
        public async Task<IActionResult> PurchsdHstryInbx()
        {
            try
            {
                var inbxlst = await _iPurchased.GetAll(User.Identity.Name);
                var vendors = await _iVendor.GetAll(User.Identity.Name);
                var lst = commonServices.MapPurchasedHistoryToVM(inbxlst, vendors);
                //Mapper.Map<IEnumerable<PurchasedHistoryVM>>(inbxlst);
                return Ok(lst);
            }
            catch (Exception e)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(e, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(e.Message);
            }
        }
    }
}
