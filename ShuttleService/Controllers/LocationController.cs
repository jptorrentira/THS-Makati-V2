using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShuttleService.Data;
using ShuttleService.Models;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult LocationMaintenance()
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
        public async Task<ActionResult> ShowLocationLists()
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

            var DataList = _context.Location.OrderBy(e => e.LocationName).OrderByDescending(s => s.Status).
                            Select(u => new
                            {
                                Id = u.Id,
                                Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> ",
                                LocationName = u.LocationName,
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
                DataList = DataList.Where(x => x.LocationName.ToLower().Contains(search.ToLower()));
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
            DataList = DataList.OrderBy(x => x.LocationName);
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
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = _context.Location.Where(r => r.Id == id).Select(u => new {
                Id = u.Id,
                LocationName = u.LocationName,
                Status = u.Status,
            });

            var jsonData = new
            {
                userData = location.ToList().ToArray(),
                Success = 1
            };

            return new JsonResult(jsonData);

        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateLocation(int id, Location location)
        {
            var returnSuccess = 1;
            var returnMessage = "";

            //End Manage Parameters

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    if (id == 0)
                    {
                        location.InsertBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        location.InsertDate = DateTime.Now;

                    }
                    else
                    {

                        location.UpdatedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        location.UpdatedDate = DateTime.Now;
                    }

                    _context.Update(location);
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
