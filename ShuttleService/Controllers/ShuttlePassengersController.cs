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
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;

namespace ShuttleService.Controllers
{

    [Authorize(Policy = "RequireAllRole")]
    public class ShuttlePassengersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // database connection for user management only
        private readonly SignInManager<ApplicationUser> _signInManager; // database connection for login

        public ShuttlePassengersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager; //for user management only
            _signInManager = signInManager; //for user login only
        
        }
        private void ResetContextState() => _context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        // GET: ShuttlePassengers
        public async Task<IActionResult> Index()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end

            if (Request.Cookies["showGuideline"] == "1")
            {
                ViewBag.showGuidelines = 1;
            }
            else
            {
                ViewBag.showGuidelines = 0;

            }

            Response.Cookies.Append("showGuideline", "0");

            var applicationDbContext = _context.ShuttlePassengers.Include(s => s.PassengerType).Include(s => s.TripType);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> IndexDrivers()
        {
            var applicationDbContext = _context.ShuttlePassengers.Include(s => s.PassengerType).Include(s => s.TripType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ShuttlePassengers/Details/5
        public async Task<IActionResult> Details()
        {
     
            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            ViewBag.unixtime = unixTime * 1000;
            ViewBag.unixtimeFormatted = unixTime;

            ViewBag.t = HttpContext.Request.Query["t"].ToString();

            var shuttlePassenger = await _context.ShuttlePassengers
                .FirstOrDefaultAsync(m => m.ServiceDateTimeStamp == unixTime);

            String today = DateTime.Today.ToString();

            //ViewBag.invalidDate = 0; 
            //if (ViewBag.dtimestamp < today) {
            //    ViewBag.invalidDate = 1;
            //}


            int[] NonRejectedCancelledStatus = { 1,2,4 };
            ViewBag.AMPassengerCount = _context.ShuttlePassengers.Count(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 2) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.PMPassengerCount = _context.ShuttlePassengers.Count(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 3) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.AssemblyArea = "In Front of DMCI Annex Building";


            ViewBag.txtAM = 0;
            ViewBag.txtPM = 0;
            var _trip = _context.Trip.FirstOrDefault(e => e.ServiceDateTimeStamp.Equals(unixTime) && e.ReservationTypeId.Equals(1));
            ViewData["Trip"] = _trip;
            if (_trip != null) { 
            var _veh = _context.VehicleLists.FirstOrDefault(e => e.Id.Equals(_trip.VehicleListId));
            var _driver = _context.Drivers.FirstOrDefault(e => e.Id.Equals(_trip.DriverId));
                ViewBag.txtVehicle = _veh.Model + "/" + _veh.PlateNumber;
                ViewBag.txtCapacity = _trip.Capacity;
                ViewBag.txtDriver = _driver.FirstName + " " + _driver.LastName;
                ViewBag.txtAM = _context.ShuttlePassengers.Where(e => e.ShuttleId.Equals(_trip.Id)  && (e.TripTypeId.Equals(1) || e.TripTypeId.Equals(2))).Count();
                ViewBag.txtPM = _context.ShuttlePassengers.Where(e => e.ShuttleId.Equals(_trip.Id) && (e.TripTypeId.Equals(1) || e.TripTypeId.Equals(3))).Count();
            }
            //ViewBag.txtDriver = _trip.Driver.FirstName + "/" + _trip.Driver.LastName;
           

            return View(shuttlePassenger);
        }

        // GET: ShuttlePassengers/Create
        //[HttpGet("ShuttlePassengers/Create/{d}")]
        public async Task<IActionResult> Create()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end
            ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description");
            ViewData["ShuttleId"] = new SelectList(_context.Shuttles, "Id", "Id");
            ViewData["TripTypeId"] = new SelectList(_context.TripTypes, "Id", "Description");

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var ChargingCompanysList = _context.ChargingCompanys.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.CompanyName).ToList();
            ViewData["ChargingCompanyId"] = new SelectList(ChargingCompanysList, "Id", "CompanyName");

            var ChargingDepartmentsList = _context.ChargingDepartments.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.DepartmentName).ToList();
            ViewData["ChargingDepartmentId"] = new SelectList(ChargingDepartmentsList, "Id", "DepartmentName");

            var CompanyListsList = _context.CompanyLists.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.CompanyName).ToList();
            ViewData["CompanyId"] = new SelectList(CompanyListsList, "Id", "CompanyName");

            var NationalityList = _context.Nationalities.OrderBy(m => m.NationalityName).ToList();
            ViewData["Nationality"] = new SelectList(NationalityList, "NationalityName", "NationalityName");

            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dGet = unixTime;
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");

            var employee = _context.Employees.FirstOrDefault(e=> e.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))));

            ViewBag.defaultChargingCompanyId = employee.ChargingCompanyId;
            ViewBag.defaultChargingDepartmentId = employee.ChargingDepartmentId;
            ViewBag.defaultChargingDepartmentName = _context.ChargingDepartments.FirstOrDefault(e => e.Id.Equals(employee.ChargingDepartmentId)).DepartmentName;

            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            ViewBag.OverAllApprover = roles;

            ViewBag.FromTrip = HttpContext.Request.Query["ft"].ToString();
            //end checking the role if overall approver


            //var checkroleIds = _context.Roles.Where(r => roles.Contains(r.Name)).Select(u => new {
            //    RoleIds = u.Id
            //});

            return View();
        }

        // POST: ShuttlePassengers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ShuttleId,PassengerTypeId,EmployeeNo,CompanyId,ChargingCompanyId,ChargingDepartmentId,Name,Position,Purpose,Remarks,TripTypeId,Breakfast,AmSnack,PmSnack,Lunch,Dinner")] ShuttlePassenger shuttlePassenger)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(shuttlePassenger);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", shuttlePassenger.PassengerTypeId);
        //    ViewData["ShuttleId"] = new SelectList(_context.Shuttles, "Id", "Id", shuttlePassenger.ShuttleId);
        //    ViewData["TripTypeId"] = new SelectList(_context.TripTypes, "Id", "Description", shuttlePassenger.TripTypeId);
        //    return View(shuttlePassenger);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,PassengerTypeId,EmployeeNo,CompanyListId,CompanyOther," +
            "ChargingCompanyId,ChargingDepartmentId,Name,Position,Purpose," +
            "Remarks,TripTypeId,Breakfast,AmSnack,PmSnack,Lunch,Dinner," +
            "FirstName,MiddleName,LastName,LodgingTo,ServiceDateTimeStamp,ContactNo,Nationality,Gender")]
            ShuttlePassenger shuttlePassenger)
        {
            var returnSuccess = 1;
            var returnMessage = "";

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int[] NonDuplicateStatus = { 1, 2, 4,6 };


                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(shuttlePassenger.ServiceDateTimeStamp).ToLocalTime();

                    long unixTime = Convert.ToInt64(shuttlePassenger.ServiceDateTimeStamp);
                    var _lodgingFrom = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("yyyy-MM-dd 00:00:00.0000000");

                    shuttlePassenger.LodgingFrom = Convert.ToDateTime(_lodgingFrom);

                    //commented as requested by AGS 05062020 EBE during ECQ
                    if (dtDateTime < DateTime.Now)
                    {
                        throw new System.Exception("Invalid reservation date.");
                    }

                    shuttlePassenger.EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    shuttlePassenger.ServiceDate = dtDateTime;
                    shuttlePassenger.EncodeDate = DateTime.Now;
                    shuttlePassenger.ModifyDate = DateTime.Now;
                    shuttlePassenger.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"));
                    shuttlePassenger.Status = 6;


                    if (shuttlePassenger.CompanyListId > 0)
                    {
                        //wala lang.
                    }
                    else {
                        shuttlePassenger.CompanyListId = 1;
                    }

                    if (shuttlePassenger.PassengerTypeId == 1)
                    {
                        if (_context.Employees.FirstOrDefault(emp => emp.EmployeeNo == shuttlePassenger.EmployeeNo) == null)
                        {

                            throw new System.Exception("Employee number does not exist to the system."); //if employee number is not exist
                        }

                        if (_context.ShuttlePassengers.Count(s => NonDuplicateStatus.Contains(s.Status) && s.ServiceDateTimeStamp == shuttlePassenger.ServiceDateTimeStamp && s.EmployeeNo == shuttlePassenger.EmployeeNo && s.TripTypeId == shuttlePassenger.TripTypeId) > 0)
                        {
                            throw new System.Exception("Passenger already booked on this date."); // if already have reservation with same trip type
                        }
                        else if (_context.ShuttlePassengers.Count(s => NonDuplicateStatus.Contains(s.Status) && s.ServiceDateTimeStamp == shuttlePassenger.ServiceDateTimeStamp && s.EmployeeNo == shuttlePassenger.EmployeeNo && (s.TripTypeId == 2 || s.TripTypeId == 3)) > 0 && shuttlePassenger.TripTypeId == 1)
                        {
                            throw new System.Exception("Passenger already booked on this date."); // if has reservation for calaca - makati vice versa but making roundtrip
                        }
                        else if (_context.ShuttlePassengers.Count(s => NonDuplicateStatus.Contains(s.Status) && s.ServiceDateTimeStamp == shuttlePassenger.ServiceDateTimeStamp && s.EmployeeNo == shuttlePassenger.EmployeeNo && (s.TripTypeId == 1)) > 0 && (shuttlePassenger.TripTypeId == 2 || shuttlePassenger.TripTypeId == 3))
                        {
                            throw new System.Exception("Passenger already booked on this date."); // if has roundtrip but making other reservation
                        }

                        var checkApprover = _context.Employees.FirstOrDefault(e => e.EmployeeNo == shuttlePassenger.EmployeeNo);
                        shuttlePassenger.InitialApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                        shuttlePassenger.OriginalApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                        shuttlePassenger.Gender = checkApprover.Gender;
                        shuttlePassenger.ContactNo = checkApprover.LocalNumber + " / " + checkApprover.MobileNumber;
                    }
                    else
                    {
                        shuttlePassenger.EmployeeNo = "";
                        var checkApprover = _context.Employees.FirstOrDefault(e => e.Id.ToString() == HttpContext.Session.GetString("Session_employeeId"));
                        shuttlePassenger.InitialApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                        shuttlePassenger.OriginalApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                    }

                    //logs
                    Log _log = new Log();
                    _log.Process = "Add Shuttle Reservation For " + shuttlePassenger.FirstName + " " + shuttlePassenger.LastName + " / Time Stamp: " + shuttlePassenger.ServiceDateTimeStamp;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

                    _context.Add(shuttlePassenger);
                    var result = await _context.SaveChangesAsync();


                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message + "/" + e.InnerException;
                    if (returnMessage.Contains("instance"))
                    {
                        returnMessage = returnMessage + " Please check all the details.";
                    }
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

        // GET: ShuttlePassengers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shuttlePassenger = await _context.ShuttlePassengers.FindAsync(id);
            if (shuttlePassenger == null)
            {
                return NotFound();
            }
            ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", shuttlePassenger.PassengerTypeId);
            ViewData["ShuttleId"] = new SelectList(_context.Shuttles, "Id", "Id", shuttlePassenger.ShuttleId);
            ViewData["TripTypeId"] = new SelectList(_context.TripTypes, "Id", "Description", shuttlePassenger.TripTypeId);
            return View(shuttlePassenger);
        }

        // POST: ShuttlePassengers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShuttleId,PassengerTypeId,EmployeeNo,CompanyListId,ChargingCompanyId,ChargingDepartmentId,Name,Position,Purpose,Remarks,TripTypeId,Breakfast,AmSnack,PmSnack,Lunch,Dinner")] ShuttlePassenger shuttlePassenger)
        {
            if (id != shuttlePassenger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shuttlePassenger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShuttlePassengerExists(shuttlePassenger.Id))
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
            ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", shuttlePassenger.PassengerTypeId);
            ViewData["ShuttleId"] = new SelectList(_context.Shuttles, "Id", "Id", shuttlePassenger.ShuttleId);
            ViewData["TripTypeId"] = new SelectList(_context.TripTypes, "Id", "Description", shuttlePassenger.TripTypeId);
            return View(shuttlePassenger);
        }

        // GET: ShuttlePassengers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shuttlePassenger = await _context.ShuttlePassengers
                .Include(s => s.PassengerType)
                .Include(s => s.TripType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shuttlePassenger == null)
            {
                return NotFound();
            }

            return View(shuttlePassenger);
        }

        // POST: ShuttlePassengers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shuttlePassenger = await _context.ShuttlePassengers.FindAsync(id);
            _context.ShuttlePassengers.Remove(shuttlePassenger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShuttlePassengerExists(int id)
        {
            return _context.ShuttlePassengers.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<JsonResult> ReloadCalendar(string dateFilter, string method)
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];

            string _status = "OK";
            string _message = "";

            string iDate = dateFilter;
            DateTime oDate = Convert.ToDateTime(iDate);
            DateTime nextMonth = oDate.AddDays(31);
            DateTime lastMonth = oDate.AddDays(-31);
            int[] NonDuplicateStatus = { 1, 2,3, 4 };

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var PassengerList = _context.ShuttlePassengers.Where(r => NonDuplicateStatus.Contains(r.Status)
                && r.ServiceDate >= lastMonth
                && r.ServiceDate <= nextMonth
                && _context.Employees
                       .Where(e => e.Id == r.EmployeeId)
                       .Select(e => e.CompanyGroupId)
                       .FirstOrDefault() == currentCompanyGroupId
                )
                                                           //.GroupBy(x => x.FirstName)
                                                           .Select(u => new
                                                           {
                                                               Title = u.FirstName + " " + u.LastName,
                                                               //EventTimeStamp = u.ServiceDateTimeStamp,
                                                               Start = u.ServiceDate,
                                                               Fname = u.FirstName,
                                                               Lname = u.LastName,
                                                           }).GroupBy(x => new { x.Fname, x.Lname, x.Start });





            var jsonData = new
            {
                Status = _status,
                Message = _message,
                Events = PassengerList.ToList().ToArray(),
                DateFilter = oDate
            };

            return new JsonResult(jsonData);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CheckPassengerReservation()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var tripTypeEqual = 0;
            if (Request.Form["triptype"] == "morning")
            {
                tripTypeEqual = 2;
            }
            else if (Request.Form["triptype"] == "afternoon")
            {
                tripTypeEqual = 3;
            }
            int[] StatustoShow = { 1, 2, 3, 4, 5, 7 };

            //var DataList = _context.ShuttlePassengers.Where(r => r.Status < 5 && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"]) && (r.TripTypeId.Equals(1) || r.TripTypeId.Equals(tripTypeEqual)))
            //                                                                        .Select(u => new
            //                                                                         {
            //                                                                             No = u.Id,
            //                                                                             Name = u.FirstName + " " + u.MiddleName + " " + u.LastName,
            //                                                                             ContactNumber = u.ContactNo,
            //                                                                             StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>RESERVED - As of " + u.ApprovedDatetime + "</b>" : u.Status == 3 ? "Rejected" : u.Status == 4 ? "Closed" : u.Status == 5 ? "Cancelled" : "",
            //                                                                             Remarks = u.Remarks
            //                                                                         });

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var DataList = _context.ShuttlePassengers.AsEnumerable().
                            OrderBy(x => x.Name).
                            Where(r => StatustoShow.Contains(r.Status) && r.ShuttleId <=0 
                                && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"]) 
                                && (r.TripTypeId.Equals(1) || r.TripTypeId.Equals(tripTypeEqual))
                                && _context.Employees
                                   .Where(e => e.Id == r.EmployeeId)
                                   .Select(e => e.CompanyGroupId)
                                   .FirstOrDefault() == currentCompanyGroupId
                                ).
                            Select((u, index) => new
                                {
                                    No = index + 1,
                                    Name = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                                    ContactNumber = u.ContactNo,
                                    //StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + u.ApprovedDatetime + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : "",
                                    StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + u.ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss ") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED:</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "<b style='color:orange!important'>" + " CANCELLED - By: " + u.CancelledBy + " - Reason: " + u.CancelReason + "</b> ": u.Status == 6 ? "OPEN" : u.Status == 8 ? "REMOVED" : u.Status == 7 ? "<b style='color:orange!important'>" + " DECLINED - By: " + u.DeclinedBy + " - Reason: " + u.DeclinedReason + "</b> " : "",
                                    Remarks = u.Remarks
                                });
            // Total record count.
            int totalRecords = DataList.Count();

            // Verification.
            //if (!string.IsNullOrEmpty(search))
            //{   // Apply search
            //    DataList = DataList.Where(x => x.FirstName.ToLower().Contains(search.ToLower())
            //                             && x.MiddleName.ToLower().Contains(search.ToLower())
            //                             && x.LastName.ToLower().Contains(search.ToLower())
            //                             );
            //}
            // Sorting.
            //string[] sort = new string[] { "Name", "Name" };
            //var sortfield = sort[int.Parse(order)];
            //DataList = DataList.OrderBy(x=> x.Name);

            // Filter record count.
            int recFilter = DataList.Count();

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
        public async Task<ActionResult> CheckPassengerReservationReserved()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            
            var tripTypeEqual = 0;
            if (Request.Form["triptype"] == "morning")
            {
                tripTypeEqual = 2;
            }
            else if (Request.Form["triptype"] == "afternoon")
            {
                tripTypeEqual = 3;
            }
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var DataList = _context.ShuttlePassengers.AsEnumerable().
                            OrderBy(x => x.Name).
                            Where(r => r.Status == 2 && r.ShuttleId == Convert.ToInt32(Request.Form["shuttleid"]) 
                                && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"]) 
                                && (r.TripTypeId.Equals(1) || r.TripTypeId.Equals(tripTypeEqual))
                                && _context.Employees
                                   .Where(e => e.Id == r.EmployeeId)
                                   .Select(e => e.CompanyGroupId)
                                   .FirstOrDefault() == currentCompanyGroupId
                                ).
                            Select((u, index) => new
                            {
                                //No = index + 1,

                                //No = "<a href='javascript:void(0);' class='btn btn-sm btn-danger' onClick='RemoveAssign(`" + u.Id + "`);'> Remove </a>",
                                No = "<div class='checkbox checkbox-danger'><input id='checkselectR" + tripTypeEqual + "_" + u.Id + "' value='" + u.Id + "' type='checkbox' class='checkBoxGroupR'  onClick='checkOther(" + u.Id + ",this,`R`);'><label for='checkselectR" + tripTypeEqual + "_" + u.Id + "'  onClick='checkOther(" + u.Id + ",this,`R`);'></label></div>",
                                Name = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                                ContactNumber = u.ContactNo,
                                StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + u.ApprovedDatetime + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : "",
                                Remarks = u.Remarks,
                            });


            // Total record count.
            int totalRecords = DataList.Count();

         

            // Filter record count.
            int recFilter = DataList.Count();

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
        public async Task<ActionResult> CheckPassengerReservationAvailable()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var tripTypeEqual = 0;
            if (Request.Form["triptype"] == "morning")
            {
                tripTypeEqual = 2;
            }
            else if (Request.Form["triptype"] == "afternoon")
            {
                tripTypeEqual = 3;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var DataList = _context.ShuttlePassengers.AsEnumerable().
                            OrderBy(x => x.Name).
                            Where(r => r.Status == 2 
                                && r.ShuttleId <= 0 
                                && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"]) 
                                && (r.TripTypeId.Equals(1) || r.TripTypeId.Equals(tripTypeEqual))
                                && _context.Employees
                                   .Where(e => e.Id == r.EmployeeId)
                                   .Select(e => e.CompanyGroupId)
                                   .FirstOrDefault() == currentCompanyGroupId
                                ).
                            Select((u, index) => new
                            {
                                //No = index + 1,
                                //No = "<a href='javascript:void(0);' class='btn btn-sm btn-primary' onClick='Assign(`" + u.Id + "`);'> Reserve </a>",
                                No = "<div class='checkbox checkbox-success'><input id='checkselect" + tripTypeEqual + "_" + u.Id + "' value='" + u.Id + "' onClick='checkOther("+ u.Id + ",this,``);' type='checkbox' class='checkBoxGroup'><label for='checkselect" + tripTypeEqual + "_" + u.Id + "' onClick='checkOther(" + u.Id + ",this,``);'></label></div>",                          
                                Name = u.FirstName + " " + (u.MiddleName == null  || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                                ContactNumber = u.ContactNo,
                                StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + u.ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss ") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : "",
                                Remarks = u.Remarks,
                                Action =    (u.Status == 2 || u.Status == 9) ?
                                            "<a href='javascript:void(0);'  onClick='cancelTrip(" + u.Id + ")'   class='btn btn-block btn-sm btn-danger' > Cancel </a>" +
                                            "<a href='javascript:void(0);'  onClick='declineTrip(" + u.Id + ")'    class='btn btn-block btn-sm btn-warning' > Decline </a>" +
                                            ""
                                            : ""
                            });


            // Total record count.
            int totalRecords = DataList.Count();



            // Filter record count.
            int recFilter = DataList.Count();

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
        public ActionResult ShowPassengerLists()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);
            //int[] RejectedStatus = { 5 };

            var DataList = _context.ShuttlePassengers.Where(r => r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"]) && r.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))) &&  (r.Status == 1 || r.Status == 2 || r.Status == 6) )
                            .Select(u => new
                            {
                                Id = u.Status == 6 ? "<a href='javascript:void(0);' class='btn btn-sm btn-primary' onClick='RemoveEmployeeAjax(`" + u.Id + "`);'> Remove </a>" : "",
                                Company = u.CompanyListId == 1 ?  u.CompanyList.CompanyName + " " + u.CompanyOther : u.CompanyList.CompanyName,
                                EmployeeID = u.EmployeeNo,
                                PassengerName = u.FirstName + " " + (u.MiddleName == null  || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ")  + u.LastName,
                                Position = u.Position,
                                Purpose = u.Purpose,
                                Remarks = u.Remarks,
                                TripType = u.TripType.Description,
                                Breakfast = u.Breakfast.ToString() == "1" ? "<div class='checkbox checkbox-success'><input id='checkbf" + u.Id + "' disabled type='checkbox' checked><label for='checkbf" + u.Id+"'></label></div>" : "",
                                AMSnacks = u.AmSnack.ToString() == "1" ? "<div class='checkbox checkbox-success'><input id='checkam" + u.Id + "' disabled type='checkbox' checked><label for='checkam" + u.Id + "'></label></div>" : "",
                                Lunch = u.Lunch.ToString() == "1" ? "<div class='checkbox checkbox-success'><input id='checkl" + u.Id + "' disabled type='checkbox' checked><label for='checkl" + u.Id + "'></label></div>" : "",
                                PMSnacks = u.PmSnack.ToString() == "1" ? "<div class='checkbox checkbox-success'><input id='checkpm" + u.Id + "' disabled type='checkbox' checked><label for='checkpm" + u.Id + "'></label></div>" : "",
                                Dinner = u.Dinner.ToString() == "1" ? "<div class='checkbox checkbox-success'><input id='checkd" + u.Id + "' disabled type='checkbox' checked><label for='checkd" + u.Id + "'></label></div>" : "",
                                LodgingFrom = u.LodgingFrom.GetValueOrDefault().ToString("MM/dd/yyyy"),
                                LodgingTo = u.LodgingTo.GetValueOrDefault().ToString("MM/dd/yyyy")
                            });





                           // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = DataList.Count();

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

        [HttpPost]
        public async Task<IActionResult> removeReservation(int id)
        {
            var returnSuccess = 1;
            var returnMessage = "";


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var shuttlePassenger = _context.ShuttlePassengers.FirstOrDefault(s => s.Id == id);
                    shuttlePassenger.Status = 8;
                    _context.Update(shuttlePassenger);
                    await _context.SaveChangesAsync();

                    //logs
                    Log _log = new Log();
                    _log.Process = "Remove Shuttle Reservation For ID" + id + " -> " + shuttlePassenger.FirstName + " " + shuttlePassenger.LastName + " / Time Stamp: " + shuttlePassenger.ServiceDateTimeStamp;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

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

        [HttpPost]
        public async Task<IActionResult> submitReservation(int timestamp)
        {
            ResetContextState();
            var returnSuccess = 1;
            var returnMessage = "";
            var _otherReservation = "";
            var _transactionId = "";



            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();


            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 1);
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
                try {

                    //commented as requested by AGS 05062020 EBE during ECQ
                    if (dtDateTime < DateTime.Now)
                    {
                        throw new System.Exception("Invalid reservation date.");
                    }

                    var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.ServiceDateTimeStamp == timestamp && s.Status == 6 && s.EncodedBy.Equals(HttpContext.Session.GetString("Session_userDomainWithName")));

                    if (shuttlePassenger.Count() <= 0) {
                        throw new System.Exception("There is no reservation to submit.");
                    }
                    var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 1);
                    _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                    parameter.Value = parameter.Value + 1;
                    _context.Update(parameter);
                    _context.SaveChanges();

                  
                    


                    //logs
                    Log _log = new Log();
                    _log.Process = "Submit Reservation Trans No.: " + _transactionId;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

                    string[] smsArr = new string[999];
                    var smsCodesArr = new Dictionary<string, string>();
                    int smsArrIndex = 0;
                    string smsReferenceNo = "";
                    //await shuttlePassenger.ForEachAsync(e => { //JPT commented code 09182024
                    foreach (var e in shuttlePassenger){ //JPT additional code 09182024

                        e.Status = 1;
                        e.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        e.ModifyDate = DateTime.Now;
                        e.SubmitDate = DateTime.Now;
                        e.TransactionId = _transactionId;

                        //if (_context.SmsTransactionCodes.Count(s => s.TransactionId.Equals(_transactionId) && s.ApproverEmployeeNo.Equals(e.OriginalApproverEmployeeNo)) == 0) {
                        if (!smsArr.Contains(e.OriginalApproverEmployeeNo))
                        { //new if
                            var tempSmsTransactionCodes = new SmsTransactionCode();
                            //generating sms Reference No

                            do
                            {
                                smsReferenceNo = TripsController.RandomString(6);

                                //ADDED BY EBE during ECQ
                                if (System.Environment.MachineName == "SODIUM2")
                                {
                                    //smsReferenceNo = "T" + smsReferenceNo;
                                    smsReferenceNo = "A" + smsReferenceNo;
                                   
                                }
                                else if (System.Environment.MachineName == "ALUMINUM" || System.Environment.MachineName == "CALIFORNIUM")
                                {
                                    smsReferenceNo = "M" + smsReferenceNo;
                                }
                                else if (System.Environment.MachineName == "ANDROMEDA")
                                {
                                    smsReferenceNo = "C" + smsReferenceNo;
                                }
                                else {
                                    smsReferenceNo = "L" + smsReferenceNo;
                                }
                                //End ADDED BY EBE during ECQ

                            } while (_context.SmsTransactionCodes.Count(s => s.ReferenceNo.Equals(smsReferenceNo)) > 0);
                            //as of 01-29-2020 -> ApproverEmployeeNo and OriginalApproverEmployeeNo will be the same

                            tempSmsTransactionCodes.ReferenceNo = smsReferenceNo;
                            tempSmsTransactionCodes.TransactionId = _transactionId;
                            tempSmsTransactionCodes.ApproverEmployeeNo = e.OriginalApproverEmployeeNo;
                            tempSmsTransactionCodes.OriginalApproverEmployeeNo = e.OriginalApproverEmployeeNo;
                            tempSmsTransactionCodes.ReservationTypeId = 1;
                            _context.Add(tempSmsTransactionCodes);
                            smsArr[smsArrIndex] = e.OriginalApproverEmployeeNo;
                            smsCodesArr.Add(e.OriginalApproverEmployeeNo, smsReferenceNo);
                            //_context.SaveChanges();
                            //end
                            e.SmsId = smsReferenceNo;
                            smsArrIndex++;

                            //sending message

                            string _names = "";
                            var _getname = _context.ShuttlePassengers.Where(g => g.ServiceDateTimeStamp == timestamp && g.OriginalApproverEmployeeNo.Equals(e.OriginalApproverEmployeeNo) && g.Status.Equals(6)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).Distinct().ToList().ToArray();
                          
                            foreach (string fullname in _getname.Select(x=>x.FullName))
                            {
                                _names = _names + ", " + fullname;
                            }

                            string _destination = "";
                            if (e.TripTypeId == 1)
                            {
                                _destination = "Makati HO - Calaca (Roundtrip)";
                            }
                            else if (e.TripTypeId == 2)
                            {
                                _destination = "Calaca";
                            }
                            else if (e.TripTypeId == 3)
                            {
                                _destination = "Makati HO";
                            }


                            string _TxtMessage = "Ref code: " + smsReferenceNo + Environment.NewLine +
                                                "A vehicle request of " + _names + " going to " + _destination + " on " + e.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + " is pending for your approval. " +
                                                 //"Type VR 2 if approve or VR 3 if disapprove with the ref code. Ex. VR 2 "+ smsReferenceNo + " . Ty. "; //JPT commented code 09182024
                                                "Type 1 " + smsReferenceNo + " THSM for approval or 2 " + smsReferenceNo + " THSM for disapprove. Ex: 1 " + smsReferenceNo + " THSM and send response to: 09191610404."; //JPT additional code 09182024

                            var _MobileNumber = "";
                            /*  if (e.PassengerTypeId == 1)
                              { */
                                if (e.OriginalApproverEmployeeNo != "")
                                _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).MobileNumber;
                            /*   }
                               else
                               { 
                                   _MobileNumber = e.ContactNo;
                               }*/


                            if (_MobileNumber != "") {
                                //new ReportsController().CallSMSAPI(_MobileNumber, _TxtMessage, smsReferenceNo); //JPT commented code 09182024

                                //JPT additional code 09182024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _TxtMessage;
                                string referenceNo = smsReferenceNo;  // Replace with your transaction ID
                                string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09182024
                            }

                            var _Email = "";
                            if (e.PassengerTypeId == 1)
                            {
                                _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).CompanyEmail;
                            }
                            //var _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).CompanyEmail;
                            if (_Email != "")
                            {
                                var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;
                                var callback = Url.Action(nameof(Approval), "shuttlepassengers", new { s = 1 });

                                //ADDED BY EBE during ECQ
                                var hostLink = "";
                                if (System.Environment.MachineName == "SODIUM2")
                                {
                                    //hostLink = "http://sodium2/TransportHub/shuttlepassengers/approval";
                                    hostLink = "http://sodium2/TransportHubCPC/shuttlepassengers/approval";
                                }
                                else if (System.Environment.MachineName == "ALUMINUM")
                                {
                                    hostLink = "https://ictsystems.semirarampc.com:8443/TransportHub/shuttlepassengers/approval";
                                }
                                else if (System.Environment.MachineName == "CALIFORNIUM")
                                {
                                    hostLink = "https://www.semirarampc.com:8443/TransportHub/shuttlepassengers/approval";
                                }
                                else if (System.Environment.MachineName == "ANDROMEDA")
                                {
                                    hostLink = "http://192.168.30.182/TransportHubCPC/approval";
                                }
                                else
                                {
                                    hostLink = "http://sodium2/TransportHub/shuttlepassengers/approval";
                                }
                                //End ADDED BY EBE during ECQ

                                //ebe 05072020 -> ECQ
                                string newContent = "THSR ID: " + _transactionId + " <br/> "  +
                                                "A vehicle request of " + _names + " going to " + _destination + " on " + e.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + " is pending for your approval. " ;
                                //ebe 05072020 -> ECQ

                                new ReportsController().SendEmail("", _Email, _senderEmail, newContent + " <br/> " + " Please click the link: " + "<br/> <a href='"+ hostLink + "'> TransportHub/Approval </a>", "Shuttle Service for Approval");
                            }
                            
                            //end sending message
                        }
                        else {
                            e.SmsId = smsCodesArr[e.OriginalApproverEmployeeNo];
                        }//end new if
                         //}

                    } //JPT additional code 09182024
                      //}); //JPT commented code 09182024
                    await _context.SaveChangesAsync();


                  


                    //_context.Entry(shuttlePassenger).State = EntityState.Detached;

                    _context.Database.ExecuteSqlCommand("execute AutoExpireAlternateHead");

                    


                    _otherReservation = "";
                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                    

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    if (returnMessage.Contains("instance"))
                    {
                        returnMessage = returnMessage + " Please check all the details.";
                    }
                    transaction.Rollback();
                    //throw e;
                }

            }

            var jsonData = new
            {
                otherReservation = _otherReservation,
                message = returnMessage,
                success = returnSuccess
            };
            return new JsonResult(jsonData);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ReservationForApprovalGroup()
        {

            //check if has over ride


            //var User = _userManager.Users.Where(r => r.Id == id).Select(u => new {
            //    EmployeeNumber = u.Employee.EmployeeNo,
            //    Domain = u.Domain,
            //    UserName = u.UserName,
            //    DisplayName = u.DisplayName,
            //    Company = u.Employee.CompanyList.CompanyName,
            //    UserId = u.Id,
            //}); ;


            //var _user = _context.Users.FirstOrDefault(x => x.Id == id);

            //var roles = await _userManager.GetRolesAsync(_user);

            //var checkroleIds = _context.Roles.Where(r => roles.Contains(r.Name)).Select(u => new {
            //    RoleIds = u.Id
            //});

            //end check if has over ride



            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();


            //end checking the role if overall approver


            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);
            
            var memberList = _context.Employees.Where(e => e.SupervisorEmployeeNo.Equals(HttpContext.Session.GetString("Session_employeeId"))).Select(r => r.EmployeeNo);
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));


            var _DataList = _context.ShuttlePassengers;

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var DataListTemp = _DataList.Where(u => u.Status == 1
                && _context.Employees
                    .Where(e => e.Id == u.EmployeeId)
                    .Select(e => e.CompanyGroupId)
                    .FirstOrDefault() == currentCompanyGroupId);


            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 1 && u.InitialApproverEmployeeNo == approverData.EmployeeNo);
            }

            

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Status == 1 ? "<a href='javascript:void(0);' class='btn btn-sm btn-primary'  " +
                                        "onClick='ViewEmployeeAjax(`" + u.TransactionId + "`," +
                                        "`" + u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName + "`," +
                                        "`" + u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + "`,`"+u.ChargingCompany.CompanyName+ "`,`" + u.ChargingDepartment.DepartmentName + "`);'>" +
                                        " View </a>" : "",
                TransactionId = u.TransactionId,
                NatureApplication = "SHUTTLE SERVICE",
                DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='"+ u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
            }).Distinct();
            //.GroupBy(x => new { x.TransactionId });


            //if (approverData.AlternateImmediateHeadValidity == false && approverData.IsImmediateHead == true) // if this is the immediate head with no alternate approver
            //{
            //    //DataList = DataList.Where(u => u.fieldStatus.ToString() == "1" && memberList.Contains(u.fieldEncodedBy.ToString()));
            //}

            //if (approverData.AlternateImmediateHeadValidity == false && approverData.IsImmediateHead == true) // if this is the alternate approver
            //{
            //    //DataList = DataList.Where(u => u.fieldStatus.ToString() == "1" && memberList.Contains(u.fieldEncodedBy.ToString()));
            //}



            // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = DataList.Count();

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
        public ActionResult ReservationForApprovalIndividual()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var memberList = _context.Employees.Where(e => e.SupervisorEmployeeNo.Equals(HttpContext.Session.GetString("Session_employeeId"))).Select(r => r.EmployeeNo);
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));


            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();


            //end checking the role if overall approver

            var _DataList = _context.ShuttlePassengers;

            var DataListTemp = _DataList.Where(u => u.Status == 1 && u.TransactionId == Request.Form["TransactionId"]);

            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 1 && u.InitialApproverEmployeeNo == approverData.EmployeeNo && u.TransactionId == Request.Form["TransactionId"]);
            }

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Id,
                TransactionId = u.TransactionId,
                NatureApplication = "SHUTTLE SERVICE",
                DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                FullName = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                CheckBox = "<div class='checkbox checkbox-success'><input id='checkselectIndividual" + u.Id + "' value='" + u.Id + "' type='checkbox' class='checkBoxIndividual'><label for='checkselectIndividual" + u.Id + "'></label></div>"
            });
            //.GroupBy(x => new { x.TransactionId });


            //if (approverData.AlternateImmediateHeadValidity == false && approverData.IsImmediateHead == true) // if this is the immediate head with no alternate approver
            //{
            //    //DataList = DataList.Where(u => u.fieldStatus.ToString() == "1" && memberList.Contains(u.fieldEncodedBy.ToString()));
            //}

            //if (approverData.AlternateImmediateHeadValidity == false && approverData.IsImmediateHead == true) // if this is the alternate approver
            //{
            //    //DataList = DataList.Where(u => u.fieldStatus.ToString() == "1" && memberList.Contains(u.fieldEncodedBy.ToString()));
            //}



            // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = DataList.Count();

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
        public async Task<ActionResult> ReservationApprovedGroup()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var memberList = _context.Employees.Where(e => e.SupervisorEmployeeNo.Equals(HttpContext.Session.GetString("Session_employeeId"))).Select(r => r.EmployeeNo);
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));

            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();


            //end checking the role if overall approver

            var _DataList = _context.ShuttlePassengers;

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var DataListTemp = _DataList.Where(u => u.Status == 2
                && _context.Employees
                    .Where(e => e.Id == u.EmployeeId)
                    .Select(e => e.CompanyGroupId)
                    .FirstOrDefault() == currentCompanyGroupId);

            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 2 && u.InitialApproverEmployeeNo == approverData.EmployeeNo);
            }

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Status == 2 ? "<a href='javascript:void(0);' class='btn btn-sm btn-primary'  " +
                                        "onClick='ViewEmployeeAjax(`" + u.TransactionId + "`," +
                                        "`" + u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName + "`," +
                                        "`" + u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + "`,`" + u.ChargingCompany.CompanyName + "`,`" + u.ChargingDepartment.DepartmentName + "`);'>" +
                                        " View </a>" : "",
                TransactionId = u.TransactionId,
                NatureApplication = "SHUTTLE SERVICE",
                DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null ||  u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='" + u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
            }).Distinct();



            // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = DataList.Count();

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
        public ActionResult ReservationApprovedIndividual()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var memberList = _context.Employees.Where(e => e.SupervisorEmployeeNo.Equals(HttpContext.Session.GetString("Session_employeeId"))).Select(r => r.EmployeeNo);
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));



            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();


            //end checking the role if overall approver

            var _DataList = _context.ShuttlePassengers;

            var DataListTemp = _DataList.Where(u => u.Status == 2 && u.TransactionId == Request.Form["TransactionId"]);

            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 2 && u.InitialApproverEmployeeNo == approverData.EmployeeNo && u.TransactionId == Request.Form["TransactionId"]);
            }

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Id,
                TransactionId = u.TransactionId,
                NatureApplication = "SHUTTLE SERVICE",
                DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                FullName = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                CheckBox = "<div class='checkbox checkbox-success'><input id='checkselectIndividual" + u.Id + "' value='" + u.Id + "' type='checkbox' class='checkBoxIndividual'><label for='checkselectIndividual" + u.Id + "'></label></div>"
            });


            // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = DataList.Count();

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






        // GET: ShuttlePassengers/Approval
        public async Task<IActionResult> Approval()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end

            return View();
        }
        public async Task<IActionResult> Approved()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> approveBypass(int timestamp)
        {

            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            
            if (roles <= 0)
            {
                throw new System.Exception("Access Denied.");
            }

            //end checking the role if overall approver

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();


            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 1);
            if (parameterTemp.Year < DateTime.Now.Year)
            {
                parameterTemp.Value = 1;
                parameterTemp.Year = DateTime.Now.Year;
                _context.Update(parameterTemp);
                _context.SaveChanges();
            }



            //End Manage Parameters


            var TransactionIds = "";
            var returnSuccess = 1;
            var returnMessage = "";
            int approveCounter = 0;

            var _transactionId = "";
            var _otherReservation = "";

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    // SUBMIT

                     var _shuttlePassenger = _context.ShuttlePassengers.Where(s => s.ServiceDateTimeStamp == timestamp && s.Status == 6 && s.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))));

                    if (_shuttlePassenger.Count() <= 0) {
                        throw new System.Exception("There is no reservation to submit.");
                    }
                    var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 1);
                    _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                    parameter.Value = parameter.Value + 1;
                    _context.Update(parameter);
                    _context.SaveChanges();

                    await _shuttlePassenger.ForEachAsync(e => {

                        e.Status = 1;
                        e.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        e.ModifyDate = DateTime.Now;
                        e.SubmitDate = DateTime.Now;
                        e.TransactionId = _transactionId;
                        e.SmsId = "";
                        

                    });
                    await _context.SaveChangesAsync();



                    // END SUBMIT






                    TransactionIds = _transactionId;
                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.TransactionId == reference && s.Status == 1 && s.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))));

                        if (shuttlePassenger.Count() > 0)
                        {
                            await shuttlePassenger.ForEachAsync(e => {
                                e.Status = 2;
                                e.ApprovedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.SubmitDate = DateTime.Now;
                                e.ApprovedDatetime = DateTime.Now;
                            });
                            await _context.SaveChangesAsync();
                            approveCounter++;
                        }

                        returnMessage = "Submitted & Approved (Bypass)";
                        _otherReservation = " & Approved (Bypass)";
                        //logs
                        Log _log = new Log();
                        _log.Process = returnMessage + " Reservation Trans No.: " + reference;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);
                        //end logs

                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to approve.");
                    }





                    _context.Database.ExecuteSqlCommand("execute AutoExpireAlternateHead");

                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    if (returnMessage.Contains("instance"))
                    {
                        returnMessage = returnMessage + " Please check all the details.";
                    }
                    transaction.Rollback();
                    //throw e;
                }

            }

            var jsonData = new
            {
                otherReservation = _otherReservation,
                message = returnMessage,
                success = returnSuccess
            };
            return new JsonResult(jsonData);
        }
        [HttpPost]
        public async Task<IActionResult> approveByGroup(int _method)
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));
            var VarAlternateImmediate = approverData.AlternateImmediateHeadValidity == true ? approverData.AlternateImmediateHead.ToString() : approverData.EmployeeNo.ToString();
            int approveCounter = 0;

            //checking the role if overall approver EBE 05052020 ECQ

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            var AllowApproval = 0;
            if (roles > 0)
            {
                AllowApproval = 1;
            }

            //end checking the role if overall approver EBE 05052020 ECQ

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.TransactionId == reference && s.Status == 1 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                        if (shuttlePassenger.Count() > 0)
                        {
                            //await shuttlePassenger.ForEachAsync(e => { //JPT additional code 09062024
                            foreach (var e in shuttlePassenger) { //JPT additional code 09062024
                                e.Status = _method;
                                e.ApprovedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.ApprovedDatetime = DateTime.Now;


                                //sending message to requestor
                                string _MobileNumber = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else
                                {
                                    _MobileNumber = e.ContactNo;
                                }

                                //manual SMS to Joyce Pumares

                                _MobileNumber = _MobileNumber + "," + _context.Employees.FirstOrDefault(ags => ags.EmployeeNo.Equals("9669892")).MobileNumber.ToString();

                                //end

                                string _message = "";
                                if (_method == 2) { 
                                    _message = "Your request with THSR ID " + e.TransactionId + " is now approved and is now queued for assigning of vehicle.";
                                }
                                else 
                                {
                                    _message = "Your request with THSR ID " + e.TransactionId + " has been disapproved. Please contact your supervisor for confirmation.";
                                }


                                //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                                //JPT additional code 09062024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _message;
                                string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09062024

                                //new Email notif EBE - ECQ - 05072020


                                var _Email = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                }

                                //var _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                if (_Email != "")
                                {
                                    var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                    new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Shuttle Service");
                                }
                                //end new email Notif EBE - ECQ - 05072020

                                //end
                            } //JPT additional code 09062024
                            //}); //JPT commented code 09062024
                            await _context.SaveChangesAsync();
                            approveCounter++;
                        }

                        returnMessage = _method == 2 ? "Approved" : "Disapproved";
                        //logs
                        Log _log = new Log();
                        _log.Process = returnMessage + " Reservation Trans No.: " + reference;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);
                        //end logs

                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to approve.");
                    }

                    

               


                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    if (returnMessage.Contains("instance"))
                    {
                        returnMessage = returnMessage + " Please check all the details.";
                    }
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



        [HttpPost]
        public async Task<IActionResult> approveByIndividual(int _method)
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));
            var VarAlternateImmediate = approverData.AlternateImmediateHeadValidity == true ? approverData.AlternateImmediateHead.ToString() : approverData.EmployeeNo.ToString();
            int approveCounter = 0;

            //checking the role if overall approver EBE 05052020 ECQ

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            var AllowApproval = 0;
            if (roles > 0)
            {
                AllowApproval = 1;
            }

            //end checking the role if overall approver EBE 05052020 ECQ

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.Id.ToString() == reference  && s.Status == 1 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                        if (shuttlePassenger.Count() > 0)
                        {
                            //await shuttlePassenger.ForEachAsync(e => { //JPT commented code 09062024
                            foreach (var e in shuttlePassenger) { //JPT additional code 09062024
                                e.Status = _method;
                                e.ApprovedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.ApprovedDatetime = DateTime.Now;

                                //sending message to requestor
                                string _MobileNumber = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else
                                {
                                    _MobileNumber = e.ContactNo;
                                }

                                //manual SMS to Joyce Pumares

                                _MobileNumber = _MobileNumber + "," + _context.Employees.FirstOrDefault(ags => ags.EmployeeNo.Equals("9669892")).MobileNumber.ToString();

                                //end

                                string _message = "";
                                if (_method == 2)
                                {
                                    _message = "Your request with THSR ID " + e.TransactionId + " is now approved and is now queued for assigning of vehicle.";
                                }
                                else
                                {
                                    _message = "Your request with THSR ID " + e.TransactionId + " has been disapproved. Please contact your supervisor for confirmation.";
                                }

                                //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                                //JPT additional code 09062024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _message;
                                string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09062024

                                //new Email notif EBE - ECQ - 05072020

                                //var _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;

                                var _Email = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                }

                                if (_Email != "")
                                {
                                    var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                    new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Shuttle Service");
                                }
                                //end new email Notif EBE - ECQ - 05072020

                                //end

                            } //JPT additional code 09062024
                            //}); //JPT commented code 09062024
                            await _context.SaveChangesAsync();
                            approveCounter++;
                        }

                        returnMessage = _method == 2 ? "Approved" : "Disapproved";
                        //logs
                        Log _log = new Log();
                        _log.Process = returnMessage + " Reservation ID No.: " + reference;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);
                        //end logs

                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to approve.");
                    }


                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    if (returnMessage.Contains("instance"))
                    {
                        returnMessage = returnMessage + " Please check all the details.";
                    }
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


        [HttpPost]
        public async Task<IActionResult> cancelByGroup()
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));
            var VarAlternateImmediate = approverData.AlternateImmediateHeadValidity == true ? approverData.AlternateImmediateHead.ToString() : approverData.EmployeeNo.ToString();
            int approveCounter = 0;

            //checking the role if overall approver EBE 05052020 ECQ

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            var AllowApproval = 0;
            if (roles > 0)
            {
                AllowApproval = 1;
            }

            //end checking the role if overall approver EBE 05052020 ECQ

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.TransactionId == reference && s.Status == 2 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                        if (shuttlePassenger.Count() > 0)
                        {
                            //await shuttlePassenger.ForEachAsync(e => { //JPT commented code 09062024
                            foreach (var e in shuttlePassenger) { //JPT additional code 09062024
                                e.Status = 5;
                                e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.CancelReason = Request.Form["CancelReason"].ToString();
                                e.CancelledDatetime = DateTime.Now;

                                //sending message to requestor
                                string _MobileNumber = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else
                                {
                                    _MobileNumber = e.ContactNo;
                                }
                                string _message = "Your request with THSR ID " + e.TransactionId + " has been declined/cancelled." + Environment.NewLine + "Remarks: " + Request.Form["CancelReason"].ToString();


                                //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024
                                //end

                                //JPT additional code 09062024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _message;
                                string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09062024

                                // recalculate the trip capacity
                                if (e.ShuttleId > 0)
                                    {
                                        var _trip = _context.Trip.FirstOrDefault(t => t.Id.Equals(e.ShuttleId));
                                        int capacityAM = _trip.RemainingCapacity;
                                        int capacityPM = _trip.RemainingCapacityPM;

                                        if (e.TripTypeId == 1 || e.TripTypeId == 2)
                                        {
                                            capacityAM++;
                                        }
                                        if(e.TripTypeId == 1 || e.TripTypeId == 3)
                                        { 
                                            capacityPM++;
                                        }

                                        _trip.RemainingCapacity = capacityAM;
                                        _trip.RemainingCapacityPM = capacityPM;
                                        _context.Update(_trip);
                                        _context.SaveChanges();
                                    }
                                // end

                                e.ShuttleId = 0;
                            } //JPT additional code 09062024
                            //}); //JPT commented code 09062024
                            await _context.SaveChangesAsync();
                            approveCounter++;
                        }

                        returnMessage =  "Cancelled" ;

                        //logs
                        Log _log = new Log();
                        _log.Process = returnMessage + " Reservation Trans No.: " + reference;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);
                        //end logs

                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to cancel.");
                    }






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



        [HttpPost]
        public async Task<IActionResult> cancelByIndividual()
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));
            var VarAlternateImmediate = approverData.AlternateImmediateHeadValidity == true ? approverData.AlternateImmediateHead.ToString() : approverData.EmployeeNo.ToString();
            int approveCounter = 0;

            //checking the role if overall approver EBE 05052020 ECQ

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            var AllowApproval = 0;
            if (roles > 0)
            {
                AllowApproval = 1;
            }

            //end checking the role if overall approver EBE 05052020 ECQ

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.Id.ToString() == reference && s.Status == 2 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                        if (shuttlePassenger.Count() > 0)
                        {
                            //await shuttlePassenger.ForEachAsync(e => { //JPT commented code 09062024
                            foreach (var e in shuttlePassenger){ //JPT additional code 09062024
                                e.Status = 5;
                                e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.CancelReason = Request.Form["CancelReason"].ToString();
                                e.CancelledDatetime = DateTime.Now;


                                //sending message to requestor
                                string _MobileNumber = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else
                                {
                                    _MobileNumber = e.ContactNo;
                                }
                                string _message = "Your request with THSR ID " + e.TransactionId + " has been declined/cancelled." + Environment.NewLine + "Remarks: " + Request.Form["CancelReason"].ToString();


                                //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024
                                //end

                                //JPT additional code 09062024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _message;
                                string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09062024

                                // recalculate the trip capacity
                                if (e.ShuttleId > 0)
                                {
                                    var _trip = _context.Trip.FirstOrDefault(t => t.Id.Equals(e.ShuttleId));
                                    int capacityAM = _trip.RemainingCapacity;
                                    int capacityPM = _trip.RemainingCapacityPM;

                                    if (e.TripTypeId == 1 || e.TripTypeId == 2)
                                    {
                                        capacityAM++;
                                    }
                                    if (e.TripTypeId == 1 || e.TripTypeId == 3)
                                    {
                                        capacityPM++;
                                    }

                                    _trip.RemainingCapacity = capacityAM;
                                    _trip.RemainingCapacityPM = capacityPM;
                                    _context.Update(_trip);
                                    _context.SaveChanges();
                                }
                                // end

                                e.ShuttleId = 0;

                            } //JPT additional code 09062024
                            //}); //JPT commented code 09062024
                            await _context.SaveChangesAsync();
                            approveCounter++;
                        }

                        returnMessage = "Cancelled";
                        //logs
                        Log _log = new Log();
                        _log.Process = returnMessage + " Reservation ID No.: " + reference;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);
                        //end logs

                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to cancel.");
                    }


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




        [HttpPost]
        public async Task<IActionResult> DeclineCancel(int id)
        {
            //cancel = 0

            var returnSuccess = 1;
            var returnMessage = "";
            var smsTitle = "";
            var NewStatus = 0;
            var _Destination = "";


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    if (Request.Form["method"].ToString() == "1")
                    {
                        NewStatus = 5;
                        smsTitle = "Cancelled";
                    }
                    else if (Request.Form["method"].ToString() == "2")
                    {
                        NewStatus = 7;
                        smsTitle = "Declined";
                    }
                    else
                    {
                        throw new System.Exception("Invalid method.");
                    }



                    var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.Id == id);

                    if (shuttlePassenger.Count() > 0)
                    {
                        //await shuttlePassenger.ForEachAsync(e =>  //JPT commented code 09062024
                        foreach (var e in shuttlePassenger)  //JPT additional code 09062024
                        {
                            //destination

                            if(e.TripTypeId == 1)
                            {
                                _Destination = "Round Trip (Makati HO - Calaca)";
                            }
                            if (e.TripTypeId == 1)
                            {
                                _Destination = "Calaca";
                            }
                            if (e.TripTypeId == 1)
                            {
                                _Destination = "Makati HO";
                            }

                            //end

                            e.ShuttleId = 0;
                            e.Status = NewStatus;
                            if (NewStatus == 5)
                            {// cancelled
                                e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.CancelReason = Request.Form["CancelDeclineReason"].ToString();
                                e.CancelledDatetime = DateTime.Now;
                            }
                            else if (NewStatus == 7)
                            {// declined
                                e.DeclinedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.DeclinedReason = Request.Form["CancelDeclineReason"].ToString();
                                e.DeclinedDatetime = DateTime.Now;
                            }
                            else
                            {
                                throw new System.Exception("Invalid Method(2)");
                            }


                            //sending message to requestor  05122020 ECQ
                            string _MobileNumber = "";
                            if (e.PassengerTypeId == 1)
                            {
                                _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                            }
                            else
                            {
                                _MobileNumber = e.ContactNo;
                            }

                            string _message = "Your trip request with THSR ID " + e.TransactionId + " going to " + _Destination + " has been " + smsTitle + " by AGS." + Environment.NewLine + "Remarks: " + Request.Form["CancelDeclineReason"].ToString();


                            //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                            //JPT additional code 09062024
                            string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                            string message = _message;
                            string referenceNo = e.TransactionId;  // Replace with your transaction ID
                            string systemName = "THS Makati";

                            // Call the SendSmsAsync method
                            string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                            //JPT end additional code 09062024

                            //new Email notif EBE - ECQ - 05072020

                            var _Email = "";
                            if (e.PassengerTypeId == 1)
                            {
                                _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                            }

                            if (_Email != "")
                            {
                                var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Shuttle Reservation");
                            }
                            //end new email Notif EBE - ECQ - 05072020

                            //end EBE 05122020 ECQ
                        };  //JPT additional code 09062024
                        //});  //JPT commented code 09062024
                        await _context.SaveChangesAsync();
                    }



                    await _context.SaveChangesAsync();


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



        ////// end ////
    }
}
