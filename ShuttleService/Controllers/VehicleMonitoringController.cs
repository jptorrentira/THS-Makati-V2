using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShuttleService.Data;
using ShuttleService.Models;

namespace ShuttleService.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class VehicleMonitoringController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public VehicleMonitoringController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var currentCompanyGroupName = _context.CompanyGroup.Find(currentCompanyGroupId).CompanyGroupName;
            if(currentCompanyGroupName != "MINESITE")
            {
                return RedirectToAction("AccessDenied", "Accounts");
            }

            var PlateNumbersList = _context.VehicleLists
                .Select(v => v.PlateNumber.Trim())
                .ToList();
            ViewBag.PlateNumbers = new SelectList(PlateNumbersList);

            var DriverList = _context.Drivers
                .Select(d => new
                {
                    d.Id,
                    FullName = d.FirstName + " " + d.LastName
                })
                .OrderBy(d => d.FullName)
                .ToList();
            ViewBag.Drivers = new SelectList(DriverList, "Id", "FullName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetVehicleMonitoringData()
        {
            // Initialization.
            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            // Get today's date (start and end of day)
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Filter only today's vehicles
            var vehicles = _context.VehicleMonitor
                .Where(v => v.TripDateTime >= today && v.TripDateTime < tomorrow).AsEnumerable();

            // 🔎 Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                vehicles = vehicles.Where(u =>
                    u.PlateNumber.Contains(search) ||
                    u.Model.Contains(search) ||
                    u.Origin.Contains(search) ||
                    u.Destination.Contains(search) ||
                    _context.Drivers
                        .Where(d => d.Id == u.DriverId)
                        .Select(d => d.FirstName + " " + d.LastName)
                        .FirstOrDefault().Contains(search));
            }

            var DataList = vehicles.Select(u => new
            {
                Action = 
                    "<a href='javascript:void(0);' class='btn btn-sm btn-info' " +
                        "onClick='ViewRemarksAjax(`" + u.Remarks + "`);'>Remarks</a> " +
                     "<a href='javascript:void(0);' class='btn btn-sm btn-primary' " +
                        "onClick='EditAjax(" +
                            "`" + u.Id + "`," +
                            "`" + u.TripDateTime.ToString("yyyy-MM-ddTHH:mm") + "`," +
                            "`" + u.PlateNumber + "`," +
                            "`" + u.Model + "`," +
                            "`" + u.Capacity + "`," +
                            "`" + (_context.Drivers.Where(d => d.Id == u.DriverId)
                                 .Select(d => d.Id).FirstOrDefault()) + "`," +
                            "`" + u.Status + "`," +
                            "`" + u.Origin + "`," +
                            "`" + u.Destination + "`," +
                            "`" + u.Remarks + "`" +
                        ");'>Edit</a>",
                TripDateTime = u.TripDateTime.ToString("MM/dd/yyyy hh:mm tt"),
                PlateNumber = u.PlateNumber,
                Model = u.Model,
                Capacity = u.Capacity,
                Driver = _context.Drivers
                    .Where(d => d.Id == u.DriverId)
                    .Select(d => d.FirstName + " " + d.LastName)
                    .FirstOrDefault(),
                Status = u.Status,
                Origin = u.Origin,
                Destination = u.Destination
            });

            int totalRecords = DataList.Count();
            string[] sort = new string[] { "Id" };
            //var sortfield = sort[int.Parse(order)];
            int recFilter = DataList.Count();

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
        public async Task<IActionResult> GetVehicleMonitoringDataReport(DateTime? fromDate, DateTime? toDate)
        {
            var vehicles = _context.VehicleMonitor.AsQueryable();

            if (fromDate.HasValue)
                vehicles = vehicles.Where(v => v.TripDateTime >= fromDate.Value);

            if (toDate.HasValue)
                vehicles = vehicles.Where(v => v.TripDateTime <= toDate.Value.AddDays(1)); // include full day

            var dataList = vehicles.Select(u => new {
                TripDateTime = u.TripDateTime.ToString("MM/dd/yyyy hh:mm tt"),
                u.PlateNumber,
                u.Model,
                u.Capacity,
                Driver = _context.Drivers.Where(d => d.Id == u.DriverId)
                                         .Select(d => d.FirstName + " " + d.LastName)
                                         .FirstOrDefault(),
                u.Status,
                u.Origin,
                u.Destination
            }).ToList();

            return Json(new { data = dataList });
        }

        [HttpGet]
        public IActionResult GetVehicleDetails(string plateNumber)
        {
            var vehicle = _context.VehicleLists
                .Where(v => v.PlateNumber.Trim() == plateNumber.Trim())
                .Select(v => new
                {
                    v.Model,
                    v.PlateNumber,
                    v.Capacity
                })
                .FirstOrDefault();
            if (vehicle == null)
            {
                return Json(new { success = false, message = "Vehicle not found." });
            }
            return Json(new { success = true, data = vehicle });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDispatch(VehicleMonitor vm)
        {
            try
            {
                vm.EncodedBy = User.Identity.Name;
                if (vm.Id == 0) // New record
                {
                    await _context.VehicleMonitor.AddAsync(vm);
                }
                else // ✏️ Existing record -> update
                {
                    var existing = await _context.VehicleMonitor.FindAsync(vm.Id);
                    if (existing == null)
                        return Json(new { success = false, message = "Vehicle record not found." });

                    // Update fields
                    existing.TripDateTime = vm.TripDateTime;
                    existing.PlateNumber = vm.PlateNumber;
                    existing.Model = vm.Model;
                    existing.Capacity = vm.Capacity;
                    existing.DriverId = vm.DriverId;
                    existing.Status = vm.Status;
                    existing.Origin = vm.Origin;
                    existing.Destination = vm.Destination;
                    existing.Remarks = vm.Remarks;
                    existing.EncodedBy = vm.EncodedBy;
                    existing.EncodedDate = DateTime.Now; // optional timestamp
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Vehicle dispatched successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }            
        }
    }
}
