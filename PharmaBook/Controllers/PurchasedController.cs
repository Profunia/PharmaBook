using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Services;
using AutoMapper;
using PharmaBook.ViewModel;
using PharmaBook.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class PurchasedController : Controller
    {
        private IMasterPOServices _iMasterPo;
        private IchildPoServices _iChildPO;
        public IVendorServices _iVendor;
        private IPurchasedHistory _iPurchased;
        private IErrorLogger _iErrorLogger;
        private IProduct _iProduct;
        public PurchasedController(IMasterPOServices iMasterPOServices, 
            IVendorServices ivendor,
            IPurchasedHistory iPurchasedHistory,
            IErrorLogger iErrorLogger,
        IProduct iProduct,
            
            IchildPoServices ichildPoServices)
        {
            _iMasterPo = iMasterPOServices;
            _iChildPO = ichildPoServices;
            _iVendor = ivendor;
            _iProduct = iProduct;
            _iPurchased = iPurchasedHistory;
            _iErrorLogger = iErrorLogger;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePurchasedOrder()
        {
            return View();
        }

        public async Task<IActionResult> CreatePO([FromBody]IEnumerable<PurchasedOrderVM> obj)
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

                var mpo = await _iMasterPo.GetAll(User.Identity.Name);
                int masterPoID = mpo.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                // Child PO Entry
                ChildPO co = null;
                foreach (var item in obj)
                {
                    co = new ChildPO();
                    co.ProdID = Convert.ToInt32(item.ProdID);
                    co.masterPOid = masterPoID;
                    co.stef = Convert.ToInt32(item.stef);
                    co.Remarks = item.Remarks;
                    _iChildPO.Add(co);
                    _iChildPO.Commit();
                }
            }
            catch(Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);

            }

            return Ok("success");
        }

        public async Task<IActionResult> InboxPO()
        {
            try
            {
                Dictionary<string, object> dList = new Dictionary<string, object>();
                List<MPO> mpoList = new List<MPO>();
                List<CPO> cpoList = new List<CPO>();
                var PurchasedOrder = await _iMasterPo.GetAll(User.Identity.Name);
                PurchasedOrder = PurchasedOrder.OrderByDescending(x => x.Id).ToList();
                foreach (var Item in PurchasedOrder)
                {
                    MPO mpo = new MPO();
                    CPO cpo = null;

                    // filling master Details 
                    if (Item.VendorID != 0)
                    {
                        var vendorInfo = await _iVendor.GetById(Item.VendorID);
                        mpo.vendorID = Item.VendorID;
                        mpo.VendorName = vendorInfo.vendorName;
                        mpo.VendorAddress = vendorInfo.vendorAddress;
                        mpo.VendorContact = vendorInfo.vendorMobile;
                        mpo.VendorCompany = vendorInfo.vendorCompnay;
                    }
                    else
                    {
                        mpo.VendorName = "Self";
                    }
                    mpo.PlacedOrder = Item.placedOrderDt.ToString("dd/MM/yyyy");
                    var childObj = await _iChildPO.GetAll();
                    childObj= childObj.Where(x => x.masterPOid == Item.Id).ToList();
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
                        var productInfo = await _iProduct.GetById(cItem.ProdID);
                        cpo.ProductName = productInfo.name;
                        cpo.Mfg = productInfo.companyName;
                        cpo.stef = (int)cItem.stef;
                        cpo.tabletsCapsule =(int) productInfo.tabletsCapsule;
                        cpo.eachStefPrice = (double)productInfo.eachStefPrice;
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
            catch (Exception rp)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(rp, User.Identity.Name);
                _iErrorLogger.Add(El);

                return BadRequest(rp.Message);
            }
            
        } 

        [HttpPost]
        public async Task<IActionResult> childPoDelete(int id)
        {
            try
            {
                var childPo = await _iChildPO.GetById(id);
                _iChildPO.Delete(childPo);
                _iChildPO.Commit();

                return Ok();
            }
            catch(Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest();
            }           
        }

        [HttpPost]
        public async Task<IActionResult> EntryCreatePurchase([FromBody]IEnumerable<CreatePurchased> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    
                    var stockMRP = commonServices.getStockMRP((int)item.stef, (int)item.tabletsCapsule, item.eachStefPrice);
                    var product = await _iProduct.GetById(item.ProductID);
                    product.openingStock += stockMRP.openingStock;
                    product.batchNo = item.BatchNo;
                    product.MRP = stockMRP.MRP;
                    product.stef = item.stef;
                    product.tabletsCapsule = item.tabletsCapsule;
                    product.eachStefPrice = item.eachStefPrice;
                    product.expDate = commonServices.ConvertToDate(item.ExpDate);
                    product.lastUpdated = DateTime.Now.ToString();
                    product.vendorID = item.vendorID;
                   // update product
                    _iProduct.Commit();

                    // product feature update
                    var purchasedHis = new PurchasedHistory();
                    purchasedHis.MRP = item.MRP;
                    purchasedHis.ProductID = item.ProductID;
                    purchasedHis.cusUserName = User.Identity.Name;
                    purchasedHis.purchasedDated = DateTime.Now;
                    //purchasedHis.qty = Convert.ToString(item.Qty);
                    purchasedHis.stef = Convert.ToString(item.stef);
                    purchasedHis.tabletsCapsule = Convert.ToString(item.tabletsCapsule);
                    purchasedHis.eachStefPrice = Convert.ToString(item.eachStefPrice);
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
                    var po =  await _iMasterPo.GetById(obj.FirstOrDefault().MasterPOid);
                    po.isActive = false;                    
                    _iMasterPo.Commit();
                }
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
            
            return Ok();
        }

        public IActionResult CreateDirectPO()
        {
            return View();
        }


    }
}