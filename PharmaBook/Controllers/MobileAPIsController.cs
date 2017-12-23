using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Services;
using PharmaBook.Entities;
using PharmaBook.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace PharmaBook.Controllers
{
    [Route("api/v1/[controller]/{username}/[action]")]
    public class MobileAPIsController : Controller
    {
        private IProduct _iProduct;
        private Imaster _imaster;
        private IChild _ichild;
        private SignInManager<User> _singInManager;
        private IProfileServices _iProfile;
        private IErrorLogger _iErrorLogger;
        private INoteServices _iNote;
        public MobileAPIsController(IProduct iProduct, IProfileServices iProfile, SignInManager<User> singInManager, Imaster imaster, IChild ichild, INoteServices iNote,
            IErrorLogger iErrorLogger)
        {
            _iProduct = iProduct;
            _singInManager = singInManager;
            _imaster = imaster;
            _ichild = ichild;
            _iProfile = iProfile;
            _iErrorLogger = iErrorLogger;
            _iNote = iNote;
        }
        public async Task<IActionResult> OutofStock(string username)
        {
            try
            {
                var Products = await _iProduct.GetAll(username);
                var ProductExp = Products.Where(x => x.openingStock <= 5).ToList();
                return Ok(ProductExp.OrderBy(x => x.openingStock));
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
        }

        public async Task<IActionResult> ExpiredMedicine(string username)
        {
            try
            {
                DateTime StartDt = DateTime.Now.AddMonths(3);
                DateTime Enddt = DateTime.Now;
                var Products = await _iProduct.GetAll(username);
                var ProductExp = Products.Where(x => x.expDate <= StartDt).ToList();
                return Ok(ProductExp.OrderByDescending(x => x.openingStock));
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);

                return BadRequest(ep.Message);
            }
        }

        public async Task<IActionResult> SalesResports(string username)
        {
            try
            {
                Dictionary<string, object> dList = new Dictionary<string, object>();


                var InvList = await _imaster.GetAll(username);
                InvList = InvList.Where(x => x.UserName != null).ToList();

                var InvChild = await _ichild.GetAll();
                var DailyResult = (from m in InvList
                                   join c in InvChild on m.Id equals c.MasterInvID
                                   select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                   group x by new { date = x.InvCrtdate.Date } into g
                                   select new
                                   {
                                       inv_date = g.Key.date,
                                       totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                       amount = g.Sum(x => x.Amount),
                                       discount = g.Sum(x => x.Discount)
                                   }).ToList();


                var DailyDiscount = (from m in InvList
                                     select new { m.Discount, m.InvCrtdate } into x
                                     group x by new { date = x.InvCrtdate.Date } into g
                                     select new
                                     {
                                         inv_date = g.Key.date,
                                         discount = g.Sum(x => x.Discount)
                                     }).ToList();

                var MonthlyDiscount = (from m in InvList
                                       select new { m.Discount, m.InvCrtdate } into x
                                       group x by new { date = new DateTime(x.InvCrtdate.Year, x.InvCrtdate.Month, 1) } into g
                                       select new
                                       {
                                           inv_date = g.Key.date,
                                           discount = g.Sum(x => x.Discount)

                                       }).ToList();

                var MonthlyResult = (from m in InvList
                                     join c in InvChild on m.Id equals c.MasterInvID
                                     select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                     group x by new { date = new DateTime(x.InvCrtdate.Year, x.InvCrtdate.Month, 1) } into g
                                     select new
                                     {
                                         inv_date = g.Key.date,
                                         totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                         amount = g.Sum(x => x.Amount),
                                         discount = g.Sum(x => x.Discount)

                                     }).ToList();



                var YearlyResult = (from m in InvList
                                    join c in InvChild on m.Id equals c.MasterInvID
                                    select new { m.Discount, c.Amount, m.InvCrtdate, c.MasterInvID } into x
                                    group x by new { date = x.InvCrtdate.Year } into g
                                    select new
                                    {
                                        inv_date = g.Key.date,
                                        totalInv = g.Select(i => i.MasterInvID).Distinct().Count(),
                                        amount = g.Sum(x => x.Amount),
                                        discount = g.Sum(x => x.Discount)
                                    }).ToList();

                var YearlyDisCount = (from m in InvList
                                      select new { m.Discount, m.InvCrtdate } into x
                                      group x by new { date = x.InvCrtdate.Year } into g
                                      select new
                                      {
                                          inv_date = g.Key.date,
                                          discount = g.Sum(x => x.Discount)
                                      }).ToList();

                dList.Add("MonthlyDiscount", MonthlyDiscount);
                dList.Add("MonthlyResult", MonthlyResult);

                dList.Add("DailyResult", DailyResult);
                dList.Add("DailyDiscount", DailyDiscount);

                dList.Add("YearlyResult", YearlyResult);
                dList.Add("YearlyDisCount", YearlyDisCount);
                return Ok(dList);
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);

                return BadRequest(ep.Message);
            }
        }

        
        public async Task<IActionResult> NoteInbox(string username)
        {
            NotesModel bm = new NotesModel();
            try
            {

                bm.notes = await _iNote.GetAll(username);
                bm.notes = bm.notes.Where(x => x.isActive == true).OrderByDescending(x => x.id).ToList();
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);
            }
            return Ok(bm);
        }

        
        public IActionResult EntryNote(string username, string notes)
        {
            try
            {
                Notes n = new Notes();                
                n.createdDate = DateTime.Now.ToString();
                n.isActive = true;
                n.remarks = notes;
                n.userName = username;
                _iNote.Add(n);
                _iNote.Commit();
               return Ok("Successfully created notes");
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
          
        }

        public async Task<IActionResult> getSellingMedicineReport(string username)
        {
            try
            {
                List<graphModelVM> gList = new List<graphModelVM>();
                DateTime StartDt = DateTime.Now.AddMonths(-3);
                DateTime Enddt = DateTime.Now;
                var masterInv = await _imaster.GetAll(username);
                masterInv = masterInv.Where(x => x.InvCrtdate.Date >= StartDt.Date
                          && x.InvCrtdate.Date <= Enddt.Date
                          && x.UserName != null).ToList();
                var childInv = await _ichild.GetAll();
                var InvList = (from m in masterInv
                               join c in childInv on m.Id equals c.MasterInvID
                               select new { c.PrdId, c.Qty } into x
                               group x by new { x.PrdId } into g
                               select new
                               {
                                   PID = g.Key.PrdId,
                                   Total = g.Sum(i => i.Qty)
                               }).ToList();
                graphModelVM graph = null;
                foreach (var item in InvList.OrderByDescending(x => x.Total))
                {
                    try
                    {
                        graph = new graphModelVM();
                        var prod = await _iProduct.GetById(item.PID);
                        graph.Name = prod.name + ", " + prod.companyName;
                        graph.Value = item.Total;
                        gList.Add(graph);
                    }
                    catch
                    {

                    }

                }
                return Ok(gList);
            }
            catch (Exception ep)
            {
                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);

                return BadRequest(ep.Message);
            }

        }

        public async Task<IActionResult> GetAllMedicineStock(string username)
        {
            try
            {
                var prodList = await _iProduct.GetAll(username);
                var productlist = prodList.Where(x => x.isActive == true).OrderByDescending(x => x.Id).ToList();
                List<ProductViewModel> lst = (List<ProductViewModel>)commonServices.MapProductListToVM(productlist);                

                return Ok(lst);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
        }


        public async Task<IActionResult> UserProfile(string username)
        {
            try
            {
                var model = await _iProfile.GetByUserName(username);
                var modelVM = Mapper.Map<UserProfileVM>(model);
                return Ok(modelVM);
            }
            catch (Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, username);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginResults = await _singInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                    if (loginResults.Succeeded)
                    {
                        return Ok(true);
                    }
                   
                }
            }
            catch (System.Exception ep)
            {

                ErrorLogger El = commonServices.ErrorLoggerMapper(ep, User.Identity.Name);
                _iErrorLogger.Add(El);
                return BadRequest(ep.Message);
            }
            return Ok(false);
        }

    }
}