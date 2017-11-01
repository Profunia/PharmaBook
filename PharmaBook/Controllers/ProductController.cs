using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using ExcelDataReader;
using System.Text;
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
                    purchasedHistory.BatchNo = obj.batchNo;
                    purchasedHistory.ExpDate = obj.expDate.ToString();
                    purchasedHistory.Mfg = obj.companyName;
                    purchasedHistory.Name = obj.name;
                    purchasedHistory.Remark = "Newly added";

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
        [HttpGet]
        public IActionResult BulkUpload()
        {
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult BulkUpload(IFormFile file)
        {
            string body = null;
            if (file.Length > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName.Trim('"'));
                if (fileExtension.Equals(".xlsx") || fileExtension.Equals(".xls"))
                {
                    //if (file != null && file.Length > 0)
                    //{
                    //    StreamReader reader = new StreamReader(Server.MapPath("~/HtmlUpload/" + filenm));
                    //    body = reader.ReadToEnd();
                    //    if (Request.Files["file"].ContentLength > 0)
                    //    {
                    //        string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                    //        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    //        {
                    //            string filename = Path.GetFileName(Request.Files["file"].FileName);
                    //            string fileLocation = string.Format("{0}{1}", Server.MapPath("~/Content/UploadedFile"), filename);
                    //            if (System.IO.File.Exists(fileLocation))
                    //                System.IO.File.Delete(fileLocation);
                    //            Request.Files["file"].SaveAs(fileLocation);
                    //            string excelConnectionString = string.Empty;
                    //            if (fileExtension == ".xls")
                    //            {
                    //                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    //            }
                    //            else if (fileExtension == ".xlsx")
                    //            {
                    //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //            }
                    //            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    //            excelConnection.Open();
                    //            DataTable dt = new DataTable();
                    //            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    //            if (dt == null)
                    //            {
                    //                return null;
                    //            }
                    //            String[] excelsheets = new String[dt.Rows.Count];
                    //            int t = 0;
                    //            foreach (DataRow row in dt.Rows)
                    //            {
                    //                excelsheets[t] = row["TABLE_NAME"].ToString();
                    //                t++;
                    //            }
                    //            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                    //            DataSet ds = new DataSet();
                    //            string query = string.Format("Select * from [{0}]", excelsheets[0]);
                    //            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    //            {
                    //                dataAdapter.Fill(ds);
                    //            }
                    //            {
                    //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //                {
                    //                    //  mstEmpInformation _information = new mstEmpInformation();                               
                    //                    _information.intEmpId = Convert.ToInt32((ds.Tables[0].Rows[i]["SL_No"])); ;
                    //                    _information.txtEmpFirstName = Convert.ToString(ds.Tables[0].Rows[i]["Employee_Name"]);
                    //                    _information.txtEmailID = _information.txtEmailID = Convert.ToString(ds.Tables[0].Rows[i]["Emailid"]);

                    //                }

                    //            }

                    //        }
                    //    }
                    //    else
                    //    {
                    //        string msg = "invalid file format";
                    //    }
                    //}
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
