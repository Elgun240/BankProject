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
    public class WorkersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public WorkersController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            List<Workers> workers = await _db.Workers.Include(x => x.Position).ToListAsync();
            return View(workers);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workers worker ,int posId)
        {
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();

            }
            if (worker.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!worker.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (worker.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "admin/images");
            worker.Images = await worker.Photo.SaveImageAsync(path);
            worker.PositionId = posId;
            await _db.Workers.AddAsync(worker);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Workers worker = await _db.Workers.FirstOrDefaultAsync(x => x.Id == id);
            if (worker == null)
                return BadRequest();
            if (worker.IsDeactive)
            {
                worker.IsDeactive = false;
            }
            else
            {
                worker.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            Workers worker = await _db.Workers.FirstOrDefaultAsync(x=>x.Id==id);
            if (worker == null)
                return View("Error");
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            return View(worker);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Workers worker,int posId)
        {
            if (id == null)
                return View("Error");
            Workers dbworker = await _db.Workers.FirstOrDefaultAsync(x => x.Id == id);
            if (dbworker == null)
                return View("Error");
            ViewBag.Positions = await _db.Positions.Where(x => x.IsDeactive == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(dbworker);

            }
            if (worker.Photo != null)
            {
                if (!worker.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (worker.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "admin/images");
                dbworker.Images = await worker.Photo.SaveImageAsync(path);

            }
            dbworker.Name = worker.Name;
            dbworker.Surname = worker.Surname;
            dbworker.Age = worker.Age;
            dbworker.PositionId = posId;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

           
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Workers worker = _db.Workers.FirstOrDefault(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            if (worker == null)
            {

                return View("Error");
            }
            return View(worker);
        }

    }
}


