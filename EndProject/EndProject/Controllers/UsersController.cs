using EndProject.Models;
using EndProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        public readonly UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            List<UserVM> userVMs = new List<UserVM>();
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM {

                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Username = user.UserName,
                    Email = user.Email,
                    IsDeactive = user.IsDeactive,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                };
                userVMs.Add(userVM);
            }
            return View(userVMs);
        }
        public async Task<IActionResult> Activity(string id)
        {
            if (id == null)
                return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            if (user.IsDeactive)
            {
                user.IsDeactive = false;
            }
            else
            {
                user.IsDeactive = true;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null)
                return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            List<string> roles = new List<string>();
            roles.Add(Helpers.Roles.Admin.ToString());
            roles.Add(Helpers.Roles.Member.ToString());
            ChangeRoleVM changeRole = new ChangeRoleVM
            {
                Username = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Roles=roles

            };
           
            return View(changeRole);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id,string newrole)
        {
            if (id == null)
                return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            List<string> roles = new List<string>();
            roles.Add(Helpers.Roles.Admin.ToString());
            roles.Add(Helpers.Roles.Member.ToString());
            string previousRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            ChangeRoleVM changeRole = new ChangeRoleVM
            {
                Username = user.UserName,
                Role = previousRole,
                Roles = roles

            };
            IdentityResult addIdentityResult=await _userManager.AddToRoleAsync(user, newrole);
            if (!addIdentityResult.Succeeded)
            {
                ModelState.AddModelError("", "Something went wrong");
                    return View(changeRole);
            }
            IdentityResult removeIdentityResult = await _userManager.RemoveFromRoleAsync(user, previousRole);
            if (!removeIdentityResult.Succeeded)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(changeRole);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return View("Error");
            }
            AppUser user =await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {

                return View("Error");
            }
            return View(user);
        }
    }
}
