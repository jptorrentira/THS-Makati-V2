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
    public class VehicleListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehicleListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminAGSRole")]
        // GET: VehicleLists
        public IActionResult VehicleManagement()
        {
            return View();
        }

        [Authorize(Policy = "RequireAdminAGSRole")]
        // GET: VehicleLists
        public async Task<IActionResult> Index()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end
            var CompanyGroupList = _context.CompanyGroup.OrderBy(m => m.CompanyGroupName).ToList();
            ViewData["CompanyGroupList"] = new SelectList(CompanyGroupList, "Id", "CompanyGroupName");
            var LocationList = _context.Location.OrderBy(m => m.Id).ToList();
            ViewData["LocationListId"] = new SelectList(LocationList, "Id", "LocationName");
            return View(await _context.VehicleLists.ToListAsync());
        }

        // GET: VehicleLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleList = await _context.VehicleLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleList == null)
            {
                return NotFound();
            }

            return View(vehicleList);
        }

        // GET: VehicleLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Model,PlateNumber,CodingDay,Capacity")] VehicleList vehicleList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleList);
        }

        [HttpPost]
        public async Task<IActionResult> createMaintenance([Bind("MaintenanceFrom,MaintenanceTo,MaintenanceRemarks")] MaintenanceLog maintenanceLog)
        {
            var returnSuccess = 1;
            var returnMessage = "";
            var _transactionId = "";
            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 5);
            if (parameterTemp.Year < DateTime.Now.Year)
            {
                parameterTemp.Value = 1;
                parameterTemp.Year = DateTime.Now.Year;
                _context.Update(parameterTemp);
                _context.SaveChanges();
            }



            //End Manage Parameters

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int vehicleId = Convert.ToInt32(Request.Form["id"].ToString());
                    var duplicateMaintenance = _context.MaintenanceLogs.Count(e => e.MaintenanceFrom >= maintenanceLog.MaintenanceFrom && e.MaintenanceTo <= maintenanceLog.MaintenanceTo && e.VehicleListId.Equals(vehicleId));
                    var checkexistingtrip = _context.Trip.Count(e => e.ServiceStartDate.Date >= maintenanceLog.MaintenanceFrom && e.ServiceStartDate <= maintenanceLog.MaintenanceTo && e.VehicleListId.Equals(vehicleId));

                    if (duplicateMaintenance > 0)
                    {
                        throw new System.Exception("Duplicate Maintenance Date not allowed.");
                    }

                    if (checkexistingtrip > 0)
                    {
                        throw new System.Exception("This vehicle have an existing trip for this date. Please delete or cancel it first.");
                    }

                    var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 5);
                    _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                    parameter.Value = parameter.Value + 1;
                    _context.Update(parameter);
                    _context.SaveChanges();

                    maintenanceLog.VehicleListId = vehicleId;
                    maintenanceLog.Process = "Maintenance History";
                    maintenanceLog.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    maintenanceLog.ProcessedDate = DateTime.Now;
                    maintenanceLog.MaintenanceControlNo = _transactionId;

                    _context.Add(maintenanceLog);

                    //var vehicle = _context.VehicleLists.FirstOrDefault(e=> e.Id.Equals(vehicleId));
                    //if (vehicle != null) {
                    //    vehicle.MaintenanceFrom = maintenanceLog.MaintenanceFrom;
                    //    vehicle.MaintenanceTo = maintenanceLog.MaintenanceTo;
                    //    vehicle.MaintenanceRemarks = maintenanceLog.MaintenanceRemarks;
                    //    vehicle.MaintenanceControlNo = maintenanceLog.MaintenanceControlNo;
                    //}

                    var result = await _context.SaveChangesAsync();


                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message + "/" + e.InnerException;
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

        // GET: VehicleLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleList = await _context.VehicleLists.FindAsync(id);
            if (vehicleList == null)
            {
                return NotFound();
            }
            return View(vehicleList);
        }

        // POST: VehicleLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,PlateNumber,CodingDay,Capacity")] VehicleList vehicleList)
        {
            if (id != vehicleList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleListExists(vehicleList.Id))
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
            return View(vehicleList);
        }

        // GET: VehicleLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleList = await _context.VehicleLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleList == null)
            {
                return NotFound();
            }

            return View(vehicleList);
        }

        // POST: VehicleLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleList = await _context.VehicleLists.FindAsync(id);
            _context.VehicleLists.Remove(vehicleList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleListExists(int id)
        {
            return _context.VehicleLists.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<JsonResult> getAvailableVehicle(string dateFilter)
        {

            var qstring = Request.Form["q"].ToString().ToLower();
            var timefrom = HttpContext.Request.Query["timefrom"];
            var timeto = HttpContext.Request.Query["timeto"];
            var d = HttpContext.Request.Query["d"];
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);

            var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("yyyy-MM-dd");
            string _timestampfromStart = dtimestamp + " 00:00:00";
            string _timestampfrom = dtimestamp + " " + timefrom + ":00";
            string _timestampto = dtimestamp + " " + timeto + ":00";

            //DateTime timestampfrom = DateTime.ParseExact(_timestampfrom, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);
            //DateTime timestampto = DateTime.ParseExact(_timestampto, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            DateTime timestampfromStart = DateTime.Parse(_timestampfromStart);
            DateTime timestampfrom = DateTime.Parse(_timestampfrom);
            DateTime timestampto = DateTime.Parse(_timestampto);

            //checking if has trip today
            var checkTripToday = _context.Trip.Where(e => (
            (e.ServiceStartDate >= timestampfrom && e.ServiceStartDate <= timestampto) ||
            (e.ServiceEndDate >= timestampfrom && e.ServiceEndDate <= timestampto) ||
            (e.ServiceStartDate <= timestampfrom && e.ServiceEndDate >= timestampto))
            && e.Status >= 1
            ).Select(e => e.VehicleListId).Distinct().ToList().ToArray();

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
            //end check if has trip today

            //checking if has maintenance today
            var checkMaintenanceToday = _context.MaintenanceLogs.Where(e =>
            timestampfromStart >= e.MaintenanceFrom && timestampfromStart <= e.MaintenanceTo).Select(e => e.VehicleListId).Distinct().ToList().ToArray();

            int[] maintenanceIds = new int[200];
            var mx = 0;
            foreach (int _dataM in checkMaintenanceToday)
            {
                maintenanceIds[mx] = _dataM;
                mx++;
            }
            if (mx <= 0)
            {
                maintenanceIds[0] = 0;
            }
            //end check if has maintenance today


            //get the day of week name

            DateTime dateValue = timestampfrom;


            //end day of week name
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            //deleted filter for coding -> r.CodingDay != (int)dateValue.DayOfWeek &&
            //deleted filter for validating the vehicle if has existing trip -> !notAvailableIds.Contains(r.Id) &&
            var vehicleListContext = _context.VehicleLists.AsQueryable();
            if (currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                    .Where(e => e.Id == currentUser.EmployeeId)
                    .SelectMany(e => e.EmployeeLocations.Select(el => el.Location.Id))
                    .ToListAsync();

                vehicleListContext = vehicleListContext
                    .Where(v => v.CompanyGroupId == currentCompanyGroupId &&
                                v.VehicleLocations.Any(el => currentUserLocationIds.Contains(el.LocationId)));
            }
            else
            {
                vehicleListContext = vehicleListContext
                    .Where(v => v.CompanyGroupId == currentCompanyGroupId);
            }
            var VehicleLists = vehicleListContext.Where(r => !maintenanceIds.Contains(r.Id) && (r.Model.Contains(qstring) || r.PlateNumber.Contains(qstring) || (r.Model + " / " + r.PlateNumber).Contains(qstring))
                                                                && r.Status >= 1)
                                                           .Select(u => new
                                                           {
                                                               Id = u.Id,
                                                               Text = (u.CodingDay == (int)dateValue.DayOfWeek) ? u.Model + " / " + u.PlateNumber + " <b style='color:red'>(Coding)</b>" : u.Model + " / " + u.PlateNumber
                                                           });


            var jsonData = new
            {
                Items = VehicleLists.ToList().ToArray(),
                Count = VehicleLists.Count()
            };

            return new JsonResult(jsonData);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ShowVehicleLists()
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

            var DataList = _context.VehicleLists.Where(e => e.Id > 0).
                            Select(u => new
                            {
                                Id = u.Id,
                                Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> " + "<a href='javascript:void(0);' onClick='Maintenance(`" + u.Id + "`,`" + u.Model + " / " + u.PlateNumber + "`)'  class='btn btn-sm btn-warning' > Maintenance </a> ",
                                Model = u.Model,
                                PlateNumber = u.PlateNumber,
                                Status = u.Status == 0 ? "<label style='color:#ec4758;'>Inactive</label>" : "<label style='color:#1ab394'>Active</label>",
                                Capacity = u.Capacity,
                                Coding = u.CodingDay,
                                CodingDayName = u.CodingDay == 1 ? "Monday" : u.CodingDay == 2 ? "Tuesday" : u.CodingDay == 3 ? "Wednesday" : u.CodingDay == 4 ? "Thursday" : u.CodingDay == 5 ? "Friday" : u.CodingDay == 6 ? "Saturday" : u.CodingDay == 7 ? "Sunday" : "N/A",
                                CompanyGroupId = u.CompanyGroupId,
                                CompanyGroupName = u.CompanyGroup.CompanyGroupName
                            });

            DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);
            // Total record count.
            int totalRecords = DataList.Count();

            // Verification.
            if (!string.IsNullOrEmpty(search))
            {   // Apply search
                DataList = DataList.Where(x => x.Model.ToLower().Contains(search.ToLower()) || x.PlateNumber.Contains(search.ToLower()) || x.CodingDayName.Contains(search.ToLower()));
            }
            // Sorting.
            //string[] sort = new string[] { "Name", "Name" };
            //var sortfield = sort[int.Parse(order)];
            //DataList = DataList.OrderBy(x=> x.Name);

            // Filter record count.
            int recFilter = DataList.Count();

            DataList = DataList.OrderBy(o => o.Model);

            // Apply pagination.
            DataList = DataList.Skip(startRec).Take(pageSize);

            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = DataList.ToList(),
            };

            return new JsonResult(jsonData);

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ShowMaintenanceLists()
        {


            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            string vehicleId = Request.Form["vehicleId"];

            //1 is monday
            //7 is sunday

            var DataList = _context.MaintenanceLogs.OrderByDescending(e => e.Id).Where(e => e.VehicleListId.Equals(Convert.ToInt32(vehicleId))).
                            Select(u => new
                            {
                                Id = u.Id,
                                Remarks = u.MaintenanceRemarks,
                                From = u.MaintenanceFrom.GetValueOrDefault().ToString("MM/dd/yyyy"),
                                To = u.MaintenanceTo.GetValueOrDefault().ToString("MM/dd/yyyy"),
                                ControlNo = u.MaintenanceControlNo,
                                Process = u.Process,
                                ProcessedBy = u.ProcessedBy
                            });


            // Total record count.
            int totalRecords = DataList.Count();

            // Verification.
            if (!string.IsNullOrEmpty(search))
            {   // Apply search
                DataList = DataList.Where(x => x.Remarks.ToLower().Contains(search.ToLower()) || x.Process.Contains(search.ToLower()) || x.ControlNo.Contains(search.ToLower()) || x.ProcessedBy.Contains(search.ToLower()));
            }
            // Sorting.
            //string[] sort = new string[] { "Name", "Name" };
            //var sortfield = sort[int.Parse(order)];
            //DataList = DataList.OrderBy(x=> x.Name);

            // Filter record count.
            int recFilter = DataList.Count();

            //DataList = DataList.OrderBy(o => o.Model);

            // Apply pagination.
            DataList = DataList.Skip(startRec).Take(pageSize);

            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = DataList.ToList(),
            };

            return new JsonResult(jsonData);

        }


        [Authorize(Policy = "RequireAdminAGSRole")]
        [HttpPost]
        public async Task<IActionResult> updateList(int id)
        {

            //var _rolename = "";
            var User = _context.VehicleLists
                .Include(e => e.VehicleLocations)
                .Where(r => r.Id == id).Select(u => new {
                VehicleId = u.Id,
                Model = u.Model,
                PlateNumber = u.PlateNumber,
                Capacity = u.Capacity,
                Status = u.Status,
                Coding = u.CodingDay,
                CodingDayName = u.CodingDay == 1 ? "Monday" : u.CodingDay == 2 ? "Tuesday" : u.CodingDay == 3 ? "Wednesday" : u.CodingDay == 4 ? "Thursday" : u.CodingDay == 5 ? "Friday" : u.CodingDay == 6 ? "Saturday" : u.CodingDay == 7 ? "Sunday" : "N/A",
                CompanyGroup = u.CompanyGroupId,
                LocationIds = u.VehicleLocations.Select(dl => dl.LocationId).ToList()
            });



            var jsonData = new
            {
                userData = User.ToList().ToArray(),
                Success = 1
            };

            return new JsonResult(jsonData);

        }


        [Authorize(Policy = "RequireAdminAGSRole")]
        [HttpPost]
        public async Task<IActionResult> saveUpdate(int id, VehicleList vehicleList)
        {
            var returnSuccess = 1;
            var returnMessage = "";

            //End Manage Parameters

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    VehicleList vehicleData;

                    if (id == 0)
                    {
                        vehicleData = vehicleList;
                        vehicleData.VehicleLocations = new List<VehicleLocation>();

                        _context.VehicleLists.Add(vehicleData);
                        await _context.SaveChangesAsync();

                        if (vehicleData.LocationIds != null)
                        {
                            foreach (var locId in vehicleData.LocationIds)
                            {
                                vehicleData.VehicleLocations.Add(new VehicleLocation
                                {
                                    VehicleId = vehicleData.Id,
                                    LocationId = locId
                                });
                            }
                        }
                    }
                    else
                    {
                        vehicleData = await _context.VehicleLists
                            .Include(d => d.VehicleLocations)
                            .FirstOrDefaultAsync(d => d.Id == id);
                        vehicleData.Model = vehicleList.Model;
                        vehicleData.PlateNumber = vehicleList.PlateNumber;
                        vehicleData.Capacity = vehicleList.Capacity;
                        vehicleData.CodingDay = vehicleList.CodingDay;
                        vehicleData.CompanyGroupId = vehicleList.CompanyGroupId;
                        vehicleData.Status = vehicleList.Status;

                        vehicleData.VehicleLocations.Clear();

                        if (vehicleList.LocationIds != null)
                        {
                            foreach (var locId in vehicleList.LocationIds)
                            {
                                vehicleData.VehicleLocations.Add(new VehicleLocation
                                {
                                    VehicleId = vehicleData.Id,
                                    LocationId = locId
                                });
                            }
                        }
                        _context.VehicleLists.Update(vehicleData);
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
