using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Services;
using AutoMapper;
using PharmaBook.ViewModel;
using PharmaBook.Entities;

namespace PharmaBook.Controllers
{
    public class PurchasedController : Controller
    {
        private IMasterPOServices _iMasterPo;
        private IchildPoServices _iChildPO;
        public IVendorServices _iVendor;
        private IPurchasedHistory _iPurchased;
        
        private IProduct _iProduct;
        public PurchasedController(IMasterPOServices iMasterPOServices, 
            IVendorServices ivendor,
            IPurchasedHistory iPurchasedHistory,
            IProduct iProduct,
            
            IchildPoServices ichildPoServices)
        {
            _iMasterPo = iMasterPOServices;
            _iChildPO = ichildPoServices;
            _iVendor = ivendor;
            _iProduct = iProduct;
            _iPurchased = iPurchasedHistory;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePurchasedOrder()
        {
            return View();
        }

        public JsonResult CreatePO([FromBody]IEnumerable<PurchasedOrderVM> obj)
        {
            try
            {
                MasterPO Mo = new MasterPO();
                Mo.placedOrderDt = DateTime.Now;
                Mo.userName = User.Identity.Name;
                Mo.VendorID = Convert.ToInt32(obj.FirstOrDefault().VendorID);
                Mo.isActive = true;
                _iMasterPo.Add(Mo);
                _iMasterPo.Commit();

                int masterPoID = _iMasterPo.GetAll(User.Identity.Name)
                                 .OrderByDescending(x => x.Id).FirstOrDefault().Id;
                // Child PO Entry
                ChildPO co = null;
                foreach (var item in obj)
                {
                    co = new ChildPO();
                    co.ProdID = Convert.ToInt32(item.ProdID);
                    co.masterPOid = masterPoID;
                    co.Qty = Convert.ToInt32(item.Qty);
                    co.Remarks = item.Remarks;
                    _iChildPO.Add(co);
                    _iChildPO.Commit();
                }
            }
            catch 
            {

                
            }

            return Json("success");
        }

        public IActionResult InboxPO()
        {
            try
            {
                Dictionary<string, object> dList = new Dictionary<string, object>();
                List<MPO> mpoList = new List<MPO>();
                List<CPO> cpoList = new List<CPO>();
                var PurchasedOrder = _iMasterPo.GetAll(User.Identity.Name).OrderByDescending(x => x.Id).ToList();
                foreach (var Item in PurchasedOrder)
                {
                    MPO mpo = new MPO();
                    CPO cpo = null;

                    // filling master Details 
                    var vendorInfo = _iVendor.GetById(Item.VendorID);
                    mpo.vendorID = Item.VendorID;
                    mpo.VendorName = vendorInfo.vendorName;
                    mpo.VendorAddress = vendorInfo.vendorAddress;
                    mpo.VendorContact = vendorInfo.vendorMobile;
                    mpo.VendorCompany = vendorInfo.vendorCompnay;
                    mpo.PlacedOrder = Item.placedOrderDt.ToString("dd/MM/yyyy");
                    var childObj = _iChildPO.GetAll().Where(x => x.masterPOid == Item.Id).ToList();
                    mpo.NoOfItems = childObj.Count();
                    mpo.MasterPOid = Item.Id;
                    
                    mpo.Status = Item.isActive ? "Open" : "Closed";


                    cpoList = new List<CPO>();
                    // child Purchased order filled 
                    foreach (var cItem in childObj)
                    {
                        cpo = new CPO();
                        cpo.ChildPoId = cItem.Id;
                        cpo.ProductID = cItem.ProdID;
                        cpo.masterPOid = cItem.masterPOid;
                        var productInfo = _iProduct.GetById(cItem.ProdID);
                        cpo.ProductName = productInfo.name;
                        cpo.Mfg = productInfo.companyName;
                        cpo.Remarks = cItem.Remarks;
                        cpo.Qty = cItem.Qty;
                        cpo.BatchNo = productInfo.batchNo;
                        cpo.MRP = productInfo.MRP;
                        cpo.ExpDate = productInfo.expDate.ToString("dd/MM/yyyy");
                        cpoList.Add(cpo);

                    }
                    mpo.cpoList = cpoList;
                    mpoList.Add(mpo);
                }
                

                dList.Add("masterPo", mpoList);
               // dList.Add("childPo", cpoList);

                return Ok(dList);
            }
            catch (Exception)
            {

                return BadRequest();
            }
            
        } 

        [HttpPost]
        public IActionResult childPoDelete(int id)
        {
            try
            {
                var childPo = _iChildPO.GetById(id);
                _iChildPO.Delete(childPo);
                _iChildPO.Commit();

                return Ok();
            }
            catch
            {

                return BadRequest();
            }           
        }

        [HttpPost]
        public IActionResult EntryCreatePurchase([FromBody]IEnumerable<CreatePurchased> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    var product = _iProduct.GetById(item.ProductID);
                    product.openingStock += item.Qty;
                    product.batchNo = item.BatchNo;
                    product.MRP = item.MRP;
                    product.expDate = Convert.ToDateTime(item.ExpDate);
                    product.lastUpdated = DateTime.Now.ToString();
                    product.vendorID = item.vendorID;
                    _iProduct.Update(product);
                    _iProduct.Commit();

                    // product feature update
                    var purchasedHis = new PurchasedHistory();
                    purchasedHis.MRP = item.MRP;
                    purchasedHis.ProductID = item.ProductID;
                    purchasedHis.cusUserName = User.Identity.Name;
                    purchasedHis.purchasedDated = DateTime.Now;
                    purchasedHis.qty = Convert.ToString(item.Qty);
                    purchasedHis.vendorID = item.vendorID;
                    purchasedHis.BatchNo = item.BatchNo;
                    purchasedHis.ExpDate = item.ExpDate.ToString();
                    purchasedHis.Mfg = item.Mfg;
                    purchasedHis.Name = item.Name;
                    purchasedHis.Remark = item.Remark;
                    _iPurchased.Add(purchasedHis);
                    _iPurchased.Commit();


                }
                if (obj.FirstOrDefault().MasterPOid != 0)
                {
                    // update purchased order closed 
                    var po = _iMasterPo.GetById(obj.FirstOrDefault().MasterPOid);
                    po.isActive = false;
                    _iMasterPo.Update(po);
                    _iMasterPo.Commit();
                }
            }
            catch 
            {

                return BadRequest();
            }
            
            return Ok();
        }

        public IActionResult CreateDirectPO()
        {
            return View();
        }


    }
}