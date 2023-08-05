using EndProject.Models;
using EndProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Controllers
{
    public class HomeController : Controller
    {
        

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM();
            {

            }
            return View(homeVM);
        }

       

        
        public IActionResult Error()
        {
            return View();
        }
    }
}
