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

namespace EndProject.Controllers
{
   
    [Authorize(Roles = "Admin")]
    public class AboutUsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public AboutUsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<AboutUs> aboutus = await _db.AboutUs.ToListAsync();
            return View(aboutus);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            AboutUs dbaboutus = await _db.AboutUs.FirstOrDefaultAsync(x => x.Id == id);
            if (dbaboutus == null)
            {
                return View("Error");
            }

            return View(dbaboutus);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, AboutUs newaboutUs)
        {
            if (id == null)
                return View("Error");
            AboutUs dbaboutus = await _db.AboutUs.FirstOrDefaultAsync(x => x.Id == id);
            if (dbaboutus == null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (newaboutUs.Photo != null)
            {
                if (!newaboutUs.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (newaboutUs.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }

                string path = Path.Combine(_env.WebRootPath, "img");
                dbaboutus.Image = await newaboutUs.Photo.SaveImageAsync(path);
            }
            dbaboutus.Description = newaboutUs.Description;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            AboutUs aboutUs = _db.AboutUs.FirstOrDefault(x => x.Id == id);
            ViewBag.AboutUs = await _db.AboutUs.ToListAsync();
            if (aboutUs == null)
            {

                return View("Error");
            }
            return View(aboutUs);
        }
    }
}
