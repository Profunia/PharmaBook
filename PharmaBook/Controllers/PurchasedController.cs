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
        public PurchasedController(IMasterPOServices iMasterPOServices, IchildPoServices ichildPoServices)
        {
            _iMasterPo = iMasterPOServices;
            _iChildPO = ichildPoServices;
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

    }
}