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
    public class NewsController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public NewsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<News> news = await _db.News.ToListAsync();
            return View(news);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news)
        {

            if (news.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!news.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (news.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "img");
            news.Image = await news.Photo.SaveImageAsync(path);
            await _db.News.AddAsync(news);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            News news = await _db.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news == null)
                return BadRequest();
            if (news.IsDeactive)
            {
                news.IsDeactive = false;
            }
            else
            {
                news.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            News dbnews = await _db.News.FirstOrDefaultAsync(x => x.Id == id);
            if (dbnews == null)
            {
                return View("Error");
            }

            return View(dbnews);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, News freshNews)
        {
            if (id == null)
                return View("Error");
            News dbnews = await _db.News.FirstOrDefaultAsync(x => x.Id == id);
            if (dbnews == null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (dbnews.Photo != null)
            {
                if (!dbnews.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (dbnews.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }

                string path = Path.Combine(_env.WebRootPath, "img");
                dbnews.Image = await freshNews.Photo.SaveImageAsync(path);
            }
            dbnews.Subtitle = freshNews.Subtitle;
            dbnews.Title = freshNews.Title;
            
             await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            News news = _db.News.FirstOrDefault(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            if (news == null)
            {

                return View("Error");
            }
            return View(news);
        }
    }
}
