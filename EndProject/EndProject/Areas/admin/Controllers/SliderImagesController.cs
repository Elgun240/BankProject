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
    public class SliderImagesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public SliderImagesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            List<SliderImage> sliderImages = await _db.SliderImages.ToListAsync();
            return View(sliderImages);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderImage sliderImage)
        {

            if (sliderImage.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!sliderImage.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (sliderImage.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "img");
            sliderImage.Image = await sliderImage.Photo.SaveImageAsync(path);
            await _db.SliderImages.AddAsync(sliderImage);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            SliderImage sliderImage = await _db.SliderImages.FirstOrDefaultAsync(x => x.Id == id);
            if (sliderImage == null)
                return BadRequest();
            if (sliderImage.IsDeactive)
            {
                sliderImage.IsDeactive = false;
            }
            else
            {
                sliderImage.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
