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
    public class CompaignsController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CompaignsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Compaign> compaigns =  _db.Compaign.Where(x => x.IsDeactive == false).ToList();
            
            return View(compaigns);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Compaign compaign)
        {

            if (compaign.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!compaign.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (compaign.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "img");
            compaign.Image = await compaign.Photo.SaveImageAsync(path);
            await _db.Compaign.AddAsync(compaign);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Compaign compaign = await _db.Compaign.FirstOrDefaultAsync(x => x.Id == id);
            if (compaign == null)
            {
                return BadRequest();
            }
            return View(compaign);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Compaign compaign = await _db.Compaign.FirstOrDefaultAsync(x => x.Id == id);
            if (compaign == null)
            {
                return BadRequest();
            }
            
            compaign.IsDeactive = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            Compaign dbcompaign = await _db.Compaign.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcompaign == null)
            {
                return View("Error");
            }

            return View(dbcompaign);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Compaign newCompaign)
        {
            if (id == null)
                return View("Error");
            Compaign dbcompaign = await _db.Compaign.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcompaign == null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (dbcompaign.Photo != null)
            {
                if (!dbcompaign.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (dbcompaign.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }

                string path = Path.Combine(_env.WebRootPath, "img");
                dbcompaign.Image = await newCompaign.Photo.SaveImageAsync(path);
            }
            dbcompaign.Subtitle = newCompaign.Subtitle;
            dbcompaign.Title = newCompaign.Title;
            dbcompaign.Description = newCompaign.Description;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Compaign compaign  = _db.Compaign.FirstOrDefault(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            if (compaign == null)
            {

                return View("Error");
            }
            return View(compaign);
        }
    }
}
