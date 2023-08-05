using EndProject.DAL;
using EndProject.Helpers;
using EndProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CreditsController : Controller
    {


        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CreditsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Credit> credit = _db.Credits.Include(x => x.Customers).ToList();
            return View(credit);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Credit credit)
        {
            bool IsExist = _db.Credits.Any(x => x.Title == credit.Title);
            if (IsExist == true)
            {
                ModelState.AddModelError("Title", "This Credit type is already is exist!");
                return View();
            }
            if (!ModelState.IsValid)
            {
                return View();

            }
            if (credit.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!credit.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (credit.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "admin/images");
            credit.Image = await credit.Photo.SaveImageAsync(path);

            await _db.Credits.AddAsync(credit);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Credit credit = await _db.Credits.FirstOrDefaultAsync(x => x.Id == id);
            if (credit == null)
                return BadRequest();
            if (credit.IsDeactive)
            {
                credit.IsDeactive = false;
            }
            else
            {
                credit.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            Credit dbcredit = await _db.Credits.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcredit == null)
            {
                return View("Error");
            }

            return View(dbcredit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Credit newcredit)
        {
            if (id == null)
                return View("Error");
            Credit dbcredit = await _db.Credits.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcredit == null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (newcredit.Photo != null)
            {
                if (!newcredit.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (newcredit.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "admin/images");
                dbcredit.Image = await newcredit.Photo.SaveImageAsync(path);
            }
            dbcredit.Title = newcredit.Title;
            dbcredit.Subtitle = newcredit.Subtitle;
            dbcredit.Description = newcredit.Description;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
