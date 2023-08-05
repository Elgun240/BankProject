using EndProject.Helpers;
using EndProject.Models;
using Microsoft.AspNetCore.Mvc;
using QuickMailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Controllers
{
    public class EmailController : Controller
    {
        public IActionResult Send()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(MailMessage mailMessage)
        {
            
            
                string mgs = "";

            try
            {
                await EmailUtil.SendEmailMessageAsync(mailMessage.Subject, mailMessage.Body, mailMessage.To);
                mgs = "Email has been send";
            }
            catch (Exception e)
            {
                mgs = "Email send failed!";
              
            }
               
                ViewBag.Mgs = mgs;
                return View();
            
          
           
        }
    }
}
