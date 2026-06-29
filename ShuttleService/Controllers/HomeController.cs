using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShuttleService.Models;
using Microsoft.AspNetCore.Authorization;
using ShuttleService.Data;


using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAllRole")]
    //[Authorize]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index(string u)
        {

            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end

           
            if(u != null)
            {
                var surveyTrans = _context.SurveyTransactions.FirstOrDefault(e => e.SurveyHash.Equals(u) && e.IsAnswered.Equals(0));
                if (surveyTrans != null)
                {
                    ViewBag.Hash = surveyTrans.SurveyHash;
                    ViewBag.TransId = surveyTrans.TransactionId;
                }
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;

            ViewData["CompanyGroupId"] = currentCompanyGroupId;
            ViewData["CompanyGroupName"] = currentCompanyGroupName;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
