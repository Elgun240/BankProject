using EndProject.DAL;
using EndProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Areas.admin.Controllers
{
    
        [Area("Admin")]
        [Authorize(Roles="Admin")]
        public class PositionsController : Controller
        {


            private readonly AppDbContext _db;
            public PositionsController(AppDbContext db)
            {
                _db = db;
            }
            public IActionResult Index()
            {
                List<Position> position = _db.Positions.Where(x => x.IsDeactive == false).Include(x => x.Workers).ToList();
                return View(position);
            }
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Position position)
            {
                bool IsExist = _db.Positions.Any(x => x.Name == position.Name);
                if (IsExist == true)
                {
                    ModelState.AddModelError("Name", "This Position is already is exist!");
                    return View();
                }
                await _db.Positions.AddAsync(position);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Position position = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            if (position == null)
            {
                return BadRequest();
            }
            return View(position);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Position changedPosition)
        {
            if (changedPosition.Name == null)
            {
                ModelState.AddModelError("Name", "This field can't be empty!");
                return View();
            }
            if (id == null)
            {
                return NotFound();
            }
            Position dbposition = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            if (dbposition == null)
            {
                return BadRequest();
            }
            bool IsExist = _db.Positions.Any(x => x.Name == changedPosition.Name);
            if (IsExist == true)
            {
                ModelState.AddModelError("Name", "This Position is already is exist!");
                return View();
            }
            dbposition.Name = changedPosition.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Position position = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            if (position == null)
            {
                return BadRequest();
            }
            return View(position);
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
            Position position = await _db.Positions.Include(x => x.Workers).FirstOrDefaultAsync(x => x.Id == id);
            if (position == null)
            {
                return BadRequest();
            }
            foreach (Workers workers in position.Workers)
            {
                workers.IsDeactive = true;
            }
            position.IsDeactive = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ReturnAllWorkers()
        {
            List<Position> position = await _db.Positions.Where(x => x.IsDeactive == true).Include(x => x.Workers).ToListAsync();
            foreach (Position pos in position)
            {
                pos.IsDeactive = false;
                foreach (Workers worker in pos.Workers)
                {
                    worker.IsDeactive = false;

                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Position position = _db.Positions.Include(x=>x.Workers).FirstOrDefault(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            if (position == null)
            {

                return View("Error");
            }
            return View(position);
        }
    }
}

