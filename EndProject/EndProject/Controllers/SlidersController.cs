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
    public class SlidersController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public SlidersController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _db.Sliders.ToListAsync();
            return View(sliders);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            Slider dbslider = await _db.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbslider == null)
            {
                return View("Error");
            }

            return View(dbslider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slider newslider)
        {
            if (id == null)
                return View("Error");
            Slider dbslider = await _db.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbslider == null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (newslider.Photo != null)
            {
                if (!newslider.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (newslider.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "img");
                dbslider.Image = await newslider.Photo.SaveImageAsync(path);
            }
            dbslider.Title = newslider.Title;
            dbslider.Description = newslider.Description;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
