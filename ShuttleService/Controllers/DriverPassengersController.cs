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
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAllRole")]
    public class DriverPassengersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DriverPassengersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DriverPassengers
        public async Task<IActionResult> Index()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end 


            _context.Database.ExecuteSqlCommand("execute UpdateApprovedDriverPassengerHeaders");


            //var applicationDbContext = _context.DriverPassengers.Include(d => d.ChargingCompany).Include(d => d.ChargingDepartment).Include(d => d.CompanyList).Include(d => d.Employee).Include(d => d.PassengerType);
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);

            var applicationDbContext = _context.DriverPassengers
                .Include(d => d.ChargingCompany)
                .Include(d => d.ChargingDepartment)
                .Include(d => d.CompanyList)
                .Include(d => d.Employee)
                .Include(d => d.PassengerType)
                .Where(d => d.ServiceDate >= startOfYear);

            return View(await applicationDbContext.ToListAsync());
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


        // GET: DriverPassengers/Details/5
        public async Task<IActionResult> Details()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end

            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            ViewBag.unixtime = unixTime * 1000;

            ViewBag.t = HttpContext.Request.Query["t"].ToString();


            //var getallHours = _context.Database.ExecuteSqlCommand("GetAllHours '2020-01-22'");

            //ViewData["allHours"] = getallHours;

            String today = DateTime.Today.ToString();

            return View();
        }

        // GET: DriverPassengers/Create
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

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var ChargingCompanysList = _context.ChargingCompanys
                .Where(m => m.CompanyGroupId == currentCompanyGroupId)
                .OrderBy(m => m.CompanyName).ToList();
            ViewData["ChargingCompanyId"] = new SelectList(ChargingCompanysList, "Id", "CompanyName");

            var ChargingDepartmentsList = _context.ChargingDepartments
                .Where(m => m.CompanyGroupId == currentCompanyGroupId)
                .OrderBy(m => m.DepartmentName).ToList();
            ViewData["ChargingDepartmentId"] = new SelectList(ChargingDepartmentsList, "Id", "DepartmentName");

            var CompanyListsList = _context.CompanyLists.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.CompanyName).ToList();
            ViewData["CompanyId"] = new SelectList(CompanyListsList, "Id", "CompanyName");

            var OriginDestinationList = _context.OriginDestination.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.OriginDestinationName).ToList();
            ViewData["OriginDestinationId"] = new SelectList(OriginDestinationList, "Id", "OriginDestinationName");

            var ServiceTypeList = _context.ServiceTypes.OrderBy(m => m.Description).ToList();
            ViewData["ServiceTypeId"] = new SelectList(ServiceTypeList, "Id", "Description");

            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dGet = unixTime;
            ViewBag.tGet = Convert.ToInt64(HttpContext.Request.Query["t"]);
            ViewBag.tempid = HttpContext.Request.Query["tempid"];
            
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            var _t = Convert.ToInt64(HttpContext.Request.Query["t"]);
            var _driverPass = _context.DriverPassengerHeaders.FirstOrDefault(e=> e.Id.Equals(Convert.ToInt32(_t)) );
            ViewData["DriverPassengerHeaders"] = _driverPass;

            
            ViewBag.EmployeeId = Convert.ToInt64(HttpContext.Session.GetString("Session_employeeId"));

            //for default


            var NationalityList = _context.Nationalities.OrderBy(m => m.NationalityName).ToList();
            ViewData["Nationality"] = new SelectList(NationalityList, "NationalityName", "NationalityName");

            var employee = _context.Employees.FirstOrDefault(e => e.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))));

            ViewBag.defaultChargingCompanyId = employee.ChargingCompanyId;
            ViewBag.defaultChargingDepartmentId = employee.ChargingDepartmentId;
            ViewBag.defaultChargingDepartmentName = _context.ChargingDepartments.FirstOrDefault(e => e.Id.Equals(employee.ChargingDepartmentId)).DepartmentName;
            //end for default


            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            ViewBag.OverAllApprover = roles;

            ViewBag.FromTrip = HttpContext.Request.Query["ft"].ToString() ; 

            //end checking the role if overall approver



            return View();
        }

        //// POST: DriverPassengers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ServiceDateTimeStamp,TransactionId,ServiceDate,ShuttleId,PassengerTypeId,EmployeeNo,EmployeeId,CompanyListId,CompanyOther,ChargingCompanyId,ChargingDepartmentId,Name,FirstName,MiddleName,LastName,Position,Purpose,Remarks,TripTypeId,TripTimeFrom,TripTimeTo,Origin,Destination,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate,SubmitDate,ContactNo,Status,InitialApproverEmployeeNo,ApprovedBy,ApprovedDatetime")] DriverPassenger driverPassenger)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(driverPassenger);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ChargingCompanyId"] = new SelectList(_context.ChargingCompanys, "Id", "CompanyName", driverPassenger.ChargingCompanyId);
        //    ViewData["ChargingDepartmentId"] = new SelectList(_context.ChargingDepartments, "Id", "DepartmentName", driverPassenger.ChargingDepartmentId);
        //    ViewData["CompanyListId"] = new SelectList(_context.CompanyLists, "Id", "CompanyName", driverPassenger.CompanyListId);
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "AlternativeEmail", driverPassenger.EmployeeId);
        //    ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", driverPassenger.PassengerTypeId);
        //    ViewData["TripTypeId"] = new SelectList(_context.TripTypes, "Id", "Description", driverPassenger.TripTypeId);
        //    return View(driverPassenger);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(            
            DriverPassenger driverPassenger)
        {
            var returnSuccess = 1;
            var returnMessage = "";
            var _temptransactionId = "";
            var checkHeaderIfExist = 0;
            var _tid = "";
            var _tempid = "";
            var _d = HttpContext.Request.Query["d"].ToString();

            var driverPassengerHead = new DriverPassengerHeader();

            int[] NonDuplicateStatus = { 1, 2, 4, 6 };


            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(driverPassenger.ServiceDateTimeStamp).ToLocalTime();

            //commented as requested by AGS 05062020 EBE during ECQ
            //if (dtDateTime < DateTime.Now)
            //{
            //    throw new System.Exception("Invalid reservation date.");
            //}

            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 4);
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

                    var timefrom = driverPassenger.TripTimeFrom;
                    var timeto = driverPassenger.TripTimeTo;
                    long unixTime = Convert.ToInt64(_d);

                    var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("yyyy-MM-dd");
                    string _timestampfrom = dtimestamp + " " + timefrom + ":00";
                    string _timestampto = dtimestamp + " " + timeto + ":00";

                    DateTime timestampfrom = DateTime.Parse(_timestampfrom, System.Globalization.CultureInfo.CurrentCulture);
                    DateTime timestampto = DateTime.Parse(_timestampto, System.Globalization.CultureInfo.CurrentCulture);

                    if (timestampfrom >= timestampto) {
                        throw new System.Exception("Invalid reservation time."); //if datetime is invalid
                    }

                    //setting the other company
                    if (driverPassenger.CompanyListId > 0)
                    {
                        //wala lang.
                    }
                    else
                    {
                        driverPassenger.CompanyListId = 1;
                    } //end setting the other company


                    if (driverPassenger.PassengerTypeId == 1)
                    {

                        //checking if the reservation of the employee is duplicated

                        if (_context.Employees.FirstOrDefault(emp => emp.EmployeeNo == driverPassenger.EmployeeNo) == null) {

                            throw new System.Exception("Employee number does not exist to the system."); //if employee number is not exist
                        }

                        var checkDriverPassenger = _context.DriverPassengers.Where(e => NonDuplicateStatus.Contains(e.Status) && e.EmployeeNo == driverPassenger.EmployeeNo && e.ServiceDateTimeStamp.ToString().Contains(Request.Form["ServiceDateTimeStamp"])).Select(u => new
                        {
                            _TripTimeFrom = DateTime.Parse(dtimestamp + " " + u.TripTimeFrom + ":00", System.Globalization.CultureInfo.CurrentCulture),
                            _TripTimeTo = DateTime.Parse(dtimestamp + " " + u.TripTimeTo + ":00", System.Globalization.CultureInfo.CurrentCulture),
                            u.Id,
                            _empid = u.EmployeeNo,
                            _transid = u.TransactionId,
                            _tempid = u.DriverPassengerHeader.TempTransactionId,
                            _dhead = u.DriverPassengerHeader
                        });
                        var filterResult = checkDriverPassenger.Where(e=>
                         (e._TripTimeFrom >= timestampfrom && e._TripTimeFrom <= timestampto) ||
                         (e._TripTimeTo >= timestampfrom && e._TripTimeFrom <= timestampto) ||
                         (e._TripTimeFrom <= timestampfrom && e._TripTimeTo >= timestampto) );

                        if (filterResult.Count() > 0)
                        {
                            throw new System.Exception("Passenger already booked on this date/time."); //if already have a reservation

                        } 


                        //    if (_context.DriverPassengers.Count(e =>
                        // NonDuplicateStatus.Contains(e.Status) &&
                        // (DateTime.Parse(dtimestamp + " " +e.TripTimeFrom + ":00", System.Globalization.CultureInfo.CurrentCulture) >= timestampfrom && DateTime.Parse(dtimestamp + " " + e.TripTimeFrom + ":00", System.Globalization.CultureInfo.CurrentCulture) <= timestampto) ||
                        // (DateTime.Parse(dtimestamp + " " + e.TripTimeTo + ":00", System.Globalization.CultureInfo.CurrentCulture) >= timestampfrom && DateTime.Parse(dtimestamp + " " + e.TripTimeTo + ":00", System.Globalization.CultureInfo.CurrentCulture) <= timestampto) ||
                        // (DateTime.Parse(dtimestamp + " " + e.TripTimeFrom + ":00", System.Globalization.CultureInfo.CurrentCulture) <= timestampfrom && DateTime.Parse(dtimestamp + " " + e.TripTimeTo + ":00", System.Globalization.CultureInfo.CurrentCulture) >= timestampto) &&
                        //  e.EmployeeNo == driverPassenger.EmployeeNo) > 0)
                        //{
                        //    throw new System.Exception("Passenger already booked on this date/time."); //if already have a reservation

                        //}


                        var checkApprover = _context.Employees.FirstOrDefault(e => e.EmployeeNo == driverPassenger.EmployeeNo);
                        driverPassenger.InitialApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                        driverPassenger.OriginalApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;

                        driverPassenger.ContactNo = checkApprover.LocalNumber + " / " + checkApprover.MobileNumber;
                    }
                    else
                    {
                        
                        driverPassenger.EmployeeNo = "";
                        var checkApprover = _context.Employees.FirstOrDefault(e => e.Id.ToString() == HttpContext.Session.GetString("Session_employeeId"));
                        driverPassenger.InitialApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                        driverPassenger.OriginalApproverEmployeeNo = checkApprover.SupervisorEmployeeNo;
                    }

                    //VALIDATION END

                    if (HttpContext.Request.Query["tid"].ToString() == "0")
                    {
                        checkHeaderIfExist = 0;
                    }
                    else {
                        checkHeaderIfExist = Convert.ToInt32(_context.DriverPassengerHeaders.Where(e => e.TempTransactionId.Equals(HttpContext.Request.Query["TempTransactionId"])).Count());
                    }

                    if (checkHeaderIfExist <= 0)
                    {
                        //Manage Parameters
                        var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 4);
                        _temptransactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                        parameter.Value = parameter.Value + 1;
                        _context.Update(parameter);
                        _context.SaveChanges();
                        //End Manage Parameters

                        driverPassengerHead.EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        driverPassengerHead.ServiceDate = dtDateTime;
                        driverPassengerHead.EncodeDate = DateTime.Now;
                        driverPassengerHead.ModifyDate = DateTime.Now;
                        driverPassengerHead.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"));
                        driverPassengerHead.Status = 6;
                        driverPassengerHead.Remarks = "";
                        driverPassengerHead.Purpose = driverPassenger.Purpose;
                        driverPassengerHead.UnplannedTrip = driverPassenger.UnplannedTrip;
                        driverPassengerHead.Instructions = driverPassenger.Instructions;
                        driverPassengerHead.ServiceTypeId = driverPassenger.ServiceTypeId;
                        driverPassengerHead.ServiceDateTimeStamp = driverPassenger.ServiceDateTimeStamp;
                        driverPassengerHead.Origin = driverPassenger.Origin;
                        driverPassengerHead.Destination = driverPassenger.Destination;
                        driverPassengerHead.OriginId = driverPassenger.OriginId;
                        driverPassengerHead.DestinationId = driverPassenger.DestinationId;
                        driverPassengerHead.TripTimeFrom = driverPassenger.TripTimeFrom;
                        driverPassengerHead.TripTimeTo = driverPassenger.TripTimeTo;
                        driverPassengerHead.TempTransactionId = _temptransactionId;


                        _context.Add(driverPassengerHead);
                        _context.SaveChanges();
                    }
                    else {

                        var driverPassengerHeadTemp = _context.DriverPassengerHeaders.Where(e => e.TempTransactionId.Equals(HttpContext.Request.Query["TempTransactionId"])).FirstOrDefault();
                        driverPassengerHeadTemp.Instructions = driverPassenger.Instructions;
                        driverPassengerHeadTemp.Purpose = driverPassenger.Purpose;
                        driverPassengerHeadTemp.UnplannedTrip = driverPassenger.UnplannedTrip;
                        driverPassengerHeadTemp.ServiceTypeId = driverPassenger.ServiceTypeId;
                        driverPassengerHeadTemp.Origin = driverPassenger.Origin;
                        driverPassengerHeadTemp.Destination = driverPassenger.Destination;
                        driverPassengerHeadTemp.OriginId = driverPassenger.OriginId;
                        driverPassengerHeadTemp.DestinationId = driverPassenger.DestinationId;
                        driverPassengerHeadTemp.TripTimeFrom = driverPassenger.TripTimeFrom;
                        driverPassengerHeadTemp.TripTimeTo = driverPassenger.TripTimeTo;
                        driverPassengerHeadTemp.ServiceTypeId = driverPassenger.ServiceTypeId;

                        //checking all the passengers if has duplicate


                        //var checkDriverPassengerPass = _context.DriverPassengers.Where(e => NonDuplicateStatus.Contains(e.Status) && e.PassengerType.Id.Equals(1) && e.ServiceDateTimeStamp.ToString().Contains(Request.Form["ServiceDateTimeStamp"])).Select(u => new
                        //{
                        //    _TripTimeFrom = DateTime.Parse(dtimestamp + " " + u.TripTimeFrom + ":00", System.Globalization.CultureInfo.CurrentCulture),
                        //    _TripTimeTo = DateTime.Parse(dtimestamp + " " + u.TripTimeTo + ":00", System.Globalization.CultureInfo.CurrentCulture),
                        //    u.Id,
                        //    _empid = u.EmployeeNo,
                        //    _transid = u.TransactionId,
                        //    _tempid = u.DriverPassengerHeader.TempTransactionId,
                        //    _dhead = u.DriverPassengerHeader
                        //});

                        //var filterResultPass = checkDriverPassengerPass.Where(e =>
                        // (e._TripTimeFrom >= timestampfrom && e._TripTimeFrom <= timestampto) ||
                        // (e._TripTimeTo >= timestampfrom && e._TripTimeFrom <= timestampto) ||
                        // (e._TripTimeFrom <= timestampfrom && e._TripTimeTo >= timestampto));

                        //if (filterResultPass.Count() > 0)
                        //{
                        //    throw new System.Exception("There are duplicated passenger(s) already booked on this date/time."); //if already have a reservation

                        //}

                        //end checking



                        _context.Update(driverPassengerHeadTemp);
                        _context.SaveChanges();

                        _temptransactionId = driverPassengerHeadTemp.TempTransactionId;
                    }

                    var driverPassengerHeadData = _context.DriverPassengerHeaders.Where(e => e.TempTransactionId.Equals(_temptransactionId)).FirstOrDefault();
                    _tid = driverPassengerHeadData.Id.ToString();
                    _tempid = driverPassengerHeadData.TempTransactionId.ToString();

                    //foreach (var passenger in driverPassengerHeadData.DriverPassengers)
                    //{
                        var uPassenger = new DriverPassenger
                        {
                            EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName"),
                            ServiceDate = dtDateTime,
                            EncodeDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")),
                            Status = 6,
                            Remarks = "",
                            DriverPassengerHeaderId = driverPassengerHeadData.Id.ToString(),
                            ServiceTypeId = driverPassenger.ServiceTypeId,
                            ServiceDateTimeStamp = driverPassenger.ServiceDateTimeStamp,
                            Origin = driverPassenger.Origin,
                            Destination = driverPassenger.Destination,
                            OriginId = driverPassenger.OriginId,
                            DestinationId = driverPassenger.DestinationId,
                            TripTimeFrom = driverPassenger.TripTimeFrom,
                            TripTimeTo = driverPassenger.TripTimeTo,
                            PassengerTypeId = driverPassenger.PassengerTypeId,
                            CompanyListId = driverPassenger.CompanyListId,
                            CompanyOther = driverPassenger.CompanyOther,
                            ChargingCompanyId = driverPassenger.ChargingCompanyId,
                            ChargingDepartmentId = driverPassenger.ChargingDepartmentId,
                            Name = driverPassenger.Name,
                            Position = driverPassenger.Position,
                            Instructions = driverPassenger.Instructions,
                            Purpose = driverPassenger.Purpose,
                            FirstName = driverPassenger.FirstName,
                            MiddleName = driverPassenger.MiddleName,
                            LastName = driverPassenger.LastName,
                            InitialApproverEmployeeNo = driverPassenger.InitialApproverEmployeeNo,
                            EmployeeNo = driverPassenger.EmployeeNo,
                            Gender = driverPassenger.Gender,
                            ContactNo = driverPassenger.ContactNo,
                            Nationality = driverPassenger.Nationality,
                            OriginalApproverEmployeeNo = driverPassenger.OriginalApproverEmployeeNo,
                            UnplannedTrip = driverPassenger.UnplannedTrip
                };
                        _context.Entry(uPassenger).State = EntityState.Added;

                    //}




                    var driverPassenger_ = _context.DriverPassengers.Where(e => e.DriverPassengerHeaderId.Equals(driverPassengerHeadData.Id.ToString()));
                    await driverPassenger_.ForEachAsync(e =>
                    {
                        e.Instructions = driverPassenger.Instructions;
                        e.Purpose = driverPassenger.Purpose;
                        e.UnplannedTrip = driverPassenger.UnplannedTrip;
                        driverPassengerHead.ServiceTypeId = driverPassenger.ServiceTypeId;
                        e.Origin = driverPassenger.Origin;
                        e.Destination = driverPassenger.Destination;
                        e.OriginId = driverPassenger.OriginId;
                        e.DestinationId = driverPassenger.DestinationId;
                        e.TripTimeFrom = driverPassenger.TripTimeFrom;
                        e.TripTimeTo = driverPassenger.TripTimeTo;
                        e.ServiceTypeId = driverPassenger.ServiceTypeId;
                    });

                    //await _context.SaveChangesAsync();




                    await _context.SaveChangesAsync();






                    //logs
                    Log _log = new Log();
                    _log.Process = "Add Driver/Service Reservation For " + driverPassenger.FirstName + " " + driverPassenger.LastName + " / Time Stamp: " + driverPassenger.ServiceDateTimeStamp;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

                    //_context.Add(driverPassenger);
                    var result = await _context.SaveChangesAsync();


                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message + " / " + e.InnerException;
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
                success = returnSuccess,
                tid = _tid,
                tempid = _tempid,
                d = _d
            };
            return new JsonResult(jsonData);
        }


        // GET: DriverPassengers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driverPassenger =  _context.DriverPassengers.FirstOrDefault(e=> e.TransactionId.Equals(id));
            if (driverPassenger == null)
            {
                return NotFound();
            }
            ViewData["ChargingCompanyId"] = new SelectList(_context.ChargingCompanys, "Id", "CompanyName", driverPassenger.ChargingCompanyId);
            ViewData["ChargingDepartmentId"] = new SelectList(_context.ChargingDepartments, "Id", "DepartmentName", driverPassenger.ChargingDepartmentId);
            ViewData["CompanyListId"] = new SelectList(_context.CompanyLists, "Id", "CompanyName", driverPassenger.CompanyListId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "AlternativeEmail", driverPassenger.EmployeeId);
            ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", driverPassenger.PassengerTypeId);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypes, "Id", "Description", driverPassenger.ServiceTypeId);
            return View("Create",driverPassenger);
        }

        // POST: DriverPassengers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceDateTimeStamp,TransactionId,ServiceDate,ShuttleId,PassengerTypeId,EmployeeNo,EmployeeId,CompanyListId,CompanyOther,ChargingCompanyId,ChargingDepartmentId,Name,FirstName,MiddleName,LastName,Position,Purpose,Remarks,ServiceTypeId,TripTimeFrom,TripTimeTo,Origin,Destination,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate,SubmitDate,ContactNo,Status,InitialApproverEmployeeNo,ApprovedBy,ApprovedDatetime")] DriverPassenger driverPassenger)
        {
            if (id != driverPassenger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driverPassenger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverPassengerExists(driverPassenger.Id))
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
            ViewData["ChargingCompanyId"] = new SelectList(_context.ChargingCompanys, "Id", "CompanyName", driverPassenger.ChargingCompanyId);
            ViewData["ChargingDepartmentId"] = new SelectList(_context.ChargingDepartments, "Id", "DepartmentName", driverPassenger.ChargingDepartmentId);
            ViewData["CompanyListId"] = new SelectList(_context.CompanyLists, "Id", "CompanyName", driverPassenger.CompanyListId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "AlternativeEmail", driverPassenger.EmployeeId);
            ViewData["PassengerTypeId"] = new SelectList(_context.PassengerTypes, "Id", "Description", driverPassenger.PassengerTypeId);
            ViewData["ServiceTypeId"] = new SelectList(_context.ServiceTypes, "Id", "Description", driverPassenger.ServiceTypeId);
            return View(driverPassenger);
        }

        // GET: DriverPassengers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driverPassenger = await _context.DriverPassengers
                .Include(d => d.ChargingCompany)
                .Include(d => d.ChargingDepartment)
                .Include(d => d.CompanyList)
                .Include(d => d.Employee)
                .Include(d => d.PassengerType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driverPassenger == null)
            {
                return NotFound();
            }

            return View(driverPassenger);
        }

        // POST: DriverPassengers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driverPassenger = await _context.DriverPassengers.FindAsync(id);
            _context.DriverPassengers.Remove(driverPassenger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverPassengerExists(int id)
        {
            return _context.DriverPassengers.Any(e => e.Id == id);
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
            int[] NonDuplicateStatus = { 1, 2, 3, 4 };

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if(currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                    .Where(e => e.Id == currentUser.EmployeeId)
                    .SelectMany(e => e.EmployeeLocations.Select(el => el.Location))
                    .Select(x => x.Id)
                    .ToListAsync();


                var PassengerList = _context.DriverPassengers.Where(r => NonDuplicateStatus.Contains(r.Status)
                    && r.ServiceDate >= lastMonth
                    && r.ServiceDate <= nextMonth
                    && _context.Employees
                        .Any(e => e.Id == r.EmployeeId
                               && e.CompanyGroupId == currentCompanyGroupId
                               && e.EmployeeLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                    ))
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
            else
            {
                var PassengerList = _context.DriverPassengers.Where(r => NonDuplicateStatus.Contains(r.Status)
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
            int[] HideStatus = new int[2];
            HideStatus[0] = 8;
            string trip = Request.Form["trip"];
            if (trip == "1") {
                HideStatus[1] = 6;
            }

            var DataList = _context.DriverPassengers.Where(r => !HideStatus.Contains(r.Status) && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"])  && r.DriverPassengerHeaderId.Equals(Request.Form["tid"]) )
                            .Select(u => new
                            {
                                Id = (u.Status == 6 && u.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))) ? "<a href='javascript:void(0);' class='btn btn-sm btn-primary' onClick='RemoveEmployeeAjax(`" + u.Id + "`);'> Remove </a>" : "",
                                Company = u.CompanyListId == 1 ? u.CompanyList.CompanyName + " " + u.CompanyOther : u.CompanyList.CompanyName,
                                EmployeeID = u.EmployeeNo,
                                PassengerName = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                                Position = u.Position,
                                Remarks = u.Remarks,
                                ServiceType = u.ServiceType.Description,
                                OriginTag = _context.OriginDestination.Where(x => x.Id == u.OriginId).Select(x => x.OriginDestinationName).FirstOrDefault(),
                                Origin = u.Origin,
                                DestinationTag = _context.OriginDestination.Where(x => x.Id == u.DestinationId).Select(x => x.OriginDestinationName).FirstOrDefault(),
                                Destination = u.Destination,
                                TripTimeFrom = u.TripTimeFrom,
                                TripTimeTo = u.TripTimeTo,
                                Instruction = u.Instructions,
                                Purpose = u.Purpose,
                                StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + u.ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss ") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED - By: " + u.CancelledBy + " - Reason: " + u.CancelReason : u.Status == 6 ? "OPEN" : u.Status == 8 ? "REMOVED" : u.Status == 9 ? "RESERVED" : "",

                            });

            // Total record count.
            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            
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
                    var driverPassenger = _context.DriverPassengers.FirstOrDefault(s => s.Id == id);
                    driverPassenger.Status = 8;
                    _context.Update(driverPassenger);
                    await _context.SaveChangesAsync();


                    //logs
                    Log _log = new Log();
                    _log.Process = "Remove Driver/Service Reservation For ID" + id + " -> " + driverPassenger.FirstName + " " + driverPassenger.LastName + " / Time Stamp: " + driverPassenger.ServiceDateTimeStamp;
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
            var returnSuccess = 1;
            var returnMessage = "";
            var _otherReservation = "";
            var _transactionId = "";
            var tid = Convert.ToInt32(HttpContext.Request.Query["tid"].ToString());
            var _Counter = 0;
            int driverHeadId = 0;

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();


            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 2);
            if (parameterTemp.Year < DateTime.Now.Year)
            {
                parameterTemp.Value = 1;
                parameterTemp.Year = DateTime.Now.Year;
                _context.Update(parameterTemp);
                _context.SaveChanges();
            }
            ////additional Validation 10142020
            //var driverPassengerHeadTemp = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(tid));
            //if(driverPassengerHeadTemp.TransactionId == )
            ////end  10142020


            //End Manage Parameters

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    //commented as requested by AGS 05062020 EBE during ECQ
                    //if (dtDateTime < DateTime.Now)
                    //{
                    //    throw new System.Exception("Invalid reservation date.");
                    //}
                    var driverPassengerHead = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(tid));
                    if (driverPassengerHead == null)
                    {
                        throw new System.Exception("There is no reservation/passenger to submit.");
                    }
                    if (driverPassengerHead.TransactionId == "" || driverPassengerHead.TransactionId == null)
                    {

                        var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 2);
                        _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                        parameter.Value = parameter.Value + 1;
                        _context.Update(parameter);

                        driverPassengerHead.TransactionId = _transactionId;
                        driverPassengerHead.Status = 1;
                        _context.Update(driverPassengerHead);

                        _context.SaveChanges();


                    }
                    else
                    {

                        _transactionId = driverPassengerHead.TransactionId;

                    }

                    driverHeadId = driverPassengerHead.Id;


                    var driverPassenger = _context.DriverPassengers.Where(s => s.ServiceDateTimeStamp == timestamp && s.Status == 6 && s.DriverPassengerHeaderId.Equals(tid.ToString()));
                    

                    if (driverPassenger.Count() <= 0)
                    {
                        throw new System.Exception("There is no reservation to submit.");
                    }



                    //Dito ORIG NA NAKALAGAY YUNG SMS
                    //string[] smsArr = new string[999];
                    //var smsCodesArr = new Dictionary<string, string>();
                    //int smsArrIndex = 0;
                    //string smsReferenceNo = "";
                    await driverPassenger.ForEachAsync(e =>
                    {
                        e.Status = 1;
                        e.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        e.ModifyDate = DateTime.Now;
                        e.SubmitDate = DateTime.Now;
                        e.TransactionId = _transactionId;
                    });
                    
                    //end


                        //logs
                        Log _log = new Log();
                    _log.Process = "Submit Reservation Trans No.: " + _transactionId;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

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

                //New Validation 10142020

            }
            //using (var transaction = _context.Database.BeginTransaction())
            //{


            string smsReferenceNo = "", _TxtMessage = ""; var _MobileNumber = "";
            try
            {
                if (returnSuccess == 1){

                    var driverPassenger = _context.DriverPassengers.Where(s => s.ServiceDateTimeStamp == timestamp && s.Status == 1 && s.DriverPassengerHeaderId.Equals(tid.ToString()));

                    string[] smsArr = new string[999];
                    var smsCodesArr = new Dictionary<string, string>();
                    int smsArrIndex = 0;
                    var systemName = "";
                    //await driverPassenger.ForEachAsync(e => //JPT commented code 09112024
                    foreach (var e in driverPassenger) //JPT additional code 09112024
                    {
                        //e.Status = 1;
                        //e.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        //e.ModifyDate = DateTime.Now;
                        //e.SubmitDate = DateTime.Now;
                        //e.TransactionId = _transactionId;
                        //-----

                        _MobileNumber = "";
                        if (_context.SmsTransactionCodes.Count(s => s.TransactionId.Equals(_transactionId) && s.ApproverEmployeeNo.Equals(e.OriginalApproverEmployeeNo)) == 0)
                        {

                            if (!smsArr.Contains(e.OriginalApproverEmployeeNo))
                            {
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
                                    else if (System.Environment.MachineName == "ANDROMEDA" )
                                    {
                                        smsReferenceNo = "C" + smsReferenceNo;
                                    }
                                    else if (System.Environment.MachineName == "PEGASUS" )
                                    {
                                        smsReferenceNo = "P" + smsReferenceNo;
                                    }
                                    else
                                    {
                                        smsReferenceNo = "L" + smsReferenceNo;
                                    }
                                    //End ADDED BY EBE during ECQ
                                } while (_context.SmsTransactionCodes.Count(s => s.ReferenceNo.Equals(smsReferenceNo)) > 0);
                                //as of 01-29-2020 -> ApproverEmployeeNo and OriginalApproverEmployeeNo will be the same

                                tempSmsTransactionCodes.ReferenceNo = smsReferenceNo;
                                tempSmsTransactionCodes.TransactionId = _transactionId;
                                tempSmsTransactionCodes.ApproverEmployeeNo = e.OriginalApproverEmployeeNo;
                                tempSmsTransactionCodes.OriginalApproverEmployeeNo = e.OriginalApproverEmployeeNo;
                                tempSmsTransactionCodes.ReservationTypeId = 2;
                                _context.Add(tempSmsTransactionCodes);
                                smsArr[smsArrIndex] = e.OriginalApproverEmployeeNo;
                                smsCodesArr.Add(e.OriginalApproverEmployeeNo, smsReferenceNo);

                                //_context.SaveChanges();
                                //end
                                e.SmsId = smsReferenceNo;
                                smsArrIndex++;

                                //sending message

                                string _names = "";
                                var _getSMSname = _context.DriverPassengers.Where(g => g.DriverPassengerHeaderId.Equals(driverHeadId.ToString()) && g.OriginalApproverEmployeeNo.Equals(e.OriginalApproverEmployeeNo) && g.Status.Equals(1)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).ToArray();

                                foreach (string fullname in _getSMSname.Select(x => x.FullName))
                                {
                                    _names = _names + ", " + fullname;
                                }

                                string _destination = e.Destination;

                                 _TxtMessage = "Ref code: " + smsReferenceNo + Environment.NewLine +
                                                    "A vehicle request of " + _names + " going to " + _destination + " on " + e.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + " is pending for your approval. " +
                                                     //"Type VR 2 if approve or VR 3 if disapprove with the ref code. Ex. VR 2 " + smsReferenceNo + " . Ty. "; //JPT commented code 09112024
                                                     "Type 1 "+ smsReferenceNo + " THSM to approve or 2 "+ smsReferenceNo + " THSM to disapprove. Ex: 1 " + smsReferenceNo + " THSM and send response to: 09191610404."; //JPT additional code 09112024


                                //string _TxtMessage = "You have shuttle reservation pending approval for " + _names +
                                //                     ". Trip date " + e.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + ". Key in VR 2 for Approved, VR 3 for Disapproved and the reference code:  " + smsReferenceNo + " (Ex. VR 2 " + smsReferenceNo + ")";


                                //var _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).MobileNumber;

                                // if (e.PassengerTypeId == 1)
                                //{
                                if (e.OriginalApproverEmployeeNo != "")
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).MobileNumber;
                              //  }
                              //  else
                              //  {
                               //     _MobileNumber = e.ContactNo;
                              //  }

                               

                                var _Email = "";
                                //if (e.PassengerTypeId == 1)
                                //{
                                    _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.OriginalApproverEmployeeNo)).CompanyEmail;
                                //}

                                if (_Email != "")
                                {

                                    var callback = Url.Action("approval", "driverpassengers");
                                    var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;
                               
                                    //ADDED BY EBE during ECQ
                                    var hostLink = "";
                                    if (System.Environment.MachineName == "SODIUM2")
                                    {
                                        //hostLink = "http://sodium2/TransportHub/driverpassengers/approval";
                                        hostLink = "http://sodium2/TransportHubCPC/driverpassengers/approval";
                                        systemName = "THS Makati";
                                    }
                                    else if (System.Environment.MachineName == "ALUMINUM")
                                    {
                                        hostLink = "https://stem.aunasin.com:8443/TransportHub/driverpassengers/approval";
                                        systemName = "THS Makati";

                                    }
                                    else if (System.Environment.MachineName == "CALIFORNIUM")
                                    {
                                        hostLink = "https://www.semirarampc.com:8443/TransportHub/shuttlepassengers/approval";
                                        systemName = "THS Makati";
                                    }
                                    else if (System.Environment.MachineName == "ANDROMEDA")
                                    {
                                        hostLink = "http://192.168.30.182/TransportHubCPC/shuttlepassengers/approval";
                                    }
                                    else if (System.Environment.MachineName == "PEGASUS")
                                    {
                                        hostLink = "http://192.168.30.148/TransportHubCPC/shuttlepassengers/approval";
                                        systemName = "THS Calaca";
                                    }
                                    else if (System.Environment.MachineName == "CRONUS" || System.Environment.MachineName == "HYACINTHUSX")
                                    {
                                        hostLink = "https://access3.aratilisms.com:4413/TransportHubMS/shuttlepassengers/approval";
                                        systemName = "THS Minesite";
                                    }
                                    else
                                    {
                                        hostLink = "http://sodium2/TransportHub/driverpassengers/approval";
                                        systemName = "THS Makati";
                                    }
                                    //End ADDED BY EBE during ECQ

                                    //ebe 05072020 -> ECQ
                                    string newContent = "THSR ID: " + _transactionId + " <br/> " +
                                                    "A vehicle request of " + _names + " going to " + _destination + " on " + e.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + " is pending for your approval. ";
                                    //ebe 05072020 -> ECQ

                                    new ReportsController().SendEmail("", _Email, _senderEmail, newContent + " <br/> " + " Please click the link: " + "<br/> <a href='" + hostLink + "'> TransportHub/Approval </a>", " Driver/Vehicle for Approval");
                                }

                                //end sending message
                            }
                            else
                            {
                                e.SmsId = smsCodesArr[e.OriginalApproverEmployeeNo];
                            }


                            if (_MobileNumber != "")
                            {
                                //new ReportsController().CallSMSAPI(_MobileNumber, _TxtMessage, smsReferenceNo); //JPT commented code 09112024

                                //JPT additional code 09112024
                                string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                string message = _TxtMessage;
                                string referenceNo = smsReferenceNo;  // Replace with your transaction ID
                                //string systemName = "THS Makati";

                                // Call the SendSmsAsync method
                                string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                //JPT end additional code 09112024
                            }
                        }
                        //----
                    } //JPT additional code 09112024
                        //}); //JPT commented code 09112024
                }

                await _context.SaveChangesAsync();

                //  transaction.Commit();

            }
                    catch (Exception e)
                    {

                       // transaction.Rollback();
                        _otherReservation = " But has error on SMS: " + e.Message;
                    }

            //end New Validation 10142020

            //}
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
        public async Task<ActionResult> CheckPassengerReservation()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if(currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                    .Where(e => e.Id == currentUser.EmployeeId)
                    .SelectMany(e => e.EmployeeLocations.Select(el => el.Location))
                    .Select(x => x.Id)
                    .ToListAsync();

                var DataList = _context.DriverPassengerHeaders.
                               OrderBy(x => x.TransactionId).
                               Where(r => r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"])
                               && _context.Employees
                                        .Any(e => e.Id == r.EmployeeId
                                               && e.CompanyGroupId == currentCompanyGroupId
                                               && e.EmployeeLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                               )).
                               Select(u => new
                               {
                                   //Action = "<a href='" + @Url.Content("~/driverpassengers") + "/edit/" + u.TransactionId + "'  class='btn btn-sm btn-primary' > View/Update </a>",
                                   Action = "<a href='" + @Url.Content("~/driverpassengers") + "/create?t=" + u.Id + "&tempid=" + u.TempTransactionId + "&d=" + u.ServiceDateTimeStamp + "'  class='btn btn-sm btn-primary' > View / Update </a>",
                                   No = u.TransactionId,
                                   Name = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                                   //StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : "",
                                   StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + _context.DriverPassengers.OrderByDescending(y => y.ApprovedDatetime).FirstOrDefault(e => e.ShuttleId.Equals(u.ShuttleId) && e.TransactionId.Equals(u.TransactionId) && e.Status.Equals(2)).ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : u.Status == 7 ? "DECLINED" : u.Status == 9 ? "RESERVED" : "",
                                   Origin = u.Origin,
                                   Destination = u.Destination,
                                   Time = u.TripTimeFrom + " - " + u.TripTimeTo,
                                   Instruction = u.Instructions,
                                   Purpose = u.Purpose,
                                   PassengerCount = _context.DriverPassengers.Count(e => e.TransactionId.Equals(u.TransactionId) && e.Status < 5),
                                   AssignedVehicle = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).VehicleList.Model,
                                   AssignedDriver = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.FirstName + " " + _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.LastName,
                                   RemarksTrip = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Remarks
                               }).Distinct();
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
            else
            {

                //var currentYear = DateTime.Now.Year;

                //var DataList = _context.DriverPassengerHeaders
                //    .Where(r => Convert.ToDateTime(r.ServiceDate).Year == currentYear)
                //    .OrderBy(x => x.TransactionId)
                //    .Select(u => new
                //    {
                //        Action = "<a href='" + Url.Content("~/driverpassengers") +
                //                 "/create?t=" + u.Id +
                //                 "&tempid=" + u.TempTransactionId +
                //                 "&d=" + u.ServiceDateTimeStamp +
                //                 "' class='btn btn-sm btn-primary'>View / Update</a>",

                //        No = u.TransactionId,

                //        Name = u.Employee.FirstName + " " +
                //              (string.IsNullOrEmpty(u.Employee.MiddleName)
                //                ? ""
                //                : u.Employee.MiddleName.Substring(0, 1) + ". ")
                //              + u.Employee.LastName,

                //        StatusDescription =
                //            u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" :
                //            u.Status == 2 ? "<b>APPROVED</b>" :
                //            u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" :
                //            u.Status == 4 ? "CLOSED" :
                //            u.Status == 5 ? "CANCELLED" :
                //            u.Status == 7 ? "DECLINED" :
                //            u.Status == 9 ? "RESERVED" : "",

                //        Origin = u.Origin,
                //        Destination = u.Destination,
                //        Time = u.TripTimeFrom + " - " + u.TripTimeTo,
                //        Instruction = u.Instructions,
                //        Purpose = u.Purpose,

                //        PassengerCount = _context.DriverPassengers
                //            .Count(e => e.TransactionId == u.TransactionId && e.Status < 5 && Convert.ToDateTime(e.ServiceDate).Year == currentYear),

                //        AssignedVehicle = _context.Trip
                //            .Where(e => e.Id == u.ShuttleId && Convert.ToDateTime(e.ServiceStartDate).Year == currentYear)
                //            .Select(e => e.VehicleList.Model)
                //            .FirstOrDefault(),

                //        AssignedDriver = _context.Trip
                //            .Where(e => e.Id == u.ShuttleId && Convert.ToDateTime(e.ServiceStartDate).Year == currentYear)
                //            .Select(e => e.Driver.FirstName + " " + e.Driver.LastName)
                //            .FirstOrDefault(),

                //        RemarksTrip = _context.Trip
                //            .Where(e => e.Id == u.ShuttleId && Convert.ToDateTime(e.ServiceStartDate).Year == currentYear)
                //            .Select(e => e.Remarks)
                //            .FirstOrDefault(),

                //        CompanyGroupId = _context.DriverPassengers
                //            .Where(p => p.TransactionId == u.TransactionId && Convert.ToDateTime(p.ServiceDate).Year == currentYear)
                //            .Select(p => p.Employee.CompanyGroupId)
                //            .FirstOrDefault()
                //    });

                //DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);

                //int totalRecords = DataList.Count();

                //// Filter record count.
                //int recFilter = DataList.Count();

                //// Apply pagination.
                //DataList = DataList.Skip(startRec).Take(pageSize);

                //var jsonData = new
                //{
                //    draw = Convert.ToInt32(draw),
                //    recordsTotal = totalRecords,
                //    recordsFiltered = recFilter,
                //    data = DataList.ToList(),
                //};

                //return new JsonResult(jsonData);

                var DataList = _context.DriverPassengerHeaders.
                               OrderBy(x => x.TransactionId).
                               Where(r => r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"])
                               && _context.Employees
                                        .Any(e => e.Id == r.EmployeeId
                                               && e.CompanyGroupId == currentCompanyGroupId)
                               ).
                               Select(u => new
                               {
                                   //Action = "<a href='" + @Url.Content("~/driverpassengers") + "/edit/" + u.TransactionId + "'  class='btn btn-sm btn-primary' > View/Update </a>",
                                   Action = "<a href='" + @Url.Content("~/driverpassengers") + "/create?t=" + u.Id + "&tempid=" + u.TempTransactionId + "&d=" + u.ServiceDateTimeStamp + "'  class='btn btn-sm btn-primary' > View / Update </a>",
                                   No = u.TransactionId,
                                   Name = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                                   //StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : "",
                                   StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + _context.DriverPassengers.OrderByDescending(y => y.ApprovedDatetime).FirstOrDefault(e => e.ShuttleId.Equals(u.ShuttleId) && e.TransactionId.Equals(u.TransactionId) && e.Status.Equals(2)).ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 5 ? "CANCELLED" : u.Status == 7 ? "DECLINED" : u.Status == 9 ? "RESERVED" : "",
                                   Origin = u.Origin,
                                   Destination = u.Destination,
                                   Time = u.TripTimeFrom + " - " + u.TripTimeTo,
                                   Instruction = u.Instructions,
                                   Purpose = u.Purpose,
                                   PassengerCount = _context.DriverPassengers.Count(e => e.TransactionId.Equals(u.TransactionId) && e.Status < 5),
                                   AssignedVehicle = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).VehicleList.Model,
                                   AssignedDriver = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.FirstName + " " + _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.LastName,
                                   RemarksTrip = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Remarks,
                                   CompanyGroupId = 1,
                               }).Distinct();
                DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);
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

           

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CheckPassengerReservationAdmin()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);
            int[] statusArray = new int[20];
            statusArray[0] = 0;
            if (Request.Form["filterstatus"] == "All")
            {
                statusArray[1] = 1;
                statusArray[2] = 2;
                statusArray[3] = 9;
                statusArray[4] = 5;
                statusArray[5] = 7;
                statusArray[6] = 4;//closed
                statusArray[7] = 10;//surveyed
            }
            else if (Request.Form["filterstatus"] == "Approved")
            {
                statusArray[1] = 2;
            }
            else if (Request.Form["filterstatus"] == "For Approval")
            {
                statusArray[1] = 1;
            }
            else if (Request.Form["filterstatus"] == "Reserved")
            {
                statusArray[1] = 9;
            }
            else if (Request.Form["filterstatus"] == "Cancelled/Declined")
            {
                statusArray[1] = 5;
                statusArray[2] = 7;
            }
            else if (Request.Form["filterstatus"] == "Closed")
            {
                statusArray[1] = 4;
            }
            else if (Request.Form["filterstatus"] == "Surveyed")
            {
                statusArray[1] = 10;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if(currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                   .Where(e => e.Id == currentUser.EmployeeId)
                   .SelectMany(e => e.EmployeeLocations.Select(el => el.Location))
                   .Select(x => x.Id)
                   .ToListAsync();

                var DataList = _context.DriverPassengerHeaders.
                            OrderBy(x => x.TransactionId).
                            Where(r => statusArray.Contains(r.Status) && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"])
                                && _context.Employees
                                        .Any(e => e.Id == r.EmployeeId
                                               && e.CompanyGroupId == currentCompanyGroupId
                                               && e.EmployeeLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                            )).
                            Select(u => new
                            {
                                //Action = "<a href='" + @Url.Content("~/driverpassengers") + "/edit/" + u.TransactionId + "'  class='btn btn-sm btn-primary' > View/Update </a>",
                                //Action = (u.Status == 2 || u.Status == 9) ? "<a href='" + @Url.Content("~/trips") + "/AssignTrip?t=" + u.Id + "&tempid=" + u.TempTransactionId + "&d=" + u.ServiceDateTimeStamp + "'  class='btn btn-block btn-sm btn-primary' > Manage Trip </a>" +
                                //            "<a href='javascript:void(0);'  onClick='cancelTrip(" + u.Id + ")'   class='btn btn-block btn-sm btn-danger' > Cancel </a>" +
                                //            "<a href='javascript:void(0);'  onClick='declineTrip(" + u.Id + ")'    class='btn btn-block btn-sm btn-warning' > Decline </a>" +
                                //            "<a href='javascript:void(0);' onClick='closeTrip(" + u.Id + ")'    class='btn btn-block btn-sm btn-success' > Close Ticket </a>" +
                                //            ""
                                //            : "",
                                u.Id,
                                No = u.TransactionId,
                                Name = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                                RequestDateTime = u.EncodeDate.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss"),
                                StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + _context.DriverPassengers.OrderByDescending(y => y.ApprovedDatetime).FirstOrDefault(e => e.ShuttleId.Equals(u.ShuttleId) && e.TransactionId.Equals(u.TransactionId) && e.Status.Equals(2)).ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 10 ? "SURVEYED" : u.Status == 5 ? "<b style='color:orange!important'>CANCELLED</a>" : u.Status == 7 ? "<b style='color:orange!important'>DECLINED</a>" : u.Status == 9 ? "RESERVED" : "",
                                u.Status,
                                u.Origin,
                                u.Destination,
                                Time = u.TripTimeFrom + " - " + u.TripTimeTo,
                                Instruction = u.Instructions,
                                u.Purpose,
                                u.TempTransactionId,
                                u.ServiceDateTimeStamp,
                                PassengerCount = _context.DriverPassengers.Count(e => e.TransactionId.Equals(u.TransactionId) && e.Status < 5),
                                AssignedVehicle = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).VehicleList.Model,
                                AssignedDriver = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.FirstName + " " + _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.LastName,
                                RemarksTrip = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Remarks,
                            }).Distinct();
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
            else
            {
                var DataList = _context.DriverPassengerHeaders.
                            OrderBy(x => x.TransactionId).
                            Where(r => statusArray.Contains(r.Status) && r.ServiceDateTimeStamp.ToString().Contains(Request.Form["timestamp"])).
                            Select(u => new
                            {
                                //Action = "<a href='" + @Url.Content("~/driverpassengers") + "/edit/" + u.TransactionId + "'  class='btn btn-sm btn-primary' > View/Update </a>",
                                //Action = (u.Status == 2 || u.Status == 9) ? "<a href='" + @Url.Content("~/trips") + "/AssignTrip?t=" + u.Id + "&tempid=" + u.TempTransactionId + "&d=" + u.ServiceDateTimeStamp + "'  class='btn btn-block btn-sm btn-primary' > Manage Trip </a>" +
                                //            "<a href='javascript:void(0);'  onClick='cancelTrip(" + u.Id + ")'   class='btn btn-block btn-sm btn-danger' > Cancel </a>" +
                                //            "<a href='javascript:void(0);'  onClick='declineTrip(" + u.Id + ")'    class='btn btn-block btn-sm btn-warning' > Decline </a>" +
                                //            "<a href='javascript:void(0);' onClick='closeTrip(" + u.Id + ")'    class='btn btn-block btn-sm btn-success' > Close Ticket </a>" +
                                //            ""
                                //            : "",
                                u.Id,
                                No = u.TransactionId,
                                Name = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                                RequestDateTime = u.EncodeDate.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss"),
                                StatusDescription = u.Status == 1 ? "<b style='color:red!important'>FOR SUPERVISOR'S APPROVAL</b>" : u.Status == 2 ? "<b>APPROVED - As of " + _context.DriverPassengers.OrderByDescending(y => y.ApprovedDatetime).FirstOrDefault(e => e.ShuttleId.Equals(u.ShuttleId) && e.TransactionId.Equals(u.TransactionId) && e.Status.Equals(2)).ApprovedDatetime.GetValueOrDefault().ToString("MM/dd/yyyy HH:mm:ss") + "</b>" : u.Status == 3 ? "<b style='color:orange!important'>REJECTED</b>" : u.Status == 4 ? "CLOSED" : u.Status == 10 ? "SURVEYED" : u.Status == 5 ? "<b style='color:orange!important'>CANCELLED</a>" : u.Status == 7 ? "<b style='color:orange!important'>DECLINED</a>" : u.Status == 9 ? "RESERVED" : "",
                                u.Status,
                                u.Origin,
                                u.Destination,
                                Time = u.TripTimeFrom + " - " + u.TripTimeTo,
                                Instruction = u.Instructions,
                                u.Purpose,
                                u.TempTransactionId,
                                u.ServiceDateTimeStamp,
                                PassengerCount = _context.DriverPassengers.Count(e => e.TransactionId.Equals(u.TransactionId) && e.Status < 5),
                                AssignedVehicle = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).VehicleList.Model,
                                AssignedDriver = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.FirstName + " " + _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Driver.LastName,
                                RemarksTrip = _context.Trip.FirstOrDefault(e => e.Id.Equals(u.ShuttleId)).Remarks,
                                //CompanyGroupId = _context.DriverPassengers
                                //       .Where(p => p.TransactionId == u.TransactionId)
                                //       .Select(p => _context.Employees
                                //           .Where(e => e.Id == p.EmployeeId)
                                //           .Select(e => e.CompanyGroupId)
                                //           .FirstOrDefault()
                                //       )
                                //       .FirstOrDefault()
                                CompanyGroupId = 1,
                            }).Distinct();

                DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);
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
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ReservationForApprovalGroup()
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

            var _DataList = _context.DriverPassengers;

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if(currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                    .Where(e => e.Id == currentUser.EmployeeId)
                    .SelectMany(e => e.EmployeeLocations.Select(el => el.Location))
                    .Select(x => x.Id)
                    .ToListAsync();

                var DataListTemp = _DataList.Where(u => u.Status == 1
                    && _context.Employees
                        .Any(e => e.Id == u.EmployeeId
                               && e.CompanyGroupId == currentCompanyGroupId
                               && e.EmployeeLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                ));

                if (roles != 1)
                {
                    DataListTemp = _DataList.Where(u => u.Status == 1 && u.InitialApproverEmployeeNo == approverData.EmployeeNo);
                }

                var DataList = DataListTemp.Select(u => new
                {
                    Id = u.Status == 1 ? "<a href='javascript:void(0);' class='btn btn-sm btn-primary'  " +
                                            "onClick='ViewEmployeeAjax(`" + u.TransactionId + "`," +
                                            "`" + u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName + "`," +
                                            "`" + u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + "`,`" + u.ChargingCompany.CompanyName + "`,`" + u.ChargingDepartment.DepartmentName + "`,`" + u.TripTimeFrom + " - " + u.TripTimeTo + "`,`" + u.Origin + " to " + u.Destination + "`);'>" +
                                            " View </a>" : "",
                    TransactionId = u.TransactionId,
                    NatureApplication = "DRIVER/VEHICLE REQUEST",
                    u.Instructions,
                    DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripTime = u.ServiceDate.GetValueOrDefault().ToString("HH:mm"),//new added during ECQ by EBE
                    Destination = u.Origin + " to " + u.Destination,//new added during ECQ by EBE
                    RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                    CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='" + u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
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
            else
            {
                var DataListTemp = _DataList.Where(u => u.Status == 1
                && _context.CompanyLists
                    .Where(c => c.Id == u.CompanyListId)
                    .Select(c => c.CompanyGroupId)
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
                                            "`" + u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy") + "`,`" + u.ChargingCompany.CompanyName + "`,`" + u.ChargingDepartment.DepartmentName + "`,`" + u.TripTimeFrom + " - " + u.TripTimeTo + "`,`" + u.Origin + " to " + u.Destination + "`);'>" +
                                            " View </a>" : "",
                    TransactionId = u.TransactionId,
                    NatureApplication = "DRIVER/VEHICLE REQUEST",
                    u.Instructions,
                    DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripTime = u.ServiceDate.GetValueOrDefault().ToString("HH:mm"),//new added during ECQ by EBE
                    Destination = u.Origin + " to " + u.Destination,//new added during ECQ by EBE
                    RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                    CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='" + u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
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

            var _DataList = _context.DriverPassengers;

            var DataListTemp = _DataList.Where(u => u.Status == 1 && u.TransactionId == Request.Form["TransactionId"]);

            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 1 && u.InitialApproverEmployeeNo == approverData.EmployeeNo && u.TransactionId == Request.Form["TransactionId"]);
            }

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Id,
                TransactionId = u.TransactionId,
                u.Instructions,
                NatureApplication = "DRIVER/VEHICLE REQUEST",
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


            var returnSuccess = 1;
            var returnMessage = "";
            var _otherReservation = "";
            var _transactionId = "";
            var tid = Convert.ToInt32(HttpContext.Request.Query["tid"].ToString());
            var _Counter = 0;

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();


            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 2);
            if (parameterTemp.Year < DateTime.Now.Year)
            {
                parameterTemp.Value = 1;
                parameterTemp.Year = DateTime.Now.Year;
                _context.Update(parameterTemp);
                _context.SaveChanges();
            }



            //End Manage Parameters



            var TransactionIds = "";
            int approveCounter = 0;


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {


                    //SUBMIT

                    var _driverPassengerHead = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(tid));

                    if (_driverPassengerHead == null)
                    {
                        throw new System.Exception("There is no reservation/passenger to submit.");
                    }

                    if (_driverPassengerHead.TransactionId == "" || _driverPassengerHead.TransactionId == null)
                    {

                        var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 2);
                        _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                        parameter.Value = parameter.Value + 1;
                        _context.Update(parameter);

                        _driverPassengerHead.TransactionId = _transactionId;
                        _driverPassengerHead.Status = 1;
                        _context.Update(_driverPassengerHead);

                        _context.SaveChanges();


                    }
                    else
                    {

                        _transactionId = _driverPassengerHead.TransactionId;

                    }


                    var _driverPassenger = _context.DriverPassengers.Where(s => s.ServiceDateTimeStamp == timestamp && s.Status == 6 && s.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))) && s.DriverPassengerHeaderId.Equals(tid.ToString()));


                    if (_driverPassenger.Count() <= 0)
                    {
                        throw new System.Exception("There is no reservation to submit.");
                    }



                    await _driverPassenger.ForEachAsync(e =>
                    {
                        e.Status = 1;
                        e.LastModifiedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        e.ModifyDate = DateTime.Now;
                        e.SubmitDate = DateTime.Now;
                        e.TransactionId = _transactionId;
                        e.SmsId = "";
                    });

                    await _context.SaveChangesAsync();





                    //END SUBMIT

                    TransactionIds = _transactionId;
                    int headId = 0;

                    string[] transIds = TransactionIds.Split(",");
                    foreach (string reference in transIds)
                    {
                        if (reference != "")
                        {
                            var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 1  && s.EmployeeId.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId"))));

                            if (driverPassenger.Count() > 0)
                            {
                                await driverPassenger.ForEachAsync(e => {
                                    e.Status = 2;
                                    e.ModifyDate = DateTime.Now;
                                    e.ApprovedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                    e.ApprovedDatetime = DateTime.Now;
                                    e.ShuttleId = _driverPassengerHead.ShuttleId;
                                    headId = Convert.ToInt32(e.DriverPassengerHeaderId);
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

                            if (approveCounter > 0)
                            {
                                var driverPassengerChecker = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 1).Count();
                                var driverHead = _context.DriverPassengerHeaders.FirstOrDefault(h => h.Id == headId).Status;

                                if (driverPassengerChecker <= 0 && driverHead != 9)
                                {
                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId == reference);
                                    driverPassengerHeader.Status = 2;
                                    _context.Update(driverPassengerHeader);
                                    _context.SaveChanges();
                                }
                            }
                        }
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
                        if(reference != "") { 
                            var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 1 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                            if (driverPassenger.Count() > 0)
                            {
                                //await driverPassenger.ForEachAsync(e => { //JPT commented code 09062024
                                foreach (var e in driverPassenger){ //JPT additional code 09062024
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
                                    var _location = HttpContext.Session.GetString("Session_location");

                                    if(_location == "makati")
                                    {
                                        _MobileNumber = _MobileNumber + "," + _context.Employees.FirstOrDefault(ags => ags.EmployeeNo.Equals("9080546")).MobileNumber.ToString();
                                    }
                                    else
                                    {
                                        _MobileNumber = _MobileNumber + "," + _context.Employees.FirstOrDefault(ags => ags.EmployeeNo.Equals("0024236")).MobileNumber.ToString();
                                    }
                                                     


                                    //end


                                    string _message = "";
                                    string systemName = "";

                                    if (_method == 2)
                                    {
                                        _message = "Your request with THSR ID " + e.TransactionId + " is now approved and is now queued for assigning of vehicle.";
                                        systemName = "THS Minesite";
                                    }
                                    else
                                    {
                                        _message = "Your request with THSR dID " + e.TransactionId + " has been disapproved. Please contact your supervisor for confirmation.";
                                        systemName = "THS Minesite";
                                    }

                                    //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                                    //JPT additional code 09052024
                                    string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                    string message = _message;
                                    string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                  

                                    // Call the SendSmsAsync method
                                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                    //JPT end additional code 09052024

                                    //new Email notif EBE - ECQ - 05072020

                                    var _Email = "";
                                    if (e.PassengerTypeId == 1)
                                    {
                                        _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                    }

                                    
                                    if (_Email != "")
                                    {
                                        var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                        new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                    }
                                    //end new email Notif EBE - ECQ - 05072020

                                    //end

                                } //JPT additional code 09062024
                                //}); //JPT commented code 09062024
                                await _context.SaveChangesAsync();
                                approveCounter++;
                            }

                            //logs
                            Log _log = new Log();
                            _log.Process = returnMessage + " Reservation Trans No.: " + reference;
                            _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                            _log.ProcessedDate = DateTime.Now;
                            _context.Add(_log);
                            //end logs

                            if (approveCounter > 0) {
                                var driverPassengerChecker = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 1 ).Count();

                                if (driverPassengerChecker <= 0)
                                {
                                    var checkifhasapproved = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 2).Count(); //ebe 09162020
                                    var checkifhasrejected = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 3).Count(); //ebe 09162020
                                    var newStatus = 0;
                                    if(checkifhasapproved > 0)
                                    {
                                        newStatus = 2;
                                    }
                                    else if(checkifhasrejected > 0 && checkifhasapproved == 0)
                                    {
                                        newStatus = 3;
                                    }
                                    else
                                    {
                                        newStatus = 1;
                                    }
                                    // 09192020

                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId == reference);
                                    driverPassengerHeader.Status = newStatus;
                                    _context.Update(driverPassengerHeader);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to approve.");
                    }


                    returnMessage = _method == 2 ? "Approved" : "Disapproved";




                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    if (returnMessage.Contains("instance")) {
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

                        if (reference != "")
                        {
                                var driverPassenger = _context.DriverPassengers.Where(s => s.Id.ToString() == reference && s.Status == 1 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                            if (driverPassenger.Count() > 0)
                            {
                                await driverPassenger.ForEachAsync(e => {
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

                                    //comment for dev 05272021
                                    //new ReportsController().SendSMS(_message, _MobileNumber);

                                    //new Email notif EBE - ECQ - 05072020

                                    var _Email = "";
                                    if (e.PassengerTypeId == 1)
                                    {
                                        _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                    }

                                    if (_Email != "")
                                    {
                                        var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;
                                        //comment for dev 05272021
                                        //new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                    }
                                    //end new email Notif EBE - ECQ - 05072020

                                    //end

                                });
                                await _context.SaveChangesAsync();
                                approveCounter++;
                            }

                            if (approveCounter > 0)
                            {
                                var driverPassenger1 = _context.DriverPassengers.FirstOrDefault(s => s.Id.ToString() == reference);

                                var driverPassengerChecker = _context.DriverPassengers.Where(s => s.TransactionId == driverPassenger1.TransactionId && s.Status == 1).Count();

                                //if (driverPassengerChecker <= 0)
                                //{

                                //    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId.Equals(driverPassenger1.TransactionId));
                                //    driverPassengerHeader.Status = 2;
                                //    _context.Update(driverPassengerHeader);
                                //    _context.SaveChanges();
                                //}
                                if (driverPassengerChecker <= 0)
                                {
                                    var checkifhasapproved = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 2).Count(); //ebe 09162020
                                    var checkifhasrejected = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 3).Count(); //ebe 09162020
                                    var newStatus = 0;
                                    if (checkifhasapproved > 0)
                                    {
                                        newStatus = 2;
                                    }
                                    else if (checkifhasrejected > 0 && checkifhasapproved == 0)
                                    {
                                        newStatus = 3;
                                    }
                                    else
                                    {
                                        newStatus = 1;
                                    }
                                    // 09192020

                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId.Equals(driverPassenger1.TransactionId));
                                    driverPassengerHeader.Status = newStatus;
                                    _context.Update(driverPassengerHeader);
                                    _context.SaveChanges();
                                }
                            }

                            //logs
                            Log _log = new Log();
                                    _log.Process = returnMessage + " Reservation ID No.: " + reference;
                                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                    _log.ProcessedDate = DateTime.Now;
                                    _context.Add(_log);
                                        //end logs

                        }
                    }

                    if (approveCounter <= 0)
                    {
                        throw new System.Exception("There is no reservation to approve.");
                    }


                    returnMessage = _method == 2 ? "Approved" : "Disapproved";
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

            var _DataList = _context.DriverPassengers;

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if (currentCompanyGroupName == "UPDI")
            {
                var currentUserLocationIds = await _context.Employees
                   .Where(e => e.Id == currentUser.EmployeeId)
                   .SelectMany(e => e.EmployeeLocations.Select(el => el.Location))
                   .Select(x => x.Id)
                   .ToListAsync();

                var DataListTemp = _DataList.Where(u => u.Status == 2
                    && _context.Employees
                        .Any(e => e.Id == u.EmployeeId
                               && e.CompanyGroupId == currentCompanyGroupId
                               && e.EmployeeLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                ));

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
                    NatureApplication = "DRIVER/VEHICLE REQUEST",
                    DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                    CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='" + u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
                }).Distinct();
                //.GroupBy(x => new { x.TransactionId });




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
            else
            {
                var DataListTemp = _DataList.Where(u => u.Status == 2
                && _context.CompanyLists
                    .Where(c => c.Id == u.CompanyListId)
                    .Select(c => c.CompanyGroupId)
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
                    NatureApplication = "DRIVER/VEHICLE REQUEST",
                    DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                    CheckBox = "<div class='checkbox checkbox-success'><input id='checkselect" + u.TransactionId + "' value='" + u.TransactionId + "' type='checkbox' class='checkBoxGroup'><label for='checkselect" + u.TransactionId + "'></label></div>"
                }).Distinct();
                //.GroupBy(x => new { x.TransactionId });




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

            var _DataList = _context.DriverPassengers;

            var DataListTemp = _DataList.Where(u => u.Status == 2 && u.TransactionId == Request.Form["TransactionId"]);

            if (roles != 1)
            {
                DataListTemp = _DataList.Where(u => u.Status == 2 && u.InitialApproverEmployeeNo == approverData.EmployeeNo && u.TransactionId == Request.Form["TransactionId"]);
            }

            var DataList = DataListTemp.Select(u => new
            {
                Id = u.Id,
                TransactionId = u.TransactionId,
                NatureApplication = "DRIVER/VEHICLE REQUEST",
                DateFiled = u.SubmitDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                TripDate = u.ServiceDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                FullName = u.FirstName + " " + (u.MiddleName == null || u.MiddleName == "" ? "" : u.MiddleName[0] + ". ") + u.LastName,
                RequestedBy = u.Employee.FirstName + " " + (u.Employee.MiddleName == null || u.Employee.MiddleName == "" ? "" : u.Employee.MiddleName[0] + ". ") + u.Employee.LastName,
                CheckBox = "<div class='checkbox checkbox-success'><input id='checkselectIndividual" + u.Id + "' value='" + u.Id + "' type='checkbox' class='checkBoxIndividual'><label for='checkselectIndividual" + u.Id + "'></label></div>"
            });
            //.GroupBy(x => new { x.TransactionId });



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
        public async Task<IActionResult> cancelByGroup()
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var approverData = _context.Employees.FirstOrDefault(e => e.Id.ToString().Equals(HttpContext.Session.GetString("Session_employeeId")));
            var VarAlternateImmediate = approverData.AlternateImmediateHeadValidity == true ? approverData.AlternateImmediateHead.ToString() : approverData.EmployeeNo.ToString();
            int approveCounter = 0;
            returnMessage = "Cancelled";

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
                        if (reference != "")
                        {
                            var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 2 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                            if (driverPassenger.Count() > 0)
                            {
                                //await driverPassenger.ForEachAsync(e => { //JPT commented code 09062024
                                foreach (var e in driverPassenger){ //JPT additional code 09062024
                                    e.Status = 5;
                                    e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                    e.CancelReason = Request.Form["CancelReason"].ToString();
                                    e.CancelledDatetime = DateTime.Now;

                                    // recalculate the trip capacity
                                    if (e.ShuttleId > 0)
                                    {
                                        var _trip = _context.Trip.FirstOrDefault(t => t.Id.Equals(e.ShuttleId));
                                        int capacityAM = _trip.RemainingCapacity;
                                        int capacityPM = _trip.RemainingCapacityPM;
                                        capacityAM++;
                                        capacityPM++;
                                        _trip.RemainingCapacity = capacityAM;
                                        _trip.RemainingCapacityPM = capacityPM;
                                        _context.Update(_trip);
                                        _context.SaveChanges();
                                    }
                                    // end

                                    e.ShuttleId = 0;

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

                                    string _message = "Your request with THSR ID " + e.TransactionId + " has been declined/cancelled." + Environment.NewLine + "Remarks: " + Request.Form["CancelReason"].ToString();


                                    //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                                    //JPT additional code 09052024
                                    string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                                    string message = _message;
                                    string referenceNo = e.TransactionId;  // Replace with your transaction ID
                                    string systemName = "THS Makati";

                                    // Call the SendSmsAsync method
                                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                                    //JPT end additional code 09052024

                                    //new Email notif EBE - ECQ - 05072020

                                    var _Email = "";
                                    if (e.PassengerTypeId == 1)
                                    {
                                        _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                    }

                                    if (_Email != "")
                                    {
                                        var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                        new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                    }
                                    //end new email Notif EBE - ECQ - 05072020

                                    //end EBE 05122020 ECQ

                                } //JPT additional code 09062024
                                //}); //JPT commented code 09062024

                                await _context.SaveChangesAsync();
                                approveCounter++;
                            }

                            //logs
                            Log _log = new Log();
                            _log.Process = returnMessage + " Reservation Trans No.: " + reference;
                            _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                            _log.ProcessedDate = DateTime.Now;
                            _context.Add(_log);
                            //end logs

                            if (approveCounter > 0)
                            {
                                var driverPassengerCheckerAll = _context.DriverPassengers.Where(s => s.TransactionId == reference ).Count();

                                var driverPassengerChecker = _context.DriverPassengers.Where(s => s.TransactionId == reference && s.Status == 5).Count();

                                if (driverPassengerChecker == driverPassengerCheckerAll)
                                {
                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId == reference);
                                    driverPassengerHeader.Status = 5;
                                    _context.Update(driverPassengerHeader);
                                    _context.SaveChanges();
                                }
                            }
                        }
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
            var returnMessage = "Cancelled";
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

                        if (reference != "")
                        {
                            var driverPassenger = _context.DriverPassengers.Where(s => s.Id.ToString() == reference && s.Status == 2 && (s.InitialApproverEmployeeNo.Equals(VarAlternateImmediate) || AllowApproval == 1));

                            if (driverPassenger.Count() > 0)
                            {
                                //await driverPassenger.ForEachAsync(e => { //JPT commented code 09062024
                                foreach (var e in driverPassenger){ //JPT additional code 09062024
                                    e.Status = 5;
                                    e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                    e.CancelReason = Request.Form["CancelReason"].ToString();
                                    e.CancelledDatetime = DateTime.Now;

                                    // recalculate the trip capacity
                                    if (e.ShuttleId > 0)
                                    {
                                        var _trip = _context.Trip.FirstOrDefault(t => t.Id.Equals(e.ShuttleId));
                                        int capacityAM = _trip.RemainingCapacity;
                                        int capacityPM = _trip.RemainingCapacityPM;
                                        capacityAM++;
                                        capacityPM++;
                                        _trip.RemainingCapacity = capacityAM;
                                        _trip.RemainingCapacityPM = capacityPM;
                                        _context.Update(_trip);
                                        _context.SaveChanges();
                                    }
                                    // end

                                    e.ShuttleId = 0;


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

                                    string _message = "Your request with THSR ID " + e.TransactionId + " has been declined/cancelled." + Environment.NewLine + "Remarks: " + Request.Form["CancelReason"].ToString();


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

                                        new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                    }
                                    //end new email Notif EBE - ECQ - 05072020

                                    //end EBE 05122020 ECQ
                                } //JPT additional code 09062024
                                //}); //JPT commented code 09062024
                                await _context.SaveChangesAsync();
                                approveCounter++;
                            }

                            if (approveCounter > 0)
                            {
                                var driverPassenger1 = _context.DriverPassengers.FirstOrDefault(s => s.Id.ToString() == reference);

                                var driverPassengerCheckerAll = _context.DriverPassengers.Where(s => s.TransactionId == driverPassenger1.TransactionId).Count();

                                var driverPassengerChecker = _context.DriverPassengers.Where(s => s.TransactionId == driverPassenger1.TransactionId && s.Status == 5).Count();

                                if (driverPassengerChecker == driverPassengerCheckerAll)
                                {
                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId.Equals(driverPassenger1.TransactionId));
                                    driverPassengerHeader.Status = 5;
                                    _context.Update(driverPassengerHeader);
                                    _context.SaveChanges();
                                }
                            }

                            //logs
                            Log _log = new Log();
                            _log.Process = returnMessage + " Reservation ID No.: " + reference;
                            _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                            _log.ProcessedDate = DateTime.Now;
                            _context.Add(_log);
                            //end logs

                        }
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

                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(id));
                    if (driverPassengerHeader == null)
                    {
                        throw new System.Exception("Data not found.");
                    }


                    driverPassengerHeader.Status = NewStatus;


                    _context.Update(driverPassengerHeader);
                    await _context.SaveChangesAsync();

                    

                        var driverPassenger = _context.DriverPassengers.Where(s => s.DriverPassengerHeaderId == (driverPassengerHeader.Id.ToString()) && s.Status <= 2);
                    
                        if (driverPassenger.Count() > 0)
                        {
                            //await driverPassenger.ForEachAsync(e => //JPT commented code 09062024
                            foreach (var e in driverPassenger) //JPT additional code 09062024
                            {
                                e.ShuttleId = 0;
                                e.Status = NewStatus;
                                if (NewStatus == 5) {// cancelled
                                    e.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                    e.CancelReason = Request.Form["CancelDeclineReason"].ToString();
                                    e.CancelledDatetime = DateTime.Now;
                                } else if (NewStatus == 7){// declined
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

                                string _message = "Your trip request with THSR ID " + e.TransactionId + " going to "+ e.Destination +" has been "+ smsTitle + " by AGS." + Environment.NewLine + "Remarks: " + Request.Form["CancelDeclineReason"].ToString();


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

                                    new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                }
                                //end new email Notif EBE - ECQ - 05072020

                                //end EBE 05122020 ECQ
                            } //JPT additional code 09062024
                            //}); //JPT commented code 09062024
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



        [HttpPost]
        public async Task<IActionResult> CloseTicket(int id)
        {
            //cancel = 0

            int returnSuccess = 1;
            string returnMessage = "";
            string smsTitle = "Closed";
            int NewStatus = 4;//closed
            string surveyLink = "";
            string hashCode = "";
            


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var driverPassengerHeader = _context.DriverPassengerHeaders.Include(e=> e.Employee).FirstOrDefault(e => e.Id.Equals(id));
                    if (driverPassengerHeader == null)
                    {
                        throw new System.Exception("Data not found.");
                    }

                    if(_context.SurveyTransactions.FirstOrDefault(e => e.TransactionId.Equals(driverPassengerHeader.TransactionId)) != null)
                    {
                        throw new System.Exception("Error on generating Survey.");
                    }

                    string _Email = driverPassengerHeader.Employee.CompanyEmail;
                    hashCode = new ReportsController().stringHasher(driverPassengerHeader.TransactionId);
                    surveyLink = new ReportsController().getLink() +"?u=" + hashCode;

                    driverPassengerHeader.Status = NewStatus;
                    _context.Update(driverPassengerHeader);

                    //insert survey transaction
                    var survey = new SurveyTransaction();
                    survey.EmployeeId = driverPassengerHeader.EmployeeId;
                    survey.TransactionId = driverPassengerHeader.TransactionId;
                    survey.SurveyHash = hashCode;
                    survey.GeneratedDateTime = DateTime.Now;
                    survey.IsAnswered = 0;
                    _context.Add(survey);
                    // end

                    string _message = "Your trip request with THSR ID " + driverPassengerHeader.TransactionId + " going to " + driverPassengerHeader.Destination + " has been " + smsTitle +
                                 " by AGS. Please answer the satisfaction survey by opening the Transport hub or by Clicking the link below: ";

                    //JPT commented code 07012025 link is invalid in SMART SMS
                    //+ Environment.NewLine +
                    //surveyLink;
                    //JPT end commented code 07012025 link is invalid in SMART SMS

                    if (_Email != "")
                    {
                        var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;
                        new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation - Closed");
                    }
                    //new ReportsController().SendSMS(_message, driverPassengerHeader.Employee.MobileNumber.ToString()); //JPT commented code 09042024

                    //JPT additional code 09062024
                    string mobileNumbers = driverPassengerHeader.Employee.MobileNumber.ToString();  // Replace with actual mobile numbers
                    string message = _message;
                    string referenceNo = driverPassengerHeader.TransactionId;  // Replace with your transaction ID
                    string systemName = "THS Makati";

                    // Call the SendSmsAsync method
                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                    //JPT end additional code 09062024

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



    }
}
