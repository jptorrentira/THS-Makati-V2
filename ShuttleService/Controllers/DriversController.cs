using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ShuttleService.Controllers
{
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public DriversController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Drivers
        //[Authorize(Policy = "RequireAdminRole")]
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Drivers.ToListAsync());
        //}
        [Authorize(Policy = "RequireAdminAGSRole")]
        public IActionResult DriverIndex()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            var CompanyGroupList = _context.CompanyGroup.OrderBy(m => m.CompanyGroupName).ToList();
            ViewData["CompanyGroupList"] = new SelectList(CompanyGroupList, "Id", "CompanyGroupName");
            var LocationList = _context.Location.OrderBy(m => m.Id).ToList();
            ViewData["LocationListId"] = new SelectList(LocationList, "Id", "LocationName");
            //end
            return View();
        }
        public IActionResult Index()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end
            return View();
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Birthday,License_Restriction,License_Expiry,Status,MobileNumber,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Birthday,License_Restriction,License_Expiry,Status,MobileNumber,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] Driver driver)
        {
            if (id != driver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }


        [HttpPost]
        public async Task<JsonResult> getAvailableDriver(string dateFilter)
        {

            var qstring = Request.Form["q"].ToString().ToLower();
            var timefrom = HttpContext.Request.Query["timefrom"];
            var timeto = HttpContext.Request.Query["timeto"];
            var d = HttpContext.Request.Query["d"];
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);

            var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("yyyy-MM-dd");
            string _timestampfrom = dtimestamp + " " + timefrom + ":00";
            string _timestampto = dtimestamp + " " + timeto + ":00";

            //DateTime timestampfrom = DateTime.ParseExact(_timestampfrom, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);
            //DateTime timestampto = DateTime.ParseExact(_timestampto, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            DateTime timestampfrom = DateTime.Parse(_timestampfrom);
            DateTime timestampto = DateTime.Parse(_timestampto);

            var checkTripToday = _context.Trip.Where(e =>(
            (e.ServiceStartDate >= timestampfrom && e.ServiceStartDate <= timestampto) ||
            (e.ServiceEndDate >= timestampfrom && e.ServiceEndDate <= timestampto) ||
            (e.ServiceStartDate <= timestampfrom && e.ServiceEndDate >= timestampto)) &&
            e.Status >= 1
            ).Select(e => e.DriverId).Distinct().ToList().ToArray();

            int[] notAvailableIds = new int[200];
            var x = 0;
            foreach (int _data in checkTripToday)
            {
                notAvailableIds[x] = _data;
                x++;
            }
            if (x <= 0)
            {
                notAvailableIds[0] = 0;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            //deleted filter for validating the driver if has existing trip -> !notAvailableIds.Contains(r.Id) &&
            var driversContext = _context.Drivers.AsQueryable();
            if (currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                    .Where(e => e.Id == currentUser.EmployeeId)
                    .SelectMany(e => e.EmployeeLocations.Select(el => el.Location.Id))
                    .ToListAsync();

                driversContext = driversContext
                    .Where(v => v.CompanyGroupId == currentCompanyGroupId &&
                                v.DriverLocations.Any(el => currentUserLocationIds.Contains(el.LocationId)));
            }
            else
            {
                driversContext = driversContext
                    .Where(v => v.CompanyGroupId == currentCompanyGroupId);
            }

            var DriverLists = driversContext.Where(r =>  (r.FirstName.Contains(qstring) || r.LastName.Contains(qstring) || (r.FirstName + " " + r.LastName).Contains(qstring))
                                                            && r.Status >= 1)
                                                           .Select(u => new
                                                           {
                                                               Id = u.Id,
                                                               Text = u.FirstName + " " + u.LastName
                                                           });


            var jsonData = new
            {
                Items = DriverLists.ToList().ToArray(),
                Count = DriverLists.Count()
            };

            return new JsonResult(jsonData);

        }

        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ShowDriverLists()
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

                var DataList = _context.Drivers.OrderBy(e => e.LastName).OrderByDescending(s => s.Status).
                                Select(u => new
                                {
                                    Id = u.Id,
                                    Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> ",
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Status = u.Status == 0 ? "<label style='color:#ec4758;'>Inactive</label>" : "<label style='color:#1ab394'>Active</label>",
                                    Birthday = u.Birthday.ToString("MM/dd/yyyy"),
                                    License_Restriction = u.License_Restriction,
                                    License_Expiry = u.License_Expiry.ToString("MM/dd/yyyy"),
                                    Civil_status = u.CivilStatus,
                                    MobileNumber = u.MobileNumber,
                                    CompanyGroupId = u.CompanyGroupId,
                                    CompanyGroupName = u.CompanyGroup.CompanyGroupName
                                });

                DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);
                // Total record count.
                int totalRecords = DataList.Count();

                // Verification.
                if (!string.IsNullOrEmpty(search))
                {   // Apply search
                    DataList = DataList.Where(x => x.FirstName.ToLower().Contains(search.ToLower()) || x.LastName.Contains(search.ToLower()) || x.Civil_status.Contains(search.ToLower()) || x.MobileNumber.Contains(search.ToLower()));
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
                DataList = DataList.OrderBy(x => x.LastName).OrderBy(s => s.Status);
                var jsonData = new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = DataList.ToList(),
                };

                return new JsonResult(jsonData);
         
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<IActionResult> updateList(int id)
        {

            //var _rolename = "";
            var User = _context.Drivers
                .Include(e => e.DriverLocations)
                .Where(r => r.Id == id).Select(u => new {
                Id = u.Id,
                Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> ",
                FirstName = u.FirstName,
                LastName = u.LastName,
                Status = u.Status ,
                Birthday = u.Birthday.ToString("MM/dd/yyyy"),
                License_Restriction = u.License_Restriction,
                License_Expiry = u.License_Expiry.ToString("MM/dd/yyyy"),
                Civil_status = u.CivilStatus,
                MobileNumber = u.MobileNumber,
                CompanyGroup = u.CompanyGroupId,
                LocationIds = u.DriverLocations.Select(dl => dl.LocationId).ToList()
            });



            var jsonData = new
            {
                userData = User.ToList().ToArray(),
                Success = 1
            };

            return new JsonResult(jsonData);

        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<IActionResult> saveUpdate(int id, Driver drivers)
        {
            var returnSuccess = 1;
            var returnMessage = "";

            //End Manage Parameters

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Driver driverData;

                    if (id == 0)
                    {
                        drivers.EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        drivers.EncodeDate = DateTime.Now;
                        driverData = drivers;
                        driverData.DriverLocations = new List<DriverLocation>();

                        _context.Drivers.Add(driverData);
                        await _context.SaveChangesAsync();

                        if (drivers.LocationIds != null)
                        {
                            foreach (var locId in drivers.LocationIds)
                            {
                                driverData.DriverLocations.Add(new DriverLocation
                                {
                                    DriverId = driverData.Id,
                                    LocationId = locId
                                });
                            }
                        }
                    }
                    else {
                        driverData = await _context.Drivers
                            .Include(d => d.DriverLocations)
                            .FirstOrDefaultAsync(d => d.Id == id);
                        driverData.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        driverData.ModifyDate = DateTime.Now;
                        driverData.FirstName = drivers.FirstName;
                        driverData.LastName = drivers.LastName;
                        driverData.Birthday = drivers.Birthday;
                        driverData.CivilStatus = drivers.CivilStatus;
                        driverData.License_Restriction = drivers.License_Restriction;
                        driverData.License_Expiry = drivers.License_Expiry;
                        driverData.MobileNumber = drivers.MobileNumber;
                        driverData.CompanyGroupId = drivers.CompanyGroupId;
                        driverData.Status = drivers.Status;
                        driverData.DriverLocations.Clear();

                        if (drivers.LocationIds != null)
                        {
                            foreach (var locId in drivers.LocationIds)
                            {
                                driverData.DriverLocations.Add(new DriverLocation
                                {
                                    DriverId = driverData.Id,
                                    LocationId = locId
                                });
                            }
                        }
                        _context.Drivers.Update(driverData);
                    }

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
