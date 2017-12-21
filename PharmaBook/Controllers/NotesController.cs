using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PharmaBook.Entities;
using PharmaBook.Services;
using PharmaBook.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace PharmaBook.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private INoteServices _iNote;
        private IErrorLogger _iErrorLogger;
        public NotesController(INoteServices note, IErrorLogger iErrorLogger)
        {
            _iNote = note;
            _iErrorLogger = iErrorLogger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            NotesModel bm = new NotesModel();
            bm.notes = await _iNote.GetAll(User.Identity.Name);
            bm.notes = bm.notes.Where(x => x.isActive == true).OrderByDescending(x => x.id).ToList();
           return View(bm);
        }
        public async Task<IActionResult> Closed(int id)
        {
            var m = await _iNote.GetById(id);
            m.isActive = false;
            _iNote.Commit();
            TempData["msg"] = "Successfully Deleted notes";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Index(NotesModel notes)
        {
            Notes n = new Notes();
            var model = notes.note;
            n.createdDate = DateTime.Now.ToString();
            n.isActive = true;
            n.remarks = model.remarks;
            n.userName = User.Identity.Name;
            _iNote.Add(n);
            _iNote.Commit();
            TempData["msg"] = "Successfully created notes";
            return RedirectToAction("Index");
        }
    }
}