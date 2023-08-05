using EndProject.Helpers;
using EndProject.Models;
using EndProject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Controllers
{
    public class AccountController : Controller
    {
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signinManager;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
        }
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return NotFound();
            //}
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);
            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Passowrd is incorect!");
                return View();
            }
            if (appUser.IsDeactive)
            {
                ModelState.AddModelError("", "Your account has been blocked!");
                return View();
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signinManager.PasswordSignInAsync(appUser, loginVM.Password, true,true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account locked out for 1 min");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Passowrd is incorect!");
                return View();
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser newUser = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email,
                UserName=registerVM.Username,
                
            };
           IdentityResult identityResult= await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(newUser,Roles.Member.ToString());
            await _signinManager.SignInAsync(newUser, true);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        public async Task CreateRoles()
        {
            if(!(await _roleManager.RoleExistsAsync(Roles.Admin.ToString())))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin.ToString() });
            }
            else if (!(await _roleManager.RoleExistsAsync(Roles.Member.ToString())))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Member.ToString() });
            }
        }
        
    }
}

