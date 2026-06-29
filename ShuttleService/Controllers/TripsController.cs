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
using ReportService;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAllRole")]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // database connection for user management only

        public TripsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; //for user management only
        }

        // GET: Trips
        [Authorize(Policy = "RequireAdminAGSRole")]
        public async Task<IActionResult> Index()
        {
            //checking if has session
            if (HttpContext.Session.GetString("Session_employeeId") == null || HttpContext.Session.GetString("Session_employeeId") == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
            //end
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            _context.Database.ExecuteSqlCommand("execute UpdateApprovedDriverPassengerHeaders");
            ViewBag.CompanyGroup = currentCompanyGroupName;

            var applicationDbContext = _context.Trip.Include(t => t.Driver).Include(t => t.ReservationType).Include(t => t.VehicleList);
            return View(await applicationDbContext.ToListAsync());
        }

        //// GET: Trips/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var trip = await _context.Trip
        //        .Include(t => t.Driver)
        //        .Include(t => t.ReservationType)
        //        .Include(t => t.VehicleList)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (trip == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(trip);
        //}

        [Authorize(Policy = "RequireAdminAGSRole")]
        public async Task<IActionResult> Details()
        {

            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            ViewBag.unixtime = unixTime * 1000;
            ViewBag.unixtimeFormatted = unixTime;



            String today = DateTime.Today.ToString();

            //ViewBag.invalidDate = 0; 
            //if (ViewBag.dtimestamp < today) {
            //    ViewBag.invalidDate = 1;
            //}


            int[] NonRejectedCancelledStatus = { 1, 2, 4 };
            ViewBag.AMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 2) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.PMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 3) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.AssemblyArea = "In Front of DMCI Annex Building";


            //ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName");
            ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description");
            //ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model");

            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            ViewBag.OverAllApprover = roles;
            

            //end checking the role if overall approver


            var _smslog = _context.SMSLogs.LastOrDefault(t => t.ServiceDateTimeStamp.Equals(unixTime));
            ViewBag.SMSLog = -1;
            if (_smslog != null)
            {
                ViewBag.SMSLog = _smslog.Version;
            }

            return View();
        }

        public ActionResult DynamicContent(string d)
        {
            return PartialView(String.Format("shuttlepassengers/details?t=sr&d=" + d));
        }

        [Authorize(Policy = "RequireAdminAGSRole")]
        public async Task<IActionResult> AssignTrip()
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
            ViewBag.unixtimeFormatted = unixTime;

            ViewBag.dGet = unixTime;
            ViewBag.tGet = Convert.ToInt64(HttpContext.Request.Query["t"]);
            ViewBag.tempid = HttpContext.Request.Query["tempid"];



            String today = DateTime.Today.ToString();

            //ViewBag.invalidDate = 0; 
            //if (ViewBag.dtimestamp < today) {
            //    ViewBag.invalidDate = 1;
            //}

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            int[] NonRejectedCancelledStatus = { 1, 2, 4 };
            ViewBag.AMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 2) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.PMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 3) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.AssemblyArea = "In Front of DMCI Annex Building";


            //ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName");
            ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description");
            //ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model");

            var _t = Convert.ToInt64(HttpContext.Request.Query["t"]);
            var _driverPass = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(Convert.ToInt32(_t)));
            ViewData["DriverPassengerHeaders"] = _driverPass;

           
            if (_driverPass.ShuttleId > 0)
            {
                var _trip = await _context.Trip.FirstOrDefaultAsync(e => e.Id.Equals(Convert.ToInt32(_driverPass.ShuttleId)));
                ViewData["Trip"] = _trip;
                                                                    
                var _veh = await _context.VehicleLists.FirstOrDefaultAsync(e => e.Id.Equals(Convert.ToInt32(_trip.VehicleListId)));
                // ViewBag.ModelPlate = _veh.Model + " / " + _veh.PlateNumber;
                //ViewBag.ModelPlate = (_veh.CodingDay == (int)_trip.ServiceStartDate.Date.DayOfWeek) ? _veh.Model + " / " + _veh.PlateNumber + " <b style='color:red'>(Coding)</b>" : _veh.Model + " / " + _veh.PlateNumber;
                ViewBag.ModelPlate = (_veh.CodingDay == (int)_trip.ServiceStartDate.Date.DayOfWeek) ? _veh.Model + " / " + _veh.PlateNumber + " (Coding)" : _veh.Model + " / " + _veh.PlateNumber;


                var _drive = await _context.Drivers.FirstOrDefaultAsync(e => e.Id.Equals(Convert.ToInt32(_trip.DriverId)));
                ViewBag.Driver = _drive.FirstName + " " + _drive.LastName ;
            }
            else {
                ViewData["Trip"] = new Trip();
                ViewBag.ModelPlate = "";
            }

            var employee = _context.Employees.FirstOrDefault(e => e.Id.Equals(_driverPass.EmployeeId));
            ViewBag.RequestedBy = employee.FirstName + " " + employee.LastName;

            var checkallthepassengers = _context.DriverPassengers.Where(e => e.TransactionId.Equals(_driverPass.TransactionId)).Select(e=> e.ChargingCompany.CompanyName).Distinct().ToList().ToArray();

            string ChargeTo = "";
            foreach (string reference in checkallthepassengers)
            {
                ChargeTo = ViewBag.ChargeTo + "," + reference;
            }
            
            ViewBag.ChargeTo = ChargeTo.TrimStart(',');
            ViewBag.ChargeTo = ViewBag.ChargeTo.TrimEnd(',');
            DateTime? serviceDate = _driverPass.ServiceDate;
            ViewBag.TripDate = serviceDate.Value.ToString("MM/dd/yyyy");
            ViewBag.ServiceType = _context.ServiceTypes.FirstOrDefault(e => e.Id.Equals(_driverPass.ServiceTypeId)).Description;

            //await checkallthepassengers.ForEachAsync(e => {

            //});


            //checking the role if overall approver

            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var roleClaimType = userIdentity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role && c.Value.Equals("Overall Approver")).Count();
            ViewBag.OverAllApprover = roles;

            ViewBag.FromTrip = HttpContext.Request.Query["ft"].ToString();

            //end checking the role if overall approver


            return View();
        }

        

        public async Task<IActionResult> DetailsDriver()
        {

            // Example of a UNIX timestamp for 11-29-2013 4:58:30
            long unixTime = Convert.ToInt64(HttpContext.Request.Query["d"]);
            ViewBag.dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            ViewBag.unixtime = unixTime * 1000;
            ViewBag.unixtimeFormatted = unixTime;



            String today = DateTime.Today.ToString();

            //ViewBag.invalidDate = 0; 
            //if (ViewBag.dtimestamp < today) {
            //    ViewBag.invalidDate = 1;
            //}


            int[] NonRejectedCancelledStatus = { 1, 2, 4 };
            ViewBag.AMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 2) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.PMPassengerCount = await _context.ShuttlePassengers.CountAsync(s => s.ServiceDateTimeStamp == unixTime && (s.TripTypeId == 1 || s.TripTypeId == 3) && NonRejectedCancelledStatus.Contains(s.Status));
            ViewBag.AssemblyArea = "In Front of DMCI Annex Building";


            //ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName");
            ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description");
            //ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model");

            ViewData["ReservationTypeIdList"] = _context.ReservationType.ToList();


            var _smslog = _context.SMSLogs.LastOrDefault(t => t.ServiceDateTimeStamp.Equals(unixTime));
            ViewBag.SMSLog = -1;
            if (_smslog != null)
            {
                ViewBag.SMSLog = _smslog.Version;
            }




            return View();
        }

        

        // GET: Trips/Create
        public IActionResult Create()
        {



            //ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName");
            //ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description");
            //ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ServiceDateTimeStamp,VehicleListId,Capacity,DriverId,ReservationTypeId,Remarks")] Trip trip)
        {

            var returnSuccess = 1;
            var returnMessage = "";
            var _transactionId = "";
            string SMSStatus = "";

            string emailRecipient = "";//new code for optimized text
            string transactionId = "";//new code for optimized text

            //Manage Parameters
            var parameterTemp = _context.Parameters.FirstOrDefault(e => e.Id == 3);
            if (parameterTemp.Year < DateTime.Now.Year)
            {
                parameterTemp.Value = 1;
                parameterTemp.Year = DateTime.Now.Year;
                _context.Update(parameterTemp);
                _context.SaveChanges();
            }


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    double unixTime = double.Parse(Request.Form["ServiceDateTimeStamp"]);

                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();

                

                    string[] explodedStartTime = Request.Form["ServiceStartDate"].ToString().Split(':');
                    string[] explodedEndTime = Request.Form["ServiceEndDate"].ToString().Split(':');
                    TimeSpan starttime = new TimeSpan(Int32.Parse(explodedStartTime[0]), Int32.Parse(explodedStartTime[1]), 0);
                    trip.ServiceStartDate = dtDateTime.Date + starttime;

                    TimeSpan endtime = new TimeSpan(Int32.Parse(explodedEndTime[0]), Int32.Parse(explodedEndTime[1]), 0);
                    trip.ServiceEndDate = dtDateTime.Date + endtime;

                    //if (dtDateTime < DateTime.Now)
                    //{
                    //    throw new System.Exception("Invalid reservation date.");
                    //}

                    //trip.EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName");

                    var currentUser = await _userManager.GetUserAsync(User);
                    trip.EncodedBy = User.Identity.Name;

                    trip.EncodeDate = DateTime.Now;
                    trip.ModifyDate = DateTime.Now;
                    trip.Status = 1;
                    trip.RemainingCapacity = trip.Capacity;
                    trip.RemainingCapacityPM = trip.Capacity;

                    //if (trip.ReservationTypeId == 1)
                    //{
                    //    int tripId = Convert.ToInt32(Request.Form["tripId"]);
                    //    int method = Convert.ToInt32(Request.Form["method"]);

                    //    if (method == 2)
                    //    {
                    //        var _trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(tripId));
                    //        int countAM = _context.ShuttlePassengers.Count(e => e.ShuttleId.Equals(_trip.Id) && (e.TripType.Equals(1) || e.TripType.Equals(2)));
                    //        int countPM = _context.ShuttlePassengers.Count(e => e.ShuttleId.Equals(_trip.Id) && (e.TripType.Equals(1) || e.TripType.Equals(3)));
                    //        _trip.ServiceStartDate = trip.ServiceStartDate;
                    //        _trip.ServiceEndDate = trip.ServiceEndDate;
                    //        _trip.EncodedBy = trip.EncodedBy;
                    //        _trip.EncodeDate = trip.EncodeDate;
                    //        _trip.ModifyDate = trip.ModifyDate;
                    //        _trip.Status = 1;
                    //        _trip.RemainingCapacity = trip.Capacity - countAM;
                    //        _trip.RemainingCapacityPM = trip.Capacity - countPM;
                    //        _trip.ServiceDateTimeStamp = trip.ServiceDateTimeStamp;
                    //        _trip.VehicleListId = trip.VehicleListId;
                    //        _trip.Capacity = trip.Capacity;
                    //        _trip.DriverId = trip.DriverId;
                    //        _trip.ReservationTypeId = trip.ReservationTypeId;
                    //        _trip.Remarks = trip.Remarks;
                    //        if (_trip.RemainingCapacity < 0 || _trip.RemainingCapacityPM < 0)
                    //        {
                    //            throw new System.Exception("Invalid Capacity.");
                    //        }

                    //        _context.Update(_trip);
                    //    }
                    //    else {
                    //        var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 3);
                    //        _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                    //        parameter.Value = parameter.Value + 1;
                    //        _context.Update(parameter);
                    //        _context.SaveChanges();

                    //        trip.TripControlNo = _transactionId;
                    //        _context.Add(trip);
                    //    }
                    //}
                    //else
                    //{
                        var parameter = _context.Parameters.FirstOrDefault(e => e.Id == 3);
                        _transactionId = parameter.Code + "-" + parameter.Year + "-" + parameter.Value.ToString("D6");
                        parameter.Value = parameter.Value + 1;
                        _context.Update(parameter);
                        _context.SaveChanges();

                        trip.TripControlNo = _transactionId;
                        _context.Add(trip);
                    //}

                   
                  

                  
                    await _context.SaveChangesAsync();

                    if (trip.ReservationTypeId == 2)
                    {

                        int headerId = Int32.Parse(Request.Form["t"]);

                        var DriverHeader = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(headerId));
                        DriverHeader.ShuttleId = trip.Id;
                        DriverHeader.Status = 9;
                        _context.Update(DriverHeader);

                        var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == DriverHeader.TransactionId);


                        //composing of message
                        string _message = "";
                        string _MobileNumber = "";
                        string _destination = "";
                        string _tripdate = "";
                        string _triptimefrom = "";
                        string _triptimeto = "";
                        string _driver = "";
                        string _drivercontact = "";
                        string _vehicle = "";
                        string tempMobileNumber = "";
                        var getTrip = _context.Trip.FirstOrDefault(t => t.Id.Equals(trip.Id));

                        _destination = DriverHeader.Origin + " to " + DriverHeader.Destination;
                        _tripdate = getTrip.ServiceStartDate.ToString("MMM dd,yyyy");
                        _triptimefrom = getTrip.ServiceStartDate.ToString("hh:mm tt");
                        _triptimeto = getTrip.ServiceEndDate.ToString("hh:mm tt");


                        var getDriver = _context.Drivers.FirstOrDefault(t => t.Id.Equals(getTrip.DriverId));
                        _driver = getDriver.FirstName + " " + getDriver.LastName;
                        _drivercontact = getDriver.MobileNumber;

                        var getVehicle = _context.VehicleLists.FirstOrDefault(t => t.Id.Equals(getTrip.VehicleListId));
                        _vehicle = getVehicle.Model + " " + getVehicle.PlateNumber;

                        //end composing

                        // new 06092020

                        string _AdditionalMobileNumber = "";
                        //*******FOR DRIVERS
                        var drivers = _context.Drivers.Where(d => d.Status.Equals(1) && d.Id.Equals(getDriver.Id)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                        foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                        {
                            if (pnumbers != "")
                            {
                                _AdditionalMobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                            }
                        }
                        //*******END FOR DRIVERS

                        ////*******FOR OVERALL APPROVERS
                        //var users = _userManager.GetUsersInRoleAsync("Overall Approver").Result.ToList().ToArray();

                        //foreach (int _user in users.Select(u => u.EmployeeId))
                        //{
                        //    var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_user)).MobileNumber;
                        //    if (_userMobile != "")
                        //    {
                        //        _AdditionalMobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                        //    }
                        //}
                        ////*******FOR OVERALL APPROVERS

                        //*******FOR AGS Personnel
                        var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                        foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                        {
                            var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_userAGS)).MobileNumber;
                            if (_userMobile != "")
                            {
                                _AdditionalMobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                            }
                        }
                        //*******FOR AGS Personnel .


                        // end 06092020


                        if (driverPassenger.Count() > 0)
                        {
                            await driverPassenger.ForEachAsync(e =>
                            {
                                e.ShuttleId = trip.Id;

                                //message

                                _message = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    var getEmp = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo));
                                    if (getEmp != null)
                                    {
                                        //_MobileNumber = getEmp.MobileNumber;
                                        tempMobileNumber = tempMobileNumber + getEmp.MobileNumber + ",";
                                        //new code for optimized text
                                        if (IsValidEmail(getEmp.CompanyEmail))
                                        {
                                            emailRecipient = emailRecipient + getEmp.CompanyEmail + ",";
                                        }
                                        transactionId = e.TransactionId;
                                        //end new code for optimized text
                                    }
                                    //_MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else
                                {
                                    //_MobileNumber = e.ContactNo;
                                    tempMobileNumber = tempMobileNumber + e.ContactNo + ",";
                                }

                                //_MobileNumber += "," + _AdditionalMobileNumber;

                                if (e.SMSRevision > 0)
                                {
                                    _message += "Revision " + e.SMSRevision + " on ";
                                }

                                /*
                                _message += "THSR ID: " + e.TransactionId + Environment.NewLine +
                                            "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                            _driver + Environment.NewLine +
                                            "CP no. " + _drivercontact + Environment.NewLine +
                                            "Vehicle : " + _vehicle + ".";

                                string sendSMS = new ReportsController().SendSMS(_message, _MobileNumber);

                                if (sendSMS != "200")
                                {
                                    SMSStatus = " but has error on sending SMS.";
                                    //throw new System.Exception("Error on sending SMS.");
                                }


                                if (e.PassengerTypeId == 1)
                                {
                                    var sendemail = new ReportsController().SendEmail("SHUTTLE/DRIVER SERVICE RESERVATION", _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail, "", _message, "Driver/Vehicle Reservation");

                                }
                                */

                                //end message

                                e.SMSRevision = e.SMSRevision + 1;

                            });
                            await _context.SaveChangesAsync();
                        }

                        //NEW CODE EBELAYDA TO OPTIMIZED SENDING OF MESSAGE 10/20/2022

                        _MobileNumber = tempMobileNumber;
                        _message += "THSR ID: " + transactionId + Environment.NewLine +
                                    "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                    _driver + Environment.NewLine +
                                    "CP no. " + _drivercontact + Environment.NewLine +
                                    "Vehicle : " + _vehicle + ".";



                        //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//

                        //string sendSMS = new ReportsController().SendSMS(_message, _MobileNumber);
                        //if (sendSMS != "200")
                        //{
                        //    SMSStatus = " but has error on sending SMS.";
                        //}

                        //JPT commented code 09052024
                        //string[] mobileArr = _MobileNumber.Split(',');
                        //int xIndex = 1;
                        //foreach (string mobileNum in mobileArr)
                        //{
                        //    await new ReportsController().CallSMSAPI(mobileNum, _message, "TRIP");
                        //    xIndex++;
                        //}
                        //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//

                        //JPT additional code 09052024
                        string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                        string message = _message;
                        string referenceNo = transactionId;  // Replace with your transaction ID
                        string systemName = "THS Makati";

                        // Call the SendSmsAsync method
                        string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                        SMSStatus = smsStatus + ". ";
                        //JPT end additional code 09052024


                        string sendemail = new ReportsController().SendEmail("SHUTTLE/DRIVER SERVICE RESERVATION", emailRecipient, "", _message, "Driver/Vehicle Reservation");
                        if (sendemail != "1")
                            //SMSStatus = SMSStatus + " and has error on sending Email."; //JPT commented code 09062024
                            SMSStatus += "Email not sent due to bad connection."; //JPT additional code 09062024




                        //end message

                        //NEW CODE TO OPTIMIZED SENDING OF MESSAGE 10/20/2022

                        await _context.SaveChangesAsync();

                    }




                    returnMessage = "Success " + SMSStatus;
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message + "/" + e.InnerException + " / SMS: "+ SMSStatus;
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

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName", trip.DriverId);
            ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description", trip.ReservationTypeId);
            ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model", trip.VehicleListId);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,TripControlNo,ServiceDateTimeStamp,ServiceStartDate,ServiceEndDate,VehicleListId,Capacity,RemainingCapacity,RemainingCapacityPM,DriverId,Status,ReservationTypeId,Remarks,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] Trip trip)
        //{
        //    if (id != trip.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(trip);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TripExists(trip.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["DriverId"] = new SelectList(_context.Drivers, "Id", "FirstName", trip.DriverId);
        //    ViewData["ReservationTypeId"] = new SelectList(_context.ReservationType, "Id", "Description", trip.ReservationTypeId);
        //    ViewData["VehicleListId"] = new SelectList(_context.VehicleLists, "Id", "Model", trip.VehicleListId);
        //    return View(trip);
        //}

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceDateTimeStamp,VehicleListId,Capacity,DriverId,ReservationTypeId,Remarks")] Trip _trip)
        {
            
            var returnSuccess = 1;
            var returnMessage = "";
            string SMSStatus = "";


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(id));
                    if (trip == null)
                    {
                        throw new System.Exception("Trip data not found.");
                    }
                    trip.ServiceDateTimeStamp = _trip.ServiceDateTimeStamp;
                    trip.VehicleListId = _trip.VehicleListId;
                    trip.Capacity = _trip.Capacity;
                    trip.DriverId = _trip.DriverId;
                    trip.ReservationTypeId = _trip.ReservationTypeId;
                    trip.Remarks = _trip.Remarks;

                    double unixTime = double.Parse(Request.Form["ServiceDateTimeStamp"]);

                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();

                    string[] explodedStartTime = Request.Form["ServiceStartDate"].ToString().Split(':');
                    string[] explodedEndTime = Request.Form["ServiceEndDate"].ToString().Split(':');
                    TimeSpan starttime = new TimeSpan(Int32.Parse(explodedStartTime[0]), Int32.Parse(explodedStartTime[1]), 0);
                    trip.ServiceStartDate = dtDateTime.Date + starttime;

                    TimeSpan endtime = new TimeSpan(Int32.Parse(explodedEndTime[0]), Int32.Parse(explodedEndTime[1]), 0);
                    trip.ServiceEndDate = dtDateTime.Date + endtime;

                    //if (dtDateTime < DateTime.Now)
                    //{
                    //    throw new System.Exception("Invalid reservation date.");
                    //}

                    //trip.EncodedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    var currentUser = await _userManager.GetUserAsync(User);
                    trip.EncodedBy = User.Identity.Name;

                    trip.EncodeDate = DateTime.Now;
                    trip.ModifyDate = DateTime.Now;
                    trip.Status = 1;
                    trip.EmailStatus = 0;
                    //recounting the capacity

                    int countAM = _context.ShuttlePassengers.Count(e => e.ShuttleId.Equals(id) && (e.TripTypeId.Equals(1) || e.TripTypeId.Equals(2)));
                    int countPM = _context.ShuttlePassengers.Count(e => e.ShuttleId.Equals(id) && (e.TripTypeId.Equals(1) || e.TripTypeId.Equals(3)));

                    //recounting the capacity
                    trip.RemainingCapacity = trip.Capacity - countAM;
                    trip.RemainingCapacityPM = trip.Capacity - countPM;

                    if (trip.RemainingCapacity < 0 || trip.RemainingCapacityPM < 0)
                    {
                        throw new System.Exception("Invalid Capacity.");
                    }

                    _context.Update(trip);
                    await _context.SaveChangesAsync();

                    if (trip.ReservationTypeId == 2)
                    {

                        int headerId = Int32.Parse(Request.Form["t"]);

                        var DriverHeader = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(headerId));
                        DriverHeader.ShuttleId = trip.Id;
                        DriverHeader.Status = 9;
                        _context.Update(DriverHeader);



                        //composing of message
                        string _message = "";
                        string _MobileNumber = "";
                        string _destination = "";
                        string _tripdate = "";
                        string _triptimefrom = "";
                        string _triptimeto = "";
                        string _driver = "";
                        string _drivercontact = "";
                        string _vehicle = "";
                        var getTrip = _context.Trip.FirstOrDefault(t => t.Id.Equals(trip.Id));

                        _destination = DriverHeader.Origin + " to " + DriverHeader.Destination;
                        _tripdate = getTrip.ServiceStartDate.ToString("MMM dd,yyyy");
                        _triptimefrom = getTrip.ServiceStartDate.ToString("hh:mm tt");
                        _triptimeto = getTrip.ServiceEndDate.ToString("hh:mm tt");


                        var getDriver = _context.Drivers.FirstOrDefault(t => t.Id.Equals(getTrip.DriverId));
                        _driver = getDriver.FirstName + " " + getDriver.LastName;
                        _drivercontact = getDriver.MobileNumber;

                        var getVehicle = _context.VehicleLists.FirstOrDefault(t => t.Id.Equals(getTrip.VehicleListId));
                        _vehicle = getVehicle.Model + " " + getVehicle.PlateNumber;

                        //end composing


                        string emailRecipient = "";//new code for optimized text
                        string transactionId = "";//new code for optimized text
                        string tempMobileNumber = "";//new code for optimized text

                        var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == DriverHeader.TransactionId);

                        if (driverPassenger.Count() > 0)
                        {
                            await driverPassenger.ForEachAsync(e => {
                                e.ShuttleId = trip.Id;
                                _message = "";
                                //message
                                if (e.PassengerTypeId == 1)
                                {
                                    var getEmp = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo));
                                    if(getEmp != null) {
                                        //_MobileNumber = getEmp.MobileNumber;
                                        tempMobileNumber = tempMobileNumber + getEmp.MobileNumber + ",";
                                        //new code for optimized text
                                        if (IsValidEmail(getEmp.CompanyEmail)) { 
                                            emailRecipient = emailRecipient + getEmp.CompanyEmail + ",";
                                        }
                                        transactionId = e.TransactionId;
                                        //end new code for optimized text
                                    }
                                }
                                else
                                {
                                    tempMobileNumber = tempMobileNumber + e.ContactNo + ",";
                                    //_MobileNumber = e.ContactNo;
                                }

                                if (e.SMSRevision > 0)
                                {
                                    _message += "Revision " + e.SMSRevision + " on ";
                                }


                                //_MobileNumber = _MobileNumber + ",";

                                //Commented EBELAYDA TO OPTIMIZED SENDING OF MESSAGE 10/20/2022
                                /*
                                    //*******FOR DRIVERS
                                    var drivers = _context.Drivers.Where(d => d.Status.Equals(1) && d.Id.Equals(getDriver.Id)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                                    foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                                    {
                                        if (pnumbers != "")
                                        {
                                            _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                                        }
                                    }
                                    //*******END FOR DRIVERS

                                    ////*******FOR OVERALL APPROVERS
                                    //var users = _userManager.GetUsersInRoleAsync("Overall Approver").Result.ToList().ToArray();

                                    //foreach (int _user in users.Select(u => u.EmployeeId))
                                    //{
                                    //    var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_user)).MobileNumber;
                                    //    if (_userMobile != "")
                                    //    {
                                    //        _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                                    //    }
                                    //}
                                    ////*******FOR OVERALL APPROVERS

                                    //*******FOR AGS Personnel
                                    var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                                    foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                                    {
                                        var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_userAGS)).MobileNumber;
                                        if (_userMobile != "")
                                        {
                                            _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                                        }
                                    }
                                    //*******FOR AGS Personnel .


                                    _message += "THSR ID: " + e.TransactionId + Environment.NewLine +
                                                "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                                _driver + Environment.NewLine +
                                                "CP no. " + _drivercontact + Environment.NewLine +
                                                "Vehicle : " + _vehicle + ".";

                                     string sendSMS = new ReportsController().SendSMS(_message, _MobileNumber);
                                   // string sendSMS = "200";
                                    if (sendSMS != "200")
                                    {
                                        SMSStatus = " but has error on sending SMS.";
                                        //throw new System.Exception("Error on sending SMS.");
                                    }


                                    if (e.PassengerTypeId == 1)
                                    {
                                        var sendemail = new ReportsController().SendEmail("SHUTTLE/DRIVER SERVICE RESERVATION", _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail, "", _message, "Driver/Vehicle Reservation");

                                    }


                                    //end message
                                */
                                //Commented EBELAYDA  TO OPTIMIZED SENDING OF MESSAGE 10/20/2022


                                e.SMSRevision = e.SMSRevision + 1;
                            });
                            await _context.SaveChangesAsync();
                        }



                        await _context.SaveChangesAsync();

                        //NEW CODE EBELAYDA TO OPTIMIZED SENDING OF MESSAGE 10/20/2022
                            _MobileNumber = tempMobileNumber;
                            
                            //*******FOR DRIVERS
                            var drivers = _context.Drivers.Where(d => d.Status.Equals(1) && d.Id.Equals(getDriver.Id)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                            foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                            {
                                if (pnumbers != "")
                                {
                                    _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                                }
                            }
                            //*******END FOR DRIVERS

                            //*******FOR AGS Personnel
                            var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                            foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                            {
                                var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_userAGS)).MobileNumber;
                                if (_userMobile != "")
                                {
                                    _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                                }
                            }
                            //*******FOR AGS Personnel .


                            _message += "THSR ID: " + transactionId + Environment.NewLine +
                                        "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                        _driver + Environment.NewLine +
                                        "CP no. " + _drivercontact + Environment.NewLine +
                                        "Vehicle : " + _vehicle + ".";



                        //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//

                        //string sendSMS = new ReportsController().SendSMS(_message, _MobileNumber);
                        //if (sendSMS != "200")
                        //{
                        //    SMSStatus = " but has error on sending SMS.";
                        //}

                        //JPT commented code 09052024
                        //string[] mobileArr = _MobileNumber.Split(',');
                        //int xIndex = 1;
                        //foreach (string mobileNum in mobileArr)
                        //{
                        //    await new ReportsController().CallSMSAPI(mobileNum, _message, "TRIP");
                        //    xIndex++;
                        //}
                        //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//

                        //JPT additional code 09052024
                        string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                        string message = _message;
                        string referenceNo = transactionId;  // Replace with your transaction ID
                        string systemName = "THS Makati";

                        // Call the SendSmsAsync method
                        string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                        SMSStatus = smsStatus + ". ";
                        //JPT end additional code 09052024

                        string sendemail = new ReportsController().SendEmail("SHUTTLE/DRIVER SERVICE RESERVATION", emailRecipient, "", _message, "Driver/Vehicle Reservation");
                                if(sendemail != "1")
                                //SMSStatus = SMSStatus + " and has error on sending Email."; //JPT commented code 09062024
                                SMSStatus += "Email not sent due to bad connection."; //JPT additional code 09062024

                       


                        //end message

                        //NEW CODE TO OPTIMIZED SENDING OF MESSAGE 10/20/2022

                    }




                    returnMessage = "Success " + SMSStatus;
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


        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip
                .Include(t => t.Driver)
                .Include(t => t.ReservationType)
                .Include(t => t.VehicleList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trip.FindAsync(id);
            _context.Trip.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trip.Any(e => e.Id == id);
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

                var TripList = _context.Trip
                               .Where(r => r.ServiceStartDate >= lastMonth
                                   && r.ServiceStartDate <= nextMonth
                                   && r.Status > 0
                                   && _context.Drivers
                                       .Any(e => e.Id == r.DriverId
                                          && e.CompanyGroupId == currentCompanyGroupId
                                          && e.DriverLocations.Any(el => currentUserLocationIds.Contains(el.LocationId))
                               ))
                               .Select(u => new
                               {
                                   Title = u.VehicleList.Model + " - " + u.VehicleList.PlateNumber,
                                   Start = u.ServiceStartDate,
                                   Fname = u.VehicleList.Model,
                                   Lname = u.VehicleList.PlateNumber,
                                   Model = u.VehicleList.Model,
                                   PlateNumber = u.VehicleList.PlateNumber,
                                   TransId = u.TripControlNo,
                               })
                               .GroupBy(x => new { x.TransId });


                var jsonData = new
                {
                    Status = _status,
                    Message = _message,
                    Events = TripList.ToList().ToArray(),
                    DateFilter = oDate
                };

                return new JsonResult(jsonData);
            }
            else
            {
                var TripList = _context.Trip
                                .Where(r => r.ServiceStartDate >= lastMonth
                                    && r.ServiceStartDate <= nextMonth
                                    && r.Status > 0)
                                .Select(u => new
                                {
                                    Title = u.VehicleList.Model + " - " + u.VehicleList.PlateNumber,
                                    Start = u.ServiceStartDate,
                                    Fname = u.VehicleList.Model,
                                    Lname = u.VehicleList.PlateNumber,
                                    Model = u.VehicleList.Model,
                                    PlateNumber = u.VehicleList.PlateNumber,
                                    TransId = u.TripControlNo,
                                    CompanyGroupId = _context.Drivers
                                        .Where(driver => driver.Id == u.DriverId)
                                        .Select(d => d.CompanyGroupId)
                                        .FirstOrDefault()
                                })
                                .Where(x => x.CompanyGroupId == currentCompanyGroupId)
                                .GroupBy(x => new { x.TransId });


                var jsonData = new
                {
                    Status = _status,
                    Message = _message,
                    Events = TripList.ToList().ToArray(),
                    DateFilter = oDate
                };

                return new JsonResult(jsonData);
            }
            
        }


        [HttpPost]
        public async Task<JsonResult> checkifhastrip(long dateFilter)
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];

            string _status = "OK";
            string _message = "";

      

            var _tripCount = await _context.Trip.CountAsync(r => r.ServiceDateTimeStamp == dateFilter );//.GroupBy(x => new { x.Fname, x.Lname, x.Start });





            var jsonData = new
            {
                Status = _status,
                Message = _message,
                TripCount = _tripCount
            };

            return new JsonResult(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> CheckCapacity(int vehicleId)
        {
            var vehicle = await _context.VehicleLists
                .Where(v => v.Id == vehicleId)
                .Select(v => new { v.Capacity })
                .SingleOrDefaultAsync();

            if (vehicle == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Vehicle not found"
                });
            }

            return new JsonResult(new
            {
                success = true,
                vehicleCapacity = vehicle.Capacity
            });
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult ViewTripToday()
        {

            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var hide_ = "";
            if (Convert.ToInt32(Request.Form["method"]) == 1)
            {
                hide_ = "hidden";
            }
            var linkForManifest = Url.Content("~/reports/Manifest");



            var TripList = _context.Trip.Where(r => r.ServiceDateTimeStamp.Equals(Convert.ToInt64(Request.Form["unixTime"].ToString())) && r.ReservationTypeId == 1 && r.Status != 0 )
                                                                .Select(u => new {
                                                                    //Id =    "<div class='btn-group' >" +
                                                                    //        "                <button data-toggle='dropdown' class='btn btn-primary dropdown-toggle'>Action<span class='caret'></span></button>" +
                                                                    //        "                <ul class='dropdown-menu' style='z-index: 9999!important;'>" +
                                                                    //        "                    <li><a href='#' class='font-bold' onClick='OpenThisTrip(`" + u.Id + "`);'>Select</a></li>" +
                                                                    //        "                    <li><a href='#' class='font-bold' onClick='EditThisTrip(`" + u.Id + "`);'>Update</a></li>" +
                                                                    //        "                </ul>" +
                                                                    //        "</div>",
                                                                    Id =  
                                                                            "<a href='#' class='btn btn-sm btn-primary btn-block font-bold' onClick='OpenThisTrip(`" + u.Id + "`);'>Select</a>" +
                                                                            "<a href='#' class='btn btn-sm btn-success btn-block font-bold " + hide_ + "' onClick='EditThisTrip(`" + u.Id + "`);'>Update</a>" +
                                                                            "<a href='#' class='btn btn-sm btn-warning btn-block font-bold " + hide_ + "' onClick='CancelThisTrip(`" + u.Id + "`);'>Cancel</a>"+
                                                                            "<a href='"+ linkForManifest + "?t="+u.TripControlNo+"' class='btn btn-sm btn-danger btn-block font-bold " + hide_ + "'  target='_blank' ><span class='fa fa-pdf'></span> Print Manifest</a>" +
                                                                            "<a href='#' class='btn btn-sm btn-info btn-block font-bold " + hide_ + "' onClick='EmailGS(`" + u.Id + "`);'>"+ (u.EmailStatus == 0 ? "" : "Re-") +"Transmit</a>" 
                                                                            ,
                                                                            
                                                                    TripType = u.ReservationType.Description,
                                                                    ServiceDate = u.ServiceStartDate.ToString("MMMM dd,yyyy"),
                                                                    TimeStart = u.ServiceStartDate.ToString("HH:mm:ss"),
                                                                    TimeEnd = u.ServiceEndDate.ToString("HH:mm:ss"),
                                                                    Driver = u.Driver.FirstName + " " + u.Driver.LastName ,
                                                                    //Vehicle = u.VehicleList.Model + " / " + u.VehicleList.PlateNumber,
                                                                    Vehicle = (u.VehicleList.CodingDay == (int)u.ServiceStartDate.Date.DayOfWeek) ? u.VehicleList.Model + " / " + u.VehicleList.PlateNumber + " <b style='color:red'>(Coding)</b>" : u.VehicleList.Model + " / " + u.VehicleList.PlateNumber,
                                                                    OriginalCapacity = u.Capacity,
                                                                    Remaining = u.RemainingCapacity,
                                                                    RemainingPM = u.RemainingCapacityPM
                                                                });


            // Total record count.
            int totalRecords = TripList.Count();

            // Verification.
            if (!string.IsNullOrEmpty(search))
            {   // Apply search
                TripList = TripList.Where(x => x.Driver.ToLower().Contains(search.ToLower())
                                         && x.Vehicle.ToLower().Contains(search.ToLower())
                                         && x.TripType.ToLower().Contains(search.ToLower())
                                         );
            }
            // Sorting.
            string[] sort = new string[] { "Id", "TripType", "Driver", "Vehicle" };
            //var sortfield = sort[int.Parse(order)];
            //EmployeeList = EmployeeList.OrderBy();

            // Filter record count.
            int recFilter = TripList.Count();

            // Apply pagination.
            TripList = TripList.Skip(startRec).Take(pageSize);


            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = TripList.ToList(),
            };



            return new JsonResult(jsonData);

        }

        [HttpPost]
        public async Task<ActionResult> checkTrip(int tripId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            //var tripData = _context.Trip.Find(tripId);
            var tripData = _context.Trip
                .Where(t => t.Id == tripId
                    && _context.Drivers
                        .Where(d => d.Id == t.DriverId)
                        .Select(d => d.CompanyGroupId)
                        .FirstOrDefault() == currentCompanyGroupId
                ).FirstOrDefault();
            var _vehicleName = _context.VehicleLists.Where(e=> e.Id.Equals(tripData.VehicleListId)).Select(u => new {
                Id = u.Id,
                Model = u.Model,
                PlateNumber = (u.CodingDay == (int)tripData.ServiceStartDate.Date.DayOfWeek) ?  u.PlateNumber + " <b style='color:red'>(Coding)</b>" : u.PlateNumber,
                Vehicle = (u.CodingDay == (int)tripData.ServiceStartDate.Date.DayOfWeek) ? u.Model + " / " + u.PlateNumber + " <b style='color:red'>(Coding)</b>" : u.Model + " / " + u.PlateNumber,

            });
            var _driverName = _context.Drivers.Find(tripData.DriverId);

            var _starttime = tripData.ServiceStartDate.ToString("HH:mm");
            var _endtime = tripData.ServiceEndDate.ToString("HH:mm");


            var jsonData = new
            {
                vehicleName = _vehicleName,
                driverName = _driverName,
                data = tripData,
                starttime = _starttime,
                endtime = _endtime
            };



            return new JsonResult(jsonData);

        }

        

        [HttpPost]
        public async Task<IActionResult> reservePassengers(int _id)
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var returnRemainingAM = 0;
            var returnRemainingPM = 0;
            int Counter = 0;
            int CapacityAM = 0;
            int CapacityPM = 0;
            string _message = "";
            string SMSStatus = "";
            string _MobileNumber = "";
            using (var transaction = _context.Database.BeginTransaction())
            {
       
                try
                {
                    string _destination = "";
                    string _tripdate = "";
                    string _triptimefrom = "";
                    string _triptimeto = "";
                    string _driver = "";
                    string _drivercontact = "";
                    string _vehicle = "";
                    string _transactionId = "";
                    string _mobileNumber = "";
                    string tempMobileNumber = "";
                    string emailRecipient = "";
                    string transactionId = "";
                    //composing of message
                    var getTrip = _context.Trip.FirstOrDefault(t => t.Id.Equals(_id));
                
                    _destination = "Makati HO to Calaca (Vice Versa)";
                    _tripdate = getTrip.ServiceStartDate.ToString("MMM dd,yyyy");
                    _triptimefrom = getTrip.ServiceStartDate.ToString("hh:mm tt");
                    _triptimeto = getTrip.ServiceEndDate.ToString("hh:mm tt");


                    var getDriver = _context.Drivers.FirstOrDefault(t => t.Id.Equals(getTrip.DriverId));
                    _driver = getDriver.FirstName + " " + getDriver.LastName;
                    _drivercontact = getDriver.MobileNumber;

                    var getVehicle = _context.VehicleLists.FirstOrDefault(t => t.Id.Equals(getTrip.VehicleListId));
                    _vehicle = getVehicle.Model + " " + getVehicle.PlateNumber;

                    //end composing
                    returnMessage = "Reserved";
                    string[] transIdsTemp = TransactionIds.Split(",");
                    string[] transIds = transIdsTemp.Distinct().ToArray();
                    foreach (string reference in transIds)
                    {
                    


                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.Id == Int32.Parse(reference) && s.Status == 2 );

                        if (shuttlePassenger.Count() > 0)
                        {
                            await shuttlePassenger.ForEachAsync(e => {
                                if (e.TripTypeId == 2)
                                {
                                    CapacityAM++;
                                }
                                else if (e.TripTypeId == 3)
                                {
                                    CapacityPM++;
                                }
                                else if (e.TripTypeId == 1)
                                {
                                    CapacityAM++;
                                    CapacityPM++;
                                }

                                e.ShuttleId = _id;
                                e.ReservedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.ReservedDatetime = DateTime.Now;


                                //message

                                _message = "";
                                if (e.PassengerTypeId == 1)
                                {
                                    var getEmp = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo));
                                    if (getEmp != null)
                                    {
                                        //_MobileNumber = getEmp.MobileNumber;
                                        tempMobileNumber = tempMobileNumber + getEmp.MobileNumber + ",";
                                        //new code for optimized text
                                        if (IsValidEmail(getEmp.CompanyEmail))
                                        {
                                            emailRecipient = emailRecipient + getEmp.CompanyEmail + ",";
                                        }
                                        transactionId = e.TransactionId;
                                        //end new code for optimized text
                                    }

                                    _transactionId = e.TransactionId;
                                    _MobileNumber = _context.Employees.FirstOrDefault(emp=> emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                }
                                else {
                                    //  _MobileNumber = e.ContactNo;
                                    tempMobileNumber = tempMobileNumber + e.ContactNo + ",";
                                }
                                
                                if(e.SMSRevision > 0)
                                {
                                    _message += "Revision "+e.SMSRevision+" on ";
                                }


                                /// relocation outside the loop 
                                /*
                                _MobileNumber = _MobileNumber + ",";
                                //*******FOR DRIVERS
                                var drivers = _context.Drivers.Where(d => d.Status.Equals(1) && d.Id.Equals(getDriver.Id)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                                foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                                {
                                    if (pnumbers != "")
                                    {
                                        _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                                    }
                                }
                                //*******END FOR DRIVERS

                                ////*******FOR OVERALL APPROVERS
                                //var users = _userManager.GetUsersInRoleAsync("Overall Approver").Result.ToList().ToArray();

                                //foreach (int _user in users.Select(u => u.EmployeeId))
                                //{
                                //    var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_user)).MobileNumber;
                                //    if (_userMobile != "")
                                //    {
                                //        _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                                //    }
                                //}
                                ////*******FOR OVERALL APPROVERS

                                //*******FOR AGS Personnel
                                var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                                foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                                {
                                    var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_userAGS)).MobileNumber;
                                    if (_userMobile != "")
                                    {
                                        _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                                    }
                                }
                                //*******FOR AGS Personnel .
                                */


                                //end message

                                e.SMSRevision = e.SMSRevision + 1;


                            });

                            await _context.SaveChangesAsync();
                            Counter++;

                        }

                        if (Counter <= 0)
                        {
                            throw new System.Exception("There is no reservation to reserve.");
                        }


                        var trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(_id));
                        trip.EmailStatus = 0;
                        _context.Update(trip);
                        _context.SaveChanges();

                        Log _log = new Log();
                        _log.Process = "Add Trip - Reservation ID (" + reference + ") - Trip ID (" + _id + ")" + SMSStatus;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);


                    }





                    ////// RELOCATE SENDING SMS ON RESERVATION
                    _MobileNumber = "";

                    //edit 10202022

                    _MobileNumber = tempMobileNumber;

                    _message += "THSR ID: " + _transactionId + Environment.NewLine +
                                "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                _driver + Environment.NewLine +
                                "CP no. " + _drivercontact + Environment.NewLine +
                                "Vehicle : " + _vehicle + ".";

                    //end   //edit 10202022


                    //*******FOR DRIVERS
                    var drivers = _context.Drivers.Where(d => d.Status.Equals(1) && d.Id.Equals(getDriver.Id)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                    foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                    {
                        if (pnumbers != "")
                        {
                    //        _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                        }
                    }
                    //*******END FOR DRIVERS

                    //*******FOR AGS Personnel
                    var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                    foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                    {
                        var _userMobile = _context.Employees.FirstOrDefault(e_ => e_.Id.Equals(_userAGS)).MobileNumber;
                        if (_userMobile != "")
                        {
                    //        _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                        }
                    }
                    //*******FOR AGS Personnel .
                    /*
                    _message += "THSR ID: " + e.TransactionId + Environment.NewLine +
                                "The driver / vehicle assigned to your trip from " + _destination + " on " + _tripdate + " from " + _triptimefrom + " to " + _triptimeto + " is: " + Environment.NewLine +
                                _driver + Environment.NewLine +
                                "CP no. " + _drivercontact + Environment.NewLine +
                                "Vehicle : " + _vehicle + ".";
                    */


                    //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//
                    //var sendSMS2 = new ReportsController().SendSMS(_message, _MobileNumber);

                    //if (sendSMS2 != "200")
                    //{
                    //    SMSStatus = " but has error on sending SMS.";
                    //}

                    //JPT commented code 09052024
                    //string[] mobileArr = _MobileNumber.Split(',');
                    //int xIndex = 1;
                    //foreach(string mobileNum in mobileArr)
                    //{
                    //    await new ReportsController().CallSMSAPI(mobileNum, _message, "TRIP");
                    //    xIndex++;
                    //}
                    //----10 24 2022 NEW METHOD EBELAYDA 10/24/2022----//


                    //JPT additional code 09052024
                    string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                    string message = _message;
                    string referenceNo = _transactionId;  // Replace with your transaction ID
                    string systemName = "THS Makati";

                    // Call the SendSmsAsync method
                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                    SMSStatus = smsStatus + ". ";
                    //JPT end additional code 09052024

                    string sendemail = new ReportsController().SendEmail("SHUTTLE/DRIVER SERVICE RESERVATION", emailRecipient, "", _message, "Shuttle Service");
                    //if (sendemail != "1") SMSStatus = SMSStatus + " and has error on sending Email."; //JPT commented code 09062024
                    if (sendemail != "1") SMSStatus += "Email not sent due to bad connection."; //JPT additional code 09062024

                    //// END RELOCATION


                    //checking the remaining capacity

                    var checkcapacity = _context.Trip.FirstOrDefault(e => e.Id == _id);
                    int remainingAM = checkcapacity.RemainingCapacity - CapacityAM;
                    int remainingPM = checkcapacity.RemainingCapacityPM - CapacityPM;

                    returnRemainingAM = checkcapacity.RemainingCapacity;
                    returnRemainingPM = checkcapacity.RemainingCapacityPM;

                    if (remainingAM < 0)
                    {
                        throw new System.Exception("Invalid Capacity AM trip.");
                    }
                    else if (remainingPM < 0)
                    {
                        throw new System.Exception("Invalid Capacity For PM trip.");
                    }
                    else
                    {
                        checkcapacity.RemainingCapacity = remainingAM;
                        checkcapacity.RemainingCapacityPM = remainingPM;
                        await _context.SaveChangesAsync();
                        returnRemainingAM = remainingAM;
                        returnRemainingPM = remainingPM;
                    }

                    //end


                    _context.SaveChanges();
                    returnMessage += SMSStatus;
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
                success = returnSuccess,
                remainingAM = returnRemainingAM,
                remainingPM = returnRemainingPM
            };
            return new JsonResult(jsonData);
        }


        [HttpPost]
        public async Task<IActionResult> removeReservePassengers(int _id)
        {
            var TransactionIds = Request.Form["TransactionId"].ToString();
            var returnSuccess = 1;
            var returnMessage = "";
            var returnRemainingAM = 0;
            var returnRemainingPM = 0;
            int Counter = 0;
            int CapacityAM = 0;
            int CapacityPM = 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {


                    string[] transIdsTemp = TransactionIds.Split(",");
                    string[] transIds = transIdsTemp.Distinct().ToArray();
                    foreach (string reference in transIds)
                    {



                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.Id == Int32.Parse(reference) && s.Status == 2);

                        if (shuttlePassenger.Count() > 0)
                        {
                            await shuttlePassenger.ForEachAsync(e => {
                                if (e.TripTypeId == 2)
                                {
                                    CapacityAM++;
                                }
                                else if (e.TripTypeId == 3)
                                {
                                    CapacityPM++;
                                }
                                else if (e.TripTypeId == 1)
                                {
                                    CapacityAM++;
                                    CapacityPM++;
                                }

                                e.ShuttleId = 0;
                                e.RemovedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                                e.RemovedDatetime = DateTime.Now;
                            });

                            await _context.SaveChangesAsync();
                            Counter++;
                        }

                        if (Counter <= 0)
                        {
                            throw new System.Exception("There is no reservation to remove.");
                        }

                        var trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(_id));
                        trip.EmailStatus = 0;
                        _context.Update(trip);
                        _context.SaveChanges();

                        Log _log = new Log();
                        _log.Process = "Remove Trip - Reservation ID (" + reference + ") - Trip ID (" + _id + ")";
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);



                    }

                    //checking the remaining capacity

                    var checkcapacity = _context.Trip.FirstOrDefault(e => e.Id == _id);
                    int remainingAM = checkcapacity.RemainingCapacity + CapacityAM;
                    int remainingPM = checkcapacity.RemainingCapacityPM + CapacityPM;

                    returnRemainingAM = checkcapacity.RemainingCapacity;
                    returnRemainingPM = checkcapacity.RemainingCapacityPM;

                    if (remainingAM > checkcapacity.Capacity)
                    {
                        throw new System.Exception("Invalid Capacity AM trip.");
                    }
                    else if (remainingPM > checkcapacity.Capacity)
                    {
                        throw new System.Exception("Invalid Capacity For PM trip.");
                    }
                    else
                    {
                        checkcapacity.RemainingCapacity = remainingAM;
                        checkcapacity.RemainingCapacityPM = remainingPM;
                        await _context.SaveChangesAsync();
                        returnRemainingAM = remainingAM;
                        returnRemainingPM = remainingPM;
                    }

                    //end


                    _context.SaveChanges();
                    returnMessage = "Removed";
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
                success = returnSuccess,
                remainingAM = returnRemainingAM,
                remainingPM = returnRemainingPM
            };
            return new JsonResult(jsonData);
        }


        [Authorize(Policy = "RequireAllRole")]
        [HttpPost]
        public async Task<JsonResult> LoadVehicleSchedule(string dateFilter)
        {

            string _status = "OK";
            string _message = "";

            string iDate = dateFilter;
            DateTime oDate = Convert.ToDateTime(iDate);
            //DateTime nextMonth = oDate.AddDays(31);
            //DateTime lastMonth = oDate.AddDays(-31);
            //int[] NonDuplicateStatus = { 1, 2, 3, 4 };

            string _timestampfrom = oDate.ToString("yyyy-MM-dd") + " 00:00:00";
            string _timestampto = oDate.ToString("yyyy-MM-dd") + " 23:59:59";

            //DateTime timestampfrom = DateTime.ParseExact(_timestampfrom, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);
            //DateTime timestampto = DateTime.ParseExact(_timestampto, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            DateTime timestampfrom = DateTime.Parse(_timestampfrom);
            DateTime timestampto = DateTime.Parse(_timestampto);


            //check the ids that has trips 
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var checkTripToday = (from trip in _context.Trip
                                  join vehicle in _context.VehicleLists on trip.VehicleListId equals vehicle.Id
                                  where trip.Status >= 1
                                        && trip.ServiceStartDate >= timestampfrom
                                        && trip.ServiceEndDate <= timestampto
                                        && vehicle.CompanyGroupId == currentCompanyGroupId
                                  select trip.VehicleListId)
          .Distinct()
          .ToArray();

            int[] HasTripIds = new int[200];
            var x = 0;
            foreach (int _data in checkTripToday)
            {
                HasTripIds[x] = _data;
                x++;
            }
            if (x <= 0)
            {
                HasTripIds[0] = -1;
            }


            //end 


            var vehicleList = _context.VehicleLists.OrderBy(e => e.PlateNumber).Where(e => e.Status.Equals(1) && HasTripIds.Contains(e.Id)).Select(u => new
            {
                Id = u.Id,
                PlateNumber = u.PlateNumber,
                Model = u.Model,
                CodingDay = u.CodingDay,
                Capacity = u.Capacity,
                VehicleName = u.PlateNumber + " - " + u.Model + "(" + u.Capacity +")"
            });

            string[] _VehicleNames = vehicleList.Select(e => e.VehicleName).ToList().ToArray();
            int[] _VehicleIds = vehicleList.Select(e => e.Id).ToList().ToArray();
            //int[] _VehicleIds = vehicleList.Select(e => e.Id).ToList().ToArray();

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            Random randomGen = new Random();


            var VehicleTripLists = _context.Trip.OrderBy(e => e.VehicleList.PlateNumber).Where(r => r.Status >= 1 && _VehicleIds.Contains(r.VehicleListId) && r.ServiceStartDate >= timestampfrom && r.ServiceEndDate <= timestampto ).
                     Select(u => new
                     {
                         //x = (u.VehicleList.CodingDay == (int)timestampfrom.DayOfWeek) ? timestampfrom.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                         //x2 = (u.VehicleList.CodingDay == (int)timestampto.DayOfWeek) ? timestampto.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceEndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                         x = (u.VehicleList.CodingDay == (int)timestampfrom.DayOfWeek) ? u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                         x2 = (u.VehicleList.CodingDay == (int)timestampto.DayOfWeek) ? u.ServiceEndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceEndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,

                         driver = u.Driver.FirstName + " " + u.Driver.LastName,
                         location = (u.ReservationTypeId == 2) ? _context.DriverPassengerHeaders.FirstOrDefault(e => e.ShuttleId.Equals(u.Id)).Origin + " - " + _context.DriverPassengerHeaders.FirstOrDefault(e => e.ShuttleId.Equals(u.Id)).Destination  : "Makati HO - Calaca (Vice Versa)",
                         company = "",
                         y = Array.IndexOf(_VehicleIds, u.VehicleListId),
                         t1 = u.ServiceStartDate.ToString("HH:mm"),
                         t2 = u.ServiceEndDate.ToString("HH:mm")

                     }).ToList().ToArray();



            var jsonData = new
            {
                Status = _status,
                Message = _message,
                VehicleNames = _VehicleNames,
                trip = VehicleTripLists
                //Events = PassengerList.ToList().ToArray(),
                //DateFilter = oDate
            };

            return new JsonResult(jsonData);
        }

        [Authorize(Policy = "RequireAllRole")]
        [HttpPost]
        public async Task<JsonResult> LoadDriverSchedule(string dateFilter)
        {

            string _status = "OK";
            string _message = "";

            string iDate = dateFilter;
            DateTime oDate = Convert.ToDateTime(iDate);
            //DateTime nextMonth = oDate.AddDays(31);
            //DateTime lastMonth = oDate.AddDays(-31);
            //int[] NonDuplicateStatus = { 1, 2, 3, 4 };

            string _timestampfrom = oDate.ToString("yyyy-MM-dd") + " 00:00:00";
            string _timestampto = oDate.ToString("yyyy-MM-dd") + " 23:59:59";

            //DateTime timestampfrom = DateTime.ParseExact(_timestampfrom, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);
            //DateTime timestampto = DateTime.ParseExact(_timestampto, "yyyy-MM-dd HH:mm:ss,fff",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            DateTime timestampfrom = DateTime.Parse(_timestampfrom);
            DateTime timestampto = DateTime.Parse(_timestampto);


            //check the ids that has trips 
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();

            var checkTripToday = (from trip in _context.Trip
                                  join driver in _context.Drivers on trip.DriverId equals driver.Id
                                  where trip.Status >= 1
                                        && trip.ServiceStartDate >= timestampfrom
                                        && trip.ServiceEndDate <= timestampto
                                        && driver.CompanyGroupId == currentCompanyGroupId
                                  select trip.DriverId)
                      .Distinct()
                      .ToArray();


            int[] HasTripIds = new int[200];
            var x = 0;
            foreach (int _data in checkTripToday)
            {
                HasTripIds[x] = _data;
                x++;
            }
            if (x <= 0)
            {
                HasTripIds[0] = -1;
            }


            //end 

            var driverList = _context.Drivers.OrderBy(e => e.LastName).Where(e => e.Status.Equals(1) && HasTripIds.Contains(e.Id)).Select(u => new
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.LastName + ", " + u.FirstName
            });

            string[] _DriverNames = driverList.Select(e => e.FullName).ToList().ToArray();
            int[] _DriverIds = driverList.Select(e => e.Id).ToList().ToArray();
            //int[] _VehicleIds = vehicleList.Select(e => e.Id).ToList().ToArray();

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            Random randomGen = new Random();


            var DriverTripLists = _context.Trip.OrderBy(e => e.Driver.LastName).Where(r => r.Status >= 1 && _DriverIds.Contains(r.DriverId) && r.ServiceStartDate >= timestampfrom && r.ServiceEndDate <= timestampto).
                     Select(u => new
                     {
                         //x = (u.VehicleList.CodingDay == (int)timestampfrom.DayOfWeek) ? timestampfrom.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                         //x2 = (u.VehicleList.CodngDay == (int)timestampfrom.DayOfWeek) ? timestampfrom.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,

                         x = (u.VehicleList.CodingDay == (int)timestampfrom.DayOfWeek) ? u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceStartDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                         x2 = (u.VehicleList.CodingDay == (int)timestampto.DayOfWeek) ? u.ServiceEndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds : u.ServiceEndDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,

                         driver = u.Driver.FirstName + " " + u.Driver.LastName,
                         location = (u.ReservationTypeId == 2) ? _context.DriverPassengerHeaders.FirstOrDefault(e => e.ShuttleId.Equals(u.Id)).Origin + " - " + _context.DriverPassengerHeaders.FirstOrDefault(e => e.ShuttleId.Equals(u.Id)).Destination : "Makati HO - Calaca (Vice Versa)",
                         company = "",
                         y = Array.IndexOf(_DriverIds, u.DriverId),
                         t1 = u.ServiceStartDate.ToString("HH:mm"),
                         t2 = u.ServiceEndDate.ToString("HH:mm")

                     }).ToList().ToArray();



            // VehicleTripLists.Add(new Trip { x })

            //VehicleTripLists.Insert(14, { x = 1.01,1.04, " "," ",2});

            //var _trips = new List<string[]>();
            //int _currentIndex = 0;
            //await vehicleList.ForEachAsync(e =>
            //{
            //   var VehicleTripLists = _context.Trip.Where(r => r.VehicleListId.Equals(e.Id) && r.ServiceStartDate >= timestampfrom).
            //         Select(u => new
            //         {
            //                x = u.ServiceStartDate.ToUniversalTime(),
            //                x2 = u.ServiceEndDate.ToUniversalTime(),
            //                driver = u.Driver.FirstName + " " + u.Driver.LastName + " / ",
            //                company = "test",
            //                y = _currentIndex

            //         }).ToList();


            //    _trips.Add(VehicleTripLists);


            //    _currentIndex++;


            //});





            var jsonData = new
            {
                Status = _status,
                Message = _message,
                DriverNames = _DriverNames,
                trip = DriverTripLists
                //Events = PassengerList.ToList().ToArray(),
                //DateFilter = oDate
            };

            return new JsonResult(jsonData);
        }

        private string randomcolor() {
            string color = "";
            string[] colorArr = { "#058DC7", "#50B432", "#ED561B", "#DDDF00", "#24CBE5", "#64E572", "#FF9655", "#FFF263", "#6AF9C4" };
            var rand = new Random();
            color = colorArr[rand.Next(0, 9)];

            return color;
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            //cancel = 0

            var returnSuccess = 1;
            var returnMessage = "";



            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(id));
                    if (trip == null)
                    {
                        throw new System.Exception("Trip data not found.");
                    }

                    trip.Status = 0;
                    trip.CancelledBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    trip.CancelReason = Request.Form["CancelReason"].ToString();
                    trip.CancelledDatetime = DateTime.Now;
                    
                    _context.Update(trip);
                    await _context.SaveChangesAsync();

                    if (trip.ReservationTypeId == 2)
                    {

                        int headerId = Int32.Parse(Request.Form["t"]);

                        var DriverHeader = _context.DriverPassengerHeaders.FirstOrDefault(e => e.Id.Equals(headerId));
                        DriverHeader.ShuttleId = 0;
                        DriverHeader.Status = 2;// added ebe 05122020 ECQ
                        _context.Update(DriverHeader);

                        var driverPassenger = _context.DriverPassengers.Where(s => s.TransactionId == DriverHeader.TransactionId);

                        if (driverPassenger.Count() > 0)
                        {
                            //await driverPassenger.ForEachAsync(e => //JPT commented code 09062024
                            foreach (var e in driverPassenger) //JPT additional code 09062024
                            {
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
                        }

                        await _context.SaveChangesAsync();

                    }
                    else {

                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.ShuttleId.Equals(trip.Id));
                        await shuttlePassenger.ForEachAsync(e =>
                        {
                            e.ShuttleId = 0;
                        });
                        await _context.SaveChangesAsync();

                    }




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



        public async Task<IActionResult> Manifest()
        {
            //var webRoot = _env.WebRootPath;
            ReportService.Report _report = new Report
            {
                FileName = "ShuttleManifest.rdl",
                FolderName = "SSRS",
                //Default Directory = \\192.168.70.165\inetpub\wwwroot\jrpt\Report
                //Directory = webRoot + @"\Report\"
            };

            ReportService.Database database = new Database
            {
                //DbServer = "192.168.70.102",
                //DbName = "ShuttleReservationDB",

                DbServer = "192.168.30.156",//calaca dev
                DbName = "THS_TEST2",//calaca dev
                DbUser = "ict",
                DbPwd = "ict@ictdept"
            };

            List<ReportService.ReportDataSet> reportDataSets = new List<ReportDataSet>();

            //FOR SQL QUERY
            ReportService.ReportDataSet SQLQueryDataset = new ReportDataSet
            {
                DataSetName = "Trips",
                SQLQuery = "SELECT * FROM Trip " +
                           "   LEFT OUTER JOIN VehicleLists " +
                           "     ON Trip.VehicleListId = VehicleLists.Id " +
                           "   INNER JOIN Drivers " +
                           "     ON Trip.DriverId = Drivers.Id "
            };
            reportDataSets.Add(SQLQueryDataset);

            ////FOR STORED PROCEDURE
            //ReportService.ReportDataSet _dataset = new ReportDataSet
            //{
            //    DataSetName = "DataSet1",
            //    StoredProcedureCommandType = ReportService.CommandType.Text,
            //    StoredProcedureCommandText = "spUserList"

            //};
            //_dataset.StoredProcedureParameters = new StoredProcedureParameter[1];

            //StoredProcedureParameter paramData = new StoredProcedureParameter
            //{
            //    SpPramName = "@Id",
            //    SpPramDataType = DbType.String,
            //    SpPramValue = "8c2357bd-1c24-46bc-a492-d531e9315874"
            //};
            //_dataset.StoredProcedureParameters[0] = paramData;

            //reportDataSets.Add(_dataset);

            //PDF
            var client = new ReportService.Service1Client();
            byte[] bytes = await client.GeneratePDFAsync(reportDataSets.ToArray(), _report, database);
            return File(bytes, "application/pdf");

            //EXCEL
            //var client = new ReportService.Service1Client();
            //byte[] bytes = await client.GenerateExcelAsync(reportDataSets.ToArray(),_report,database);
            //return File(bytes, "application/vnd.ms-excel",  "Filename.xls");
        }

        

        [HttpPost]
        public async Task<JsonResult> SendEmailGS(int id)
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];


            string _message =  "Email Sent Successfully!";
            string returnMessage = "";
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {

                        var _trip = _context.Trip.FirstOrDefault(e => e.Id.Equals(id));

                    var _email = _context.EmailRecipients.Where(e => e.Group.Equals("GS")).Select(e => e.Email).Distinct().ToList().ToArray();
                    string Emails = "";
                    foreach (string emails in _email)
                    {
                        Emails = Emails + "," + emails;
                    }

                    string SenderEmails = _context.Employees.FirstOrDefault(e=> e.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                    //var sendemail = new ReportsController().SendEmail("Test Name", "ebelayda@semirarampc.com", "This is a test " + _trip.TripControlNo, "Shuttle Service");
                    
                    string sendemail = (string) new ReportsController().ReportForGS(_trip.TripControlNo, "Shuttle_Service_v"+ _trip.EmailVersion + "-"+ _trip.TripControlNo, Emails,SenderEmails, "Hi, Attached is the trip report version "+ _trip.EmailVersion + " for trip control number:  " + _trip.TripControlNo, "Shuttle Service","email").AsyncState;

                        //var task = MyAsyncMethod();
                        //var result = task.WaitAndUnwrapException();
                        //if (sendemail.ToString() == "1")
                        //{
                            _trip.EmailStatus = 1;
                            _trip.EmailVersion += 1;
                            _trip.EmailBy = HttpContext.Session.GetString("Session_userDomainWithName");
                            _trip.EmailDatetime = DateTime.Now;
                            _context.Update(_trip);
                            _context.SaveChanges();
                    //}
                    //else
                    //{
                    //    _message = sendemail.ToString();
                    //}

                    //returnMessage = sendemail.ToString();
                    returnMessage = "1" + sendemail;


                    Log _log = new Log();
                        _log.Process = "Email GS Trip No. " + _trip.TripControlNo + " - " + _message;
                        _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                        _log.ProcessedDate = DateTime.Now;
                        _context.Add(_log);

                    _context.SaveChanges();
                    transaction.Commit();

                }
                    catch (Exception e)
                    {
                        returnMessage = "Internal Server Error - " + e.Message + "/" + e.InnerException;
                        transaction.Rollback();
                        //throw e;
                    }
                }

                var jsonData = new
                {
                    Message = returnMessage
                };

                return new JsonResult(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> SendSMSGroupTemplate()
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];


            string _message = "SMS Sent Successfully!";
            string returnMessage = "";
            string timestamp = Request.Form["timestamp"];
            long unixTime = Convert.ToInt64(Request.Form["timestamp"]);
            var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var _oldsmslog = _context.SMSLogs.LastOrDefault(s => s.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)));
                    int _version = 1;
                    if(_oldsmslog != null)
                    {
                        _version = _oldsmslog.Version + 1;
                    }
                    
                    var _smslog = new SMSLog();


                    _smslog.Status = 1;
                    _smslog.ServiceDateTimeStamp = Convert.ToInt64(timestamp);
                    _smslog.Version = _version;
                    _smslog.Process = "Send SMS Summary Version " + _version + " for trip " + dtimestamp;
                    _smslog.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _smslog.ProcessedDate = DateTime.Now;
                    _context.Add(_smslog);
                    _context.SaveChanges();


                    var _trips = _context.Trip.Where(t => t.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp))).OrderBy(t => t.Driver.LastName).ThenBy(t => t.ServiceStartDate);
                    if (_trips.Count() <= 0)
                    {
                        throw new System.Exception("No trip details to send.");
                    }

                    string _TxtMessage = "";
                     _TxtMessage += "THS Trip Summary" +  Environment.NewLine + " ***This is system-generated.*** " + Environment.NewLine;
                    _TxtMessage += DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy dddd") + Environment.NewLine + "Version " + _version + Environment.NewLine ;

                    int driverId = 0;
                    int _counter = 1;
                    string _names = "";
                    int destinationCounter = 0;
                    await _trips.ForEachAsync(e => {
                        _names = "";
                        _counter = 1;
                        if (driverId != e.DriverId) {
                            var driverName = _context.Drivers.FirstOrDefault(d=> d.Id.Equals(e.DriverId));
                            _TxtMessage += Environment.NewLine + "Driver: " + driverName.LastName + ", " + driverName.FirstName; 
                        }
                        _TxtMessage += Environment.NewLine + e.ServiceStartDate.ToString("h:mm tt");
                  
                            if (e.ReservationTypeId == 1) { 
                                // for AM
                                var _getname = _context.ShuttlePassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2) && (g.TripTypeId == 1 || g.TripTypeId == 2)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).Distinct().ToList().ToArray();
                                _names += Environment.NewLine + "Makati HO - Calaca";
                                foreach (string fullname in _getname.Select(x => x.FullName))
                                {
                                    _names += Environment.NewLine +  _counter + ". " + fullname;
                                    _counter++;
                                }


                                //for PM
                                _counter = 1;
                                var _getnamePM = _context.ShuttlePassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2) && (g.TripTypeId == 1 || g.TripTypeId == 3)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).Distinct().ToList().ToArray();
                                _names += Environment.NewLine + "Calaca - Makati HO";
                                foreach (string fullname in _getnamePM.Select(x => x.FullName))
                                {
                                    _names += Environment.NewLine + _counter + ". " + fullname;
                                    _counter++;
                                }


                            }
                            else {
                                var _getname = _context.DriverPassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2)).Select(x => new { FullName = x.FirstName + " " + x.LastName, _Destination = x.Origin + " - " + x.Destination }).Distinct().ToList().ToArray();
                                foreach (string destination in _getname.Select(x => x._Destination))
                                {
                                    if(destinationCounter == 0) { 
                                        _names += Environment.NewLine + "" + destination;
                                        destinationCounter++;
                                    }
                                }
                                destinationCounter = 0;
                                foreach (string fullname in _getname.Select(x => x.FullName))
                                {
                                    _names += Environment.NewLine + _counter + ". " + fullname;
                                    _counter++;
                                }
                                
                                
                            }

                        _TxtMessage += _names + Environment.NewLine;


                        //_TxtMessage += Environment.NewLine + "Driver: " + e.Driver.LastName + ", " + e.Driver.FirstName;

                        driverId = e.DriverId;
                    });
                    _TxtMessage +=  Environment.NewLine ;

                    string _MobileNumber = "";

                    _MobileNumber = _MobileNumber + ",";
                    //*******FOR DRIVERS
                    var drivers = _context.Drivers.Where(d => d.Status.Equals(1)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                    foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                    {
                        if (pnumbers != "")
                        {
                            _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";

                        }
                    }
                    //*******END FOR DRIVERS

                    ////*******FOR OVERALL APPROVERS
                    //var users = _userManager.GetUsersInRoleAsync("Overall Approver").Result.ToList().ToArray();

                    //foreach (int _user in users.Select(u => u.EmployeeId))
                    //{
                    //    var _userMobile = _context.Employees.FirstOrDefault(e => e.Id.Equals(_user)).MobileNumber;
                    //    if (_userMobile != "")
                    //    {
                    //        _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                    //    }
                    //}
                    ////*******FOR OVERALL APPROVERS

                    //*******FOR AGS Personnel
                    var usersAGS = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                    foreach (int _userAGS in usersAGS.Select(u => u.EmployeeId))
                    {
                        var _userMobile = _context.Employees.FirstOrDefault(e => e.Id.Equals(_userAGS)).MobileNumber;
                        if (_userMobile != "")
                        {
                            _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                        }
                    }
                    //*******FOR AGS Personnel

                    //JPT commented code 09042024
                    //string sendSMS = new ReportsController().SendSMS(_TxtMessage, _MobileNumber); 

                    //if (sendSMS != "200") {
                    //    throw new System.Exception("Error on sending SMS.");
                    //}

                    //JPT additional code 09052024
                    string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                    string message = _TxtMessage;
                    string referenceNo = "";  // Replace with your transaction ID
                    string systemName = "THS Makati";

                    // Call the SendSmsAsync method
                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                    //JPT end additional code 09052024

                    returnMessage = "1";


                    Log _log = new Log();
                    _log.Process = "SMS Trip Summary TimeStamp " + timestamp ;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);

                    _context.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnMessage = "Internal Server Error - " + e.Message + "/" + e.InnerException;
                    transaction.Rollback();
                    //throw e;
                }
            }

            var jsonData = new
            {
                Message = returnMessage
            };

            return new JsonResult(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> SendSMSGroup()
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];


            string _message = "SMS Sent Successfully!";
            string returnMessage = "";
            string timestamp = Request.Form["timestamp"];
            long unixTime = Convert.ToInt64(Request.Form["timestamp"]);
            var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");
            string _TxtMessage = "";
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var _oldsmslog = _context.SMSLogs.LastOrDefault(s => s.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)));
                    int _version = 1;
                    if (_oldsmslog != null)
                    {
                        _version = _oldsmslog.Version + 1;
                    }

                    var _smslog = new SMSLog();


                    _smslog.Status = 1;
                    _smslog.ServiceDateTimeStamp = Convert.ToInt64(timestamp);
                    _smslog.Version = _version;
                    _smslog.Process = "Send SMS Summary Version " + _version + " for trip " + dtimestamp;
                    _smslog.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _smslog.ProcessedDate = DateTime.Now;
                    _context.Add(_smslog);
                    _context.SaveChanges();


                    var _trips = _context.Trip.Where(t => t.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp))).OrderBy(t => t.Driver.LastName).ThenBy(t => t.ServiceStartDate);
                    if (_trips.Count() <= 0)
                    {
                        throw new System.Exception("No trip details to send.");
                    }

                 
                    _TxtMessage += Request.Form["smsText"];

                    string _MobileNumber = "";

                    _MobileNumber = _MobileNumber + ",";
                    //*******FOR DRIVERS
                    var drivers = _context.Drivers.Where(d => d.Status.Equals(1)).Select(x => new { PhoneNumbers = x.MobileNumber }).Distinct().ToList().ToArray();
                    foreach (string pnumbers in drivers.Select(x => x.PhoneNumbers))
                    {
                        if (pnumbers != "")
                        {
                            _MobileNumber += pnumbers.Replace(" ", string.Empty) + ",";
                        }
                    }

                    //*******END FOR DRIVERS

                    //*******FOR OVERALL APPROVERS
                    var users = _userManager.GetUsersInRoleAsync("AGS Personnel").Result.ToList().ToArray();

                    foreach (int _user in users.Select(u => u.EmployeeId))
                    {
                        var _userMobile = _context.Employees.FirstOrDefault(e => e.Id.Equals(_user)).MobileNumber;
                        if (_userMobile != "")
                        {
                            _MobileNumber += _userMobile.Replace(" ", string.Empty) + ",";
                        }
                    }
                    //*******FOR OVERALL APPROVERS

                    //JPT commented code 09042024
                    //string sendSMS = new ReportsController().SendSMS(_TxtMessage, _MobileNumber); 

                    //if (sendSMS != "200")
                    //{
                    //    throw new System.Exception("Error on sending SMS.");
                    //}

                    //JPT additional code 09052024
                    string mobileNumbers = _MobileNumber;  // Replace with actual mobile numbers
                    string message = _TxtMessage;
                    string referenceNo = "";  // Replace with your transaction ID
                    string systemName = "THS Makati";

                    // Call the SendSmsAsync method
                    string smsStatus = await new ReportsController().SendSmsAsync(mobileNumbers, message, referenceNo, systemName);
                    //JPT end additional code 09052024

                    returnMessage = "1";


                    Log _log = new Log();
                    _log.Process = "SMS Trip Summary TimeStamp " + timestamp;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);

                    _context.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnMessage = "Internal Server Error - " + e.Message + "/" + e.InnerException;
                    transaction.Rollback();
                    //throw e;
                }
            }

            var jsonData = new
            {
                Message = returnMessage
            };

            return new JsonResult(jsonData);
        }


        [HttpPost]
        public async Task<JsonResult> SendSMSGroupShow()
        {
            //    eventArray = [{ title: 'New event', start: new Date() },
            //{ title: 'New event 2', start: new Date() + 1 }];


            string _message = "SMS Sent Successfully!";
            int _returnSuccess = 0;
            string returnMessage = "";
            string timestamp = Request.Form["timestamp"];
            long unixTime = Convert.ToInt64(Request.Form["timestamp"]);
            var dtimestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var _oldsmslog = _context.SMSLogs.LastOrDefault(s => s.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)));
                    int _version = 1;
                    if (_oldsmslog != null)
                    {
                        _version = _oldsmslog.Version + 1;
                    }
                    


                    var _trips = _context.Trip.Where(t => t.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp))).OrderBy(t => t.Driver.LastName).ThenBy(t => t.ServiceStartDate);
                    if (_trips.Count() <= 0)
                    {
                        throw new System.Exception("No trip details to send.");
                    }

                    string _TxtMessage = "";
                    _TxtMessage += "THS Trip Summary" + Environment.NewLine + " ***This is system-generated.*** " + Environment.NewLine;
                    _TxtMessage += DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("MM/dd/yyyy dddd") + Environment.NewLine + "Version " + _version + Environment.NewLine;

                    int driverId = 0;
                    int _counter = 1;
                    string _names = "";
                    int destinationCounter = 0;
                    await _trips.ForEachAsync(e => {
                        _names = "";
                        _counter = 1;
                        if (driverId != e.DriverId)
                        {
                            var driverName = _context.Drivers.FirstOrDefault(d => d.Id.Equals(e.DriverId));
                            _TxtMessage += Environment.NewLine + "Driver: " + driverName.LastName + ", " + driverName.FirstName;
                        }
                        _TxtMessage += Environment.NewLine + e.ServiceStartDate.ToString("h:mm tt");

                        if (e.ReservationTypeId == 1)
                        {
                            // for AM
                            var _getname = _context.ShuttlePassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2) && (g.TripTypeId == 1 || g.TripTypeId == 2)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).Distinct().ToList().ToArray();
                            _names += Environment.NewLine + "Makati HO - Calaca";
                            foreach (string fullname in _getname.Select(x => x.FullName))
                            {
                                _names += Environment.NewLine + _counter + ". " + fullname;
                                _counter++;
                            }


                            //for PM
                            _counter = 1;
                            var _getnamePM = _context.ShuttlePassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2) && (g.TripTypeId == 1 || g.TripTypeId == 3)).Select(x => new { FullName = x.FirstName + " " + x.LastName }).Distinct().ToList().ToArray();
                            _names += Environment.NewLine + "Calaca - Makati HO";
                            foreach (string fullname in _getnamePM.Select(x => x.FullName))
                            {
                                _names += Environment.NewLine + _counter + ". " + fullname;
                                _counter++;
                            }


                        }
                        else
                        {
                            var _getname = _context.DriverPassengers.Where(g => g.ServiceDateTimeStamp.Equals(Convert.ToInt64(timestamp)) && g.ShuttleId.Equals(e.Id) && g.Status.Equals(2)).Select(x => new { FullName = x.FirstName + " " + x.LastName, _Destination = x.Origin + " - " + x.Destination }).Distinct().ToList().ToArray();
                            foreach (string destination in _getname.Select(x => x._Destination))
                            {
                                if (destinationCounter == 0)
                                {
                                    _names += Environment.NewLine + "" + destination;
                                    destinationCounter++;
                                }
                            }
                            destinationCounter = 0;
                            foreach (string fullname in _getname.Select(x => x.FullName))
                            {
                                _names += Environment.NewLine + _counter + ". " + fullname;
                                _counter++;
                            }


                        }

                        _TxtMessage += _names + Environment.NewLine;


                        //_TxtMessage += Environment.NewLine + "Driver: " + e.Driver.LastName + ", " + e.Driver.FirstName;

                        driverId = e.DriverId;
                    });
                    _TxtMessage += Environment.NewLine;

             

                    returnMessage = _TxtMessage;
                    _returnSuccess = 1;


                }
                catch (Exception e)
                {
                    _returnSuccess = 0;
                    returnMessage = "Internal Server Error - " + e.Message + "/" + e.InnerException;
                    //throw e;
                }
            }

            var jsonData = new
            {
                returnSuccess = _returnSuccess,
                Message = returnMessage
            };

            return new JsonResult(jsonData);
        }



        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
