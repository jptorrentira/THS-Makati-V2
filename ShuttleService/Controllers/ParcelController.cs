using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using ShuttleService.Data;
using System.Linq;
using ShuttleService.Services;
using Microsoft.AspNetCore.Authorization;
using ShuttleService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAllRole")]
    public class ParcelController : Controller
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;
        public static string _baseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ParcelRequestIdSequenceService _requestIdSequenceService;

        public ParcelController(ApplicationDbContext context, ParcelRequestIdSequenceService requestIdSequenceService, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _requestIdSequenceService = requestIdSequenceService;
            _userManager = userManager;
            _httpContextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
          
            ViewData["UserCompanyGroupId"] = currentCompanyGroupId;

            var OriginDestinationList = _context.OriginDestination.Where(m => m.CompanyGroupId == currentCompanyGroupId).OrderBy(m => m.OriginDestinationName).ToList();
            ViewData["OriginDestinationId"] = new SelectList(OriginDestinationList, "Id", "OriginDestinationName");

            var ParcelCategoryList = _context.ParcelCategories.OrderBy(m => m.Category).ToList();
            ViewData["ParcelCategoryId"] = new SelectList(ParcelCategoryList, "Id", "Category");

            var VehicleList = _context.VehicleLists.Where(v => v.CompanyGroupId == currentCompanyGroupId && v.Status == 1)
                .Select(v => new {Id = v.Id, Vehicle = $"{v.Model} / {v.PlateNumber}"}).OrderBy(v => v.Id).ToList();
            ViewData["VehicleListId"] = new SelectList(VehicleList, "Id", "Vehicle");

            var DriverList = _context.Drivers.Where(d => d.CompanyGroupId == currentCompanyGroupId && d.Status == 1)
                .Select(d => new { Id = d.Id, Driver = $"{d.FirstName} {d.LastName}" }).OrderBy(d => d.Id).ToList();
            ViewData["DriverId"] = new SelectList(DriverList, "Id", "Driver");

            return View();
        }

        public IActionResult ParcelCategory()
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
        public async Task<ActionResult> GetParcelCategoryList()
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
            var DataList = _context.ParcelCategories
                .OrderBy(e => e.Category).
                            Select(u => new
                            {
                                Id = u.Id,
                                Action = "<a href='javascript:void(0);' onClick='Edit(`" + u.Id + "`)'  class='btn btn-sm btn-primary' > Update </a> ",
                                Category = u.Category,
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
                DataList = DataList.Where(x => x.Category.ToLower().Contains(search.ToLower()));
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
            DataList = DataList.OrderBy(x => x.Category);
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
        public IActionResult GetParcelCategory(int id)
        {
            var category = _context.ParcelCategories.Where(r => r.Id == id);

            var jsonData = new
            {
                userData = category.ToList().ToArray(),
                Success = 1
            };

            return new JsonResult(jsonData);

        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateParcelCategory(int id, ParcelCategory data)
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
                    if (id == 0)
                    {
                        data.InsertBy = currentUser.DisplayName;
                        data.InsertDate = DateTime.Now;

                    }
                    else
                    {

                        data.UpdatedBy = currentUser.DisplayName;
                        data.UpdatedDate = DateTime.Now;
                    }

                    _context.Update(data);
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

        [AllowAnonymous]
        public async Task<IActionResult> ParcelReceived(string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                return RedirectToAction("AccessDenied", "Accounts");
            }
            var parcel = await _context.ParcelDeliveries.Where(x => x.RequestId == requestId).FirstOrDefaultAsync();
            if(parcel.Status == "DISPATCHED" || parcel.Status == "DELIVERED")
            {
                ViewData["RequestId"] = requestId;
                return View();
            }
            else if(parcel.Status == "RECEIVED")
            {
                ViewData["RequestId"] = requestId;
                ViewData["Received"] = "";
                return View("ParcelReceivedAlready");
            }
            else{
                return RedirectToAction("AccessDenied", "Accounts");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ParcelReceivedForm(string requestId, string receivedDate, string receivedTime)
        {
            try
            {
                var parcel = await _context.ParcelDeliveries.Where(x => x.RequestId == requestId).FirstOrDefaultAsync();
                parcel.Status = "RECEIVED";
                parcel.ActualReceivedDate = DateTime.Parse(
                        $"{receivedDate} {receivedTime}"
                    );
                _context.ParcelDeliveries.Update(parcel);
                await _context.SaveChangesAsync();

                var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcel.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                var destinationTag = _context.OriginDestination
                        .Where(d => d.Id == parcel.DestinationId)
                        .Select(d => d.OriginDestinationName)
                        .FirstOrDefault();
                var vehicle = _context.VehicleLists.Where(x => x.Id == parcel.VehicleListId).FirstOrDefault();
                var driverName = _context.Drivers.Where(x => x.Id == parcel.DriverId).Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var message =   $"<div>Request Id : <u>{parcel.RequestId}</u></div>" +
                                $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                $"<div>Parcel Description : <u>{parcel.ParcelDescription}</u></div>" +
                                $"<div>Assigned Driver : <u>{driverName}</u></div>" +
                                $"<div>Assigned Vehicle : <u>{vehicle.Model}</u></div>" +
                                $"<div>Assigned Vehicle Plate Number : <u>{vehicle.PlateNumber}</u></div>" +
                                $"<hr>" +
                                $"<div>The parcel has been <b>RECEIVED</b> by {parcel.Recipient} at {parcel.ActualReceivedDate}</b></div>"
                                ;
                SendParcelEmail(userEmail, parcel.RecipientEmail, message, parcel.RequestId);
                ViewData["RequestId"] = requestId;
                ViewData["Received"] = "Received";
                return View("ParcelReceivedAlready");
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while receiving parcel delivery record: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetParcelDeliveryData()
        {
            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var parcelDeliveries = _context.ParcelDeliveries.AsEnumerable();
            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                parcelDeliveries = parcelDeliveries.Where(u =>
                    u.RequestId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Destination.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Recipient.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.RecipientEmail.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.RecipientDepartment.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Status.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Destination.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    _context.ParcelCategories
                        .Where(d => d.Id == u.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    _context.OriginDestination
                        .Where(d => d.Id == u.DestinationId)
                        .Select(d => d.OriginDestinationName)
                        .FirstOrDefault().Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            var DataList = new List<object>();
            // Define your custom status order
            var statusOrder = new List<string> { "SUBMITTED", "DISPATCHED", "DELIVERED", "RECEIVED", "REJECTED" };

            // Sort parcelDeliveries using this custom order
            parcelDeliveries = parcelDeliveries
                .OrderBy(u => statusOrder.IndexOf(u.Status))  // Sort by your custom status list
                .ThenByDescending(u => u.FiledDate);          // Optional: sort by date within each status

            foreach (var u in parcelDeliveries)
            {
                var parcelCategory = _context.ParcelCategories
                    .Where(d => d.Id == u.ParcelCategoryId)
                    .Select(d => d.Category)
                    .FirstOrDefault();

                var destinationTag = _context.OriginDestination
                    .Where(d => d.Id == u.DestinationId)
                    .Select(d => d.OriginDestinationName)
                    .FirstOrDefault();
                var actualDispatchDate = u.ActualDispatchDate == DateTime.MinValue ? "" : u.ActualDispatchDate.ToString("MM/dd/yyyy hh:mm tt");
                var actualReceivedDate = u.ActualReceivedDate == DateTime.MinValue ? "" : u.ActualReceivedDate.ToString("MM/dd/yyyy hh:mm tt");
                var updatedDate = u.UpdatedDate == DateTime.MinValue ? "" : u.UpdatedDate.ToString("MM/dd/yyyy");
                var rejectedDate = u.RejectedDate == DateTime.MinValue ? "" : u.RejectedDate.ToString("MM/dd/yyyy");
                var vehicle = string.Empty;
                var driver = string.Empty;
                var chargingCompany = _context.ChargingCompanys.Where(x => x.Id == u.ChargingCompanyId).Select(x => x.CompanyName).FirstOrDefault();
                var chargingDepartment= _context.ChargingDepartments.Where(x => x.Id == u.ChargingDepartmentId).Select(x => x.DepartmentName).FirstOrDefault();
                if (u.VehicleListId > 0)
                {
                    vehicle = _context.VehicleLists.Where(v => v.Id == u.VehicleListId).Select(v => $"{v.Model} / {v.PlateNumber}").FirstOrDefault();
                }
                if(u.DriverId > 0)
                {
                    driver = _context.Drivers.Where(d => d.Id == u.DriverId).Select(d => $"{d.FirstName} {d.LastName}").FirstOrDefault();
                }
                var dropdownActions = @"
                    <div class='btn-group action-dropdown'>
                        <button type='button' class='btn btn-default btn-sm dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>
                            <i class='fa fa-cog'></i>
                        </button>
                        <ul class='dropdown-menu dropdown-menu-right'>";
                dropdownActions +=
                    "<li><a href='javascript:void(0);' class='dropdown-btn btn-info'" +
                        "onClick='ViewDetailsAjax(" +
                            "`" + u.Id + "`," +
                            "`" + u.RequestId + "`," +
                            "`" + u.Status + "`," +
                            "`" + parcelCategory + "`," +
                            "`" + u.ParcelDescription + "`," +
                            "`" + u.Instruction + "`," +
                            "`" + u.Recipient + "`," +
                            "`" + u.RecipientEmail + "`," +
                            "`" + u.RecipientDepartment + "`," +
                            "`" + destinationTag + "`," +
                            "`" + u.Destination + "`," +
                            "`" + u.DateTimeToBeReceived.ToString("MMM dd, yyyy HH:mm") + "`," +
                            "`" + actualDispatchDate + "`," +
                            "`" + actualReceivedDate + "`," +
                            "`" + u.FiledBy + "`," +
                            "`" + u.FiledDate.ToString("MMM dd, yyyy HH:mm") + "`," +
                            "`" + u.UpdatedBy + "`," +
                            "`" + updatedDate + "`," +
                            "`" + u.RejectedBy + "`," +
                            "`" + rejectedDate + "`," +
                            "`" + u.RejectionReason + "`," +
                            "`" + vehicle + "`," +
                            "`" + driver + "`," +
                            "`" + u.DeliveryRemarks + "`" +
                        ");'>View Details</a></li>";
                if (u.Status == "SUBMITTED" && u.FiledBy == User.Identity.Name)
                {
                    dropdownActions +=
                        "<li><a href='javascript:void(0);' class='dropdown-btn btn-secondary' " +
                        "onClick='EditAjax(" +
                            "`" + u.Id + "`," +
                            "`" + u.RequestId + "`," +
                            "`" + u.ChargingCompanyId + "`," +
                            "`" + chargingCompany + "`," +
                            "`" + u.ChargingDepartmentId + "`," +
                            "`" + chargingDepartment + "`," +
                            "`" + u.ParcelCategoryId + "`," +
                            "`" + u.DestinationId + "`," +
                            "`" + u.Destination + "`," +
                            "`" + u.Recipient + "`," +
                            "`" + u.RecipientEmail + "`," +
                            "`" + u.RecipientDepartment + "`," +
                            "`" + u.DateTimeToBeReceived.ToString("yyyy-MM-dd") + "`," +
                            "`" + u.DateTimeToBeReceived.ToString("HH:mm") + "`," +
                            "`" + u.ParcelDescription + "`," +
                            "`" + u.Instruction + "`" +
                        ");'>Edit</a></li>";
                }
                if (u.Status == "SUBMITTED" && (User.IsInRole("AGS Personnel") || User.IsInRole("Overall Approver") || User.IsInRole("Administrator"))){
                    dropdownActions +=
                        "<li><a href='javascript:void(0);' class='dropdown-btn btn-primary' " +
                        "onClick='DispatchAjax(" +
                            "`" + u.Id + "`," +
                            "`" + u.RequestId + "`," +
                            "`" + u.Status + "`," +
                            "`" + parcelCategory + "`," +
                            "`" + u.ParcelDescription + "`," +
                            "`" + u.Instruction + "`," +
                            "`" + u.Recipient + "`," +
                            "`" + destinationTag + "`," +
                            "`" + u.Destination + "`," +
                            "`" + u.DateTimeToBeReceived.ToString("MMM dd, yyyy HH:mm") + "`," +
                            "`" + u.FiledBy + "`," +
                            "`" + u.FiledDate.ToString("MMM dd, yyyy HH:mm") + "`" +
                        ");'>Dispatch</a></li>";
                    dropdownActions +=
                        "<li><a href='javascript:void(0);' class='dropdown-btn btn-danger' " +
                        "onClick='RejectAjax(" +
                            "`" + u.Id + "`," +
                            "`" + u.RequestId + "`," +
                            "`" + u.Status + "`," +
                            "`" + parcelCategory + "`," +
                            "`" + u.ParcelDescription + "`," +
                            "`" + u.Instruction + "`," +
                            "`" + u.Recipient + "`," +
                            "`" + destinationTag + "`," +
                            "`" + u.Destination + "`," +
                            "`" + u.DateTimeToBeReceived.ToString("MMM dd, yyyy HH:mm") + "`," +
                            "`" + u.FiledBy + "`," +
                            "`" + u.FiledDate.ToString("MMM dd, yyyy HH:mm") + "`" +
                        ");'>Reject</a></li>";
                }
                if (u.Status == "DISPATCHED" && (User.IsInRole("AGS Personnel") || User.IsInRole("Overall Approver") || User.IsInRole("Administrator")))
                {
                    dropdownActions +=
                        "<li><a href='javascript:void(0);' class='dropdown-btn btn-primary' " +
                        "onClick='DeliveredAjax(" +
                            "`" + u.Id + "`" +
                        ");'>Mark as Delivered</a></li>";
                }
                if ((u.Status == "DELIVERED") && (User.IsInRole("AGS Personnel") || User.IsInRole("Overall Approver") || User.IsInRole("Administrator")))
                {
                    dropdownActions +=
                        "<li><a href='javascript:void(0);' class='dropdown-btn btn-primary' " +
                        "onClick='ReceivedAjax(" +
                            "`" + u.Id + "`" +
                        ");'>Mark as Received</a></li>";
                }

                dropdownActions += "</ul></div>";
                var sender = await _userManager.FindByNameAsync(u.FiledBy);
                DataList.Add(new
                {
                    Action = dropdownActions,
                    Status = u.Status,
                    RequestId = u.RequestId,
                    FiledBy = sender.DisplayName,
                    ParcelCategory = parcelCategory,
                    ParcelDescription = u.ParcelDescription,
                    Recipient = u.Recipient,
                    RecipientEmail = u.RecipientEmail,
                    RecipientDepartment = u.RecipientDepartment,
                    DestinationTag = destinationTag,
                    Destination = u.Destination,
                    DateTimeToBeReceived = u.DateTimeToBeReceived.ToString("MM/dd/yyyy hh:mm tt"),
                    ActualDispatchDate = actualDispatchDate,
                    ActualReceivedDate = actualReceivedDate,
                });
            }

            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            int recFilter = DataList.Count();

            var pagedData = DataList.Skip(startRec).Take(pageSize).ToList();

            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = recFilter,
                data = pagedData,
            };

            return new JsonResult(jsonData);
        }

        [HttpPost]
        public async Task<IActionResult> SaveParcelDelivery(ParcelDelivery parcelDelivery)
        {
            try
            {
                parcelDelivery.FiledBy = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var oldReceiverEmail = "";
                var parcelDeliveryId = parcelDelivery.Id;
                if (parcelDelivery.Id == 0)
                {
                    parcelDelivery.RequestId = await _requestIdSequenceService.GenerateRequestId();
                    parcelDelivery.Status = "SUBMITTED";
                    parcelDelivery.DateTimeToBeReceived = DateTime.Parse(
                        $"{parcelDelivery.DateToBeReceived} {parcelDelivery.TimeToBeReceived}"
                    );
                    _context.ParcelDeliveries.Add(parcelDelivery);
                }
                else
                {
                    var existingParcel = _context.ParcelDeliveries.Find(parcelDelivery.Id);
                    if (existingParcel == null)
                    {
                        return Json(new { success = false, message = "Parcel delivery record not found." });
                    }
                    oldReceiverEmail = existingParcel.RecipientEmail;
                    existingParcel.ChargingCompanyId = parcelDelivery.ChargingCompanyId;
                    existingParcel.ChargingDepartmentId = parcelDelivery.ChargingDepartmentId;
                    existingParcel.ParcelCategoryId = parcelDelivery.ParcelCategoryId;
                    existingParcel.DestinationId = parcelDelivery.DestinationId;
                    existingParcel.Destination = parcelDelivery.Destination;
                    existingParcel.Recipient = parcelDelivery.Recipient;
                    existingParcel.RecipientEmail = parcelDelivery.RecipientEmail;
                    existingParcel.RecipientDepartment = parcelDelivery.RecipientDepartment;
                    existingParcel.DateTimeToBeReceived = DateTime.Parse(
                        $"{parcelDelivery.DateToBeReceived} {parcelDelivery.TimeToBeReceived}"
                    );
                    existingParcel.ParcelDescription = parcelDelivery.ParcelDescription;
                    existingParcel.Instruction = parcelDelivery.Instruction;
                    existingParcel.UpdatedBy = User.Identity.Name;
                    existingParcel.UpdatedDate = DateTime.Now;
                    _context.ParcelDeliveries.Update(existingParcel);
                }

                await _context.SaveChangesAsync();
                if (parcelDeliveryId == 0)
                {
                    var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcelDelivery.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                    var message =   $"<div>Request Id : <u>{parcelDelivery.RequestId}</u></div>" +
                                    $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                    $"<div>Parcel Description : <u>{parcelDelivery.ParcelDescription}</u></div>" +
                                    $"<hr>" +
                                    $"<div>A parcel delivery has been submitted by <b>{user.DisplayName}</b></div>" +
                                    $"<div>To be received by <b>{parcelDelivery.Recipient}<b/></div>"                                    
                                    ;
                    //SendParcelEmail(parcelDelivery.RecipientEmail, userEmail, message, parcelDelivery.RequestId);
                }
                else
                {
                    var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcelDelivery.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                    var reciepients = parcelDelivery.RecipientEmail;
                    if (!parcelDelivery.RecipientEmail.Equals(oldReceiverEmail))
                    {
                        reciepients += "," + oldReceiverEmail;
                    }
                    var existingParcel = _context.ParcelDeliveries.Find(parcelDelivery.Id);
                    var message =   $"<div>Request Id : <u>{existingParcel.RequestId}</u></div>" +
                                    $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                    $"<div>Parcel Description : <u>{existingParcel.ParcelDescription}</u></div>" +
                                    $"<hr>" +
                                    $"<div>The parcel delivery details has been updated by <b>{user.DisplayName}</b></div>" +
                                    $"<div>To be received by <b>{parcelDelivery.Recipient}<b/></div>"
                                    ;
                    //SendParcelEmail(reciepients, userEmail, message, existingParcel.RequestId);
                }
                return Json(new
                {
                    success = true,
                    message = "Parcel delivery record saved successfully."
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while saving parcel delivery record: " + ex.Message
                });
            };
        }

        [HttpPost]
        public async Task<IActionResult> DispatchParcelDelivery(string dispatchDate, string dispatchTime, int parcelId, int vehicleId, int driverId, string dispatchRemarks)
        {
            try
            {
                var parcel = _context.ParcelDeliveries.Find(parcelId);
                if (parcel == null)
                {
                    return Json(new { success = false, message = "Parcel delivery record not found." });
                }
                parcel.Status = "DISPATCHED";
                parcel.ActualDispatchDate = DateTime.Parse(
                        $"{dispatchDate} {dispatchTime}"
                    );
                parcel.VehicleListId = vehicleId;
                parcel.DriverId = driverId;
                parcel.DeliveryRemarks = dispatchRemarks;
                parcel.UpdatedBy = User.Identity.Name;
                parcel.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcel.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                var destinationTag = _context.OriginDestination
                        .Where(d => d.Id == parcel.DestinationId)
                        .Select(d => d.OriginDestinationName)
                        .FirstOrDefault();
                var vehicle = _context.VehicleLists.Where(x => x.Id == vehicleId).FirstOrDefault();
                var driverName = _context.Drivers.Where(x => x.Id == driverId).Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
                var driverNumber = _context.Drivers.Where(x => x.Id == driverId).Select(x => x.MobileNumber).FirstOrDefault();
                if(driverNumber != null && driverNumber != "")                {
                    
                    var textMsg = $"THS Parcel Delivery" + Environment.NewLine +
                        $"Request Id [{parcel.RequestId}] has been assigned to you." + Environment.NewLine +
                        $"Assigned Vehicle: {vehicle.Model}" + Environment.NewLine +
                        $"Vehicle Plate Number: {vehicle.PlateNumber}" + Environment.NewLine + Environment.NewLine +
                        $"Destination: {destinationTag} | {parcel.Destination}" + Environment.NewLine +
                        $"Sender: {parcel.FiledBy}" + Environment.NewLine +
                        $"Recipient: {parcel.Recipient}" + Environment.NewLine +
                        $"Parcel Details: {parcelCategory} | {parcel.ParcelDescription}" + Environment.NewLine +
                        $"Instructions: {parcel.Instruction}";
                    var systemName = "THS Makati";

                    string smsStatus = await new ReportsController().SendSmsAsync(driverNumber, textMsg, parcel.RequestId, systemName);
                }

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var message =   $"<div>Request Id : <u>{parcel.RequestId}</u></div>" +
                                $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                $"<div>Parcel Description : <u>{parcel.ParcelDescription}</u></div>" +
                                $"<div>Assigned Driver : <u>{driverName}</u></div>" +
                                $"<div>Assigned Vehicle : <u>{vehicle.Model}</u></div>" +
                                $"<div>Assigned Vehicle Plate Number : <u>{vehicle.PlateNumber}</u></div>" +
                                $"<hr>" +
                                $"<div>The parcel has been <b>DISPATCHED</b> to {destinationTag} | {parcel.Destination}</div>" +
                                $"<div>To be received by <b>{parcel.Recipient}<b/></div>"
                                ;
                SendParcelEmail(userEmail, "", message, parcel.RequestId);
                message +=  $"<br/>" +
                            $"<div><b>Please click this <a href='{_baseUrl}/Parcel/ParcelReceived?requestId={parcel.RequestId}'>link</a> to tag it as RECEIVED upon receiving the item.</b></div>";
                SendParcelEmail(parcel.RecipientEmail, "", message, parcel.RequestId);
                return Json(new
                {
                    success = true,
                    message = "Parcel has been dispatched."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while saving parcel delivery record: " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReceivedParcelDelivery(int id)
        {
            try
            {
                var parcel = await _context.ParcelDeliveries.FindAsync(id);
                parcel.Status = "RECEIVED";
                parcel.ActualReceivedDate = DateTime.Now;
                _context.ParcelDeliveries.Update(parcel);
                await _context.SaveChangesAsync();

                var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcel.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                var destinationTag = _context.OriginDestination
                        .Where(d => d.Id == parcel.DestinationId)
                        .Select(d => d.OriginDestinationName)
                        .FirstOrDefault();
                var vehicle = _context.VehicleLists.Where(x => x.Id == parcel.VehicleListId).FirstOrDefault();
                var driverName = _context.Drivers.Where(x => x.Id == parcel.DriverId).Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var message = $"Request Id : <u>{parcel.RequestId}</u></div>" +
                                $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                $"<div>Parcel Description : <u>{parcel.ParcelDescription}</u></div>" +
                                $"<div>Assigned Driver : <u>{driverName}</u></div>" +
                                $"<div>Assigned Vehicle : <u>{vehicle.Model}</u></div>" +
                                $"<div>Assigned Vehicle Plate Number : <u>{vehicle.PlateNumber}</u></div>" +
                                $"<hr>" +
                                $"<div>The parcel has been <b>RECEIVED</b> by {parcel.Recipient} at {parcel.ActualReceivedDate}</b></div>"
                                ;
                SendParcelEmail(userEmail, parcel.RecipientEmail, message, parcel.RequestId);
                return Json(new
                {
                    success = true,
                    message = "Parcel has been marked as received."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while receiving parcel delivery record: " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeliverParcelDelivery(int id)
        {
            try
            {
                var parcel = await _context.ParcelDeliveries.FindAsync(id);
                parcel.Status = "DELIVERED";
                _context.ParcelDeliveries.Update(parcel);
                await _context.SaveChangesAsync();
                var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcel.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                var destinationTag = _context.OriginDestination
                        .Where(d => d.Id == parcel.DestinationId)
                        .Select(d => d.OriginDestinationName)
                        .FirstOrDefault();
                var vehicle = _context.VehicleLists.Where(x => x.Id == parcel.VehicleListId).FirstOrDefault();
                var driverName = _context.Drivers.Where(x => x.Id == parcel.DriverId).Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var message = $"Request Id : <u>{parcel.RequestId}</u></div>" +
                                $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                $"<div>Parcel Description : <u>{parcel.ParcelDescription}</u></div>" +
                                $"<div>Assigned Driver : <u>{driverName}</u></div>" +
                                $"<div>Assigned Vehicle : <u>{vehicle.Model}</u></div>" +
                                $"<div>Assigned Vehicle Plate Number : <u>{vehicle.PlateNumber}</u></div>" +
                                $"<hr>" +
                                $"<div>The parcel has been <b>DELIVERED</b> to {parcel.Recipient}</b></div>"
                                ;
                SendParcelEmail(userEmail, "", message, parcel.RequestId);
                message +=  $"<br/>" +
                            $"<div><b>Please click this <a href='{_baseUrl}/Parcel/ParcelReceived?requestId={parcel.RequestId}'>link</a> to tag it as RECEIVED upon receiving the item.</b></div>";
                SendParcelEmail(parcel.RecipientEmail, "", message, parcel.RequestId);
                return Json(new
                {
                    success = true,
                    message = "Parcel has been marked as delivered."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while receiving parcel delivery record: " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RejectParcelDelivery(int rejectParcelId, string rejectReason)
        {
            try
            {
                var parcel = await _context.ParcelDeliveries.FindAsync(rejectParcelId);
                parcel.Status = "REJECTED";
                parcel.RejectedBy = User.Identity.Name;
                parcel.RejectedDate = DateTime.Now;
                parcel.RejectionReason = rejectReason;
                _context.ParcelDeliveries.Update(parcel);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userEmail = user.Email;
                var parcelCategory = _context.ParcelCategories
                        .Where(d => d.Id == parcel.ParcelCategoryId)
                        .Select(d => d.Category)
                        .FirstOrDefault();
                var message =   $"<div>Request Id : <u>{parcel.RequestId}</u></div>" +
                                $"<div>Parcel Category : <u>{parcelCategory}</u></div>" +
                                $"<div>Parcel Description : <u>{parcel.ParcelDescription}</u></div>" +
                                $"<hr>" +
                                $"<div>The parcel delivery request has been <b style='color: red;'>REJECTED</b> by <b>{parcel.RejectedBy}</b></div>" +
                                $"<div>Rejection Reason : {parcel.RejectionReason}</div>"
                                ;
                SendParcelEmail(userEmail, "", message, parcel.RequestId);
                return Json(new
                {
                    success = true,
                    message = "Parcel has been marked as rejected."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while rejecting parcel delivery record: " + ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ParcelReport(DateTime? from, DateTime? to)
        {
            var parcels = await _context.ParcelDeliveries
                .AsNoTracking()
                .Where(x => x.ActualDispatchDate.Date != DateTime.MinValue)
                .ToListAsync();
            if (from.HasValue)
            {
                var startOfDay = from.Value.Date;
                parcels = parcels.Where(x => x.ActualDispatchDate >= startOfDay).ToList();
            }
            if (to.HasValue)
            {
                var endOfDay = to.Value.Date
                                   .AddDays(1)
                                   .AddSeconds(-1);
                parcels = parcels.Where(x => x.ActualDispatchDate <= endOfDay).ToList();
            }

            var entities = parcels
                .OrderBy(x => x.ActualDispatchDate)
                .ToList();
            var reportData = new List<ParcelReport>();
            var parcelCategoryDict = _context.ParcelCategories
                .ToDictionary(c => c.Id, c => c.Category);
            var driverDict = _context.Drivers
                .ToDictionary(c => c.Id, c => $"{c.FirstName} {c.LastName}");
            var plateDict = _context.VehicleLists
                .ToDictionary(c => c.Id, c => c.PlateNumber);
            for (int i = 0; i < entities.Count(); i++)
            {
                var entity = entities[i];
                var sender = await _userManager.FindByNameAsync(entity.FiledBy);
                var senderDept = await _context.DepartmentLists.FindAsync(sender.DepartmentId);
                reportData.Add(new ParcelReport
                {
                    No = i + 1,
                    Sender = $"{sender.DisplayName} [{senderDept.DepartmentName}]",
                    Receiver = $"{entity.Recipient} [{entity.RecipientDepartment}]",
                    Origin = "SMPC - HO",
                    Destination = entity.Destination,
                    RequestId = entity.RequestId,
                    Type = parcelCategoryDict[entity.ParcelCategoryId],
                    Description = entity.ParcelDescription,
                    Driver = driverDict[entity.DriverId ?? 0],
                    PlateNumber = plateDict[entity.VehicleListId ?? 0],
                    ReceivedBy = entity.Recipient,
                    ReceivedDate = entity.ActualReceivedDate == DateTime.MinValue ? "" : entity.ActualReceivedDate.ToString("MMM dd, yyyy HH:mm")
                });
            }

            DataTable dt = reportData.ToDataTable();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Parcel Delivery");
                worksheet.Cell("A1").Value = "Parcel Delivery Report Summary";
                var filter = "";
                if (from.HasValue && to.HasValue)
                {
                    filter += $"From {from:MMM dd, yyy} - To {to:MMM dd, yyy}";
                }
                else if (from.HasValue)
                {
                    filter += $"From {from:MMM dd, yyy}";
                }
                else if (to.HasValue)
                {
                    filter += $"Up to {to:MMM dd, yyy}";
                }
                worksheet.Cell("A2").Value = filter;
                worksheet.Cell("A4").InsertTable(dt);
                worksheet.Columns().AdjustToContents();
                worksheet.Column(1).Width = 5;

                worksheet.Rows().AdjustToContents();
                var usedRange = worksheet.RangeUsed();
                usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    var bytes = ms.ToArray();
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"THS_ParcelDeliveryReport_{DateTime.Today:yyyyMMdd}.xlsx");
                }
                    
            }
            
        }

        public async void SendParcelEmail(string receipients, string cc, string message, string requestId)
        {
            new ReportsController().SendEmail("", receipients, cc, message, $"Parcel Delivery | {requestId}");
        }
    }
}
