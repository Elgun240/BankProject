using EndProject.DAL;
using EndProject.Helpers;
using EndProject.Models;
using EndProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
;

namespace EndProject.Controllers
{

    
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CustomersController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            List<Customer> customers = await _db.Customers.OrderByDescending(x => x.Id).Include(x => x.Credit).ToListAsync();
            return View(customers);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Credits = await _db.Credits.Where(x => x.IsDeactive == false).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, int creditId)
        {
            ViewBag.Credits = await _db.Credits.Where(x => x.IsDeactive == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();

            }
            if (customer.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please select Photo!");
                return View();

            }

            if (!customer.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (customer.Photo.IsMore4Mb())
            {
                ModelState.AddModelError("Photo", "Image max 4 mb");
                return View();
            }
            string path = Path.Combine(_env.WebRootPath, "admin/images");
            customer.Image = await customer.Photo.SaveImageAsync(path);
            customer.CreditId = creditId;
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
                return NotFound();
            Customer customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
                return BadRequest();
            if (customer.IsDeactive)
            {
                customer.IsDeactive = false;
            }
            else
            {
                customer.IsDeactive = true;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return View("Error");
            Customer customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                return View("Error");
            ViewBag.Credits = await _db.Credits.Where(x => x.IsDeactive == false).ToListAsync();
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Customer customer, int creditId)
        {
            if (id == null)
                return View("Error");
            Customer dbcustomer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (dbcustomer == null)
                return View("Error");
            ViewBag.Credits = await _db.Credits.Where(x => x.IsDeactive == false).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(dbcustomer);

            }
            if (dbcustomer.Photo != null)
            {
                if (!dbcustomer.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View();
                }
                if (customer.Photo.IsMore4Mb())
                {
                    ModelState.AddModelError("Photo", "Image max 4 mb");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "admin/images");
                dbcustomer.Image = await customer.Photo.SaveImageAsync(path);

            }
            dbcustomer.Name = customer.Name;
           dbcustomer.Surname = customer.Surname;
            dbcustomer.Age = customer.Age;
            dbcustomer.Phone = customer.Phone;
            dbcustomer.Email = customer.Email;
            dbcustomer.CreditId = creditId;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");


        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Customer customer = _db.Customers.FirstOrDefault(x => x.Id == id);
            ViewBag.Positions = await _db.Positions.ToListAsync();
            if (customer == null)
            {

                return View("Error");
            }
            return View(customer);
        }
    }
}
