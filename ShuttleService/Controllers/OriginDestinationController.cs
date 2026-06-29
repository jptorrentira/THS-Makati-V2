using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShuttleService.Data;
using System.Linq;
using ShuttleService.Models;
using Microsoft.AspNetCore.Identity;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class OriginDestinationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OriginDestinationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult OriginDestinationMaintenance()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ShowOriginDestinationLists()
        {
            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            //1 is monday
            //7 is sunday 
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var DataList = _context.OriginDestination
                .Where(e => e.CompanyGroupId == currentCompanyGroupId)
                .OrderBy(e => e.OriginDestinationName).OrderByDescending(s => s.Status).
                            Select(u => new
                            {
                                Id = u.Id,
                                Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> ",
                                OriginDestinationName = u.OriginDestinationName,
                                InsertBy = u.InsertBy,
                                InsertDate = u.InsertDate.ToString("yyyy-MM-dd HH:mm"),
                                UpdatedBy = u.UpdatedBy,
                                UpdatedDate = u.UpdatedDate.HasValue ? u.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                                Status = u.Status,
                            });
            // Total record count.
            int totalRecords = DataList.Count();

            // Verification.
            if (!string.IsNullOrEmpty(search))
            {   // Apply search
                DataList = DataList.Where(x => x.OriginDestinationName.ToLower().Contains(search.ToLower()));
            }
            // Sorting.
            //string[] sort = new string[] { "Name", "Name" };
            //var sortfield = sort[int.Parse(order)];
            //DataList = DataList.OrderBy(x=> x.Name);

            // Filter record count.
            int recFilter = DataList.Count();

            // DataList = DataList.OrderBy(o => o.FirstName +" "+ o.LastName);

            // Apply pagination.
            DataList = DataList.Skip(startRec).Take(pageSize);
            DataList = DataList.OrderBy(x => x.OriginDestinationName);
            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = DataList.ToList(),
            };

            return new JsonResult(jsonData);

        }

        [HttpGet]
        public async Task<IActionResult> GetOriginDestination(int id)
        {
            var originDestination = _context.OriginDestination.Where(r => r.Id == id);

            var jsonData = new
            {
                userData = originDestination.ToList().ToArray(),
                Success = 1
            };

            return new JsonResult(jsonData);

        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateOriginDestination(int id, OriginDestination originDestination)
        {
            var returnSuccess = 1;
            var returnMessage = "";

            //End Manage Parameters
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    originDestination.CompanyGroupId = currentCompanyGroupId;
                    if (id == 0)
                    {
                        originDestination.InsertBy = currentUser.DisplayName;
                        originDestination.InsertDate = DateTime.Now;

                    }
                    else
                    {

                        originDestination.UpdatedBy = currentUser.DisplayName;
                        originDestination.UpdatedDate = DateTime.Now;
                    }

                    _context.Update(originDestination);
                    await _context.SaveChangesAsync();


                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    transaction.Rollback();
                    //throw e;
                }

            }

            var jsonData = new
            {
                message = returnMessage,
                success = returnSuccess
            };
            return new JsonResult(jsonData);
        }
    }
}
