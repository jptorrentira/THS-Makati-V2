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
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ShuttleService.Controllers
{
    public class DepartmentListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DepartmentLists
        [Authorize(Policy = "RequireAdminAGSRole")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var CompanyListId = _context.CompanyLists.OrderBy(m => m.CompanyName)
                .Where(c => c.CompanyGroupId == currentCompanyGroupId);
            ViewData["CompanyListId"] = new SelectList(CompanyListId, "Id", "CompanyName");

            return View();
        }

        // GET: DepartmentLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentList = await _context.DepartmentLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departmentList == null)
            {
                return NotFound();
            }

            return View(departmentList);
        }

        // GET: DepartmentLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DepartmentLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartmentName,ShortName,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] DepartmentList departmentList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(departmentList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentList);
        }

        // GET: DepartmentLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentList = await _context.DepartmentLists.FindAsync(id);
            if (departmentList == null)
            {
                return NotFound();
            }
            return View(departmentList);
        }

        // POST: DepartmentLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentName,ShortName,CompanyListId,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] DepartmentList departmentList)
        {

            var returnSuccess = 1;
            var returnMessage = "";

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdentity = _context.Users.FirstOrDefault(e => e.Id == userId);
                    var comp_ = _context.DepartmentLists.FirstOrDefault(e => e.Id.Equals(id));

                    if (comp_ == null) //add
                    {

                        //_context.Database.ExecuteSqlCommand("execute ResetAutoIncrement");
                        var comp = new DepartmentList();
                        comp.EncodedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.ModifyDate = DateTime.Now;
                        comp.EncodeDate = DateTime.Now;
                        comp.DepartmentName = departmentList.DepartmentName;
                        comp.ShortName = departmentList.ShortName;
                        comp.CompanyListId = departmentList.CompanyListId;

                        var currentUser = await _userManager.GetUserAsync(User);
                        var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                            .Select(e => e.CompanyGroupId).FirstOrDefault();
                        comp.CompanyGroupId = currentCompanyGroupId;
                        //_context.Add(comp);
                        // await _context.SaveChangesAsync();

                        var compCharge = new ChargingDepartment();
                        compCharge.EncodedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.ModifyDate = DateTime.Now;
                        compCharge.EncodeDate = DateTime.Now;
                        compCharge.DepartmentName = departmentList.DepartmentName;
                        compCharge.CompanyGroupId = currentCompanyGroupId;
                        compCharge.ShortName = departmentList.ShortName;
                        compCharge.ChargingCompanyId = departmentList.CompanyListId;
                        compCharge.Status = "1";

                        _context.Add(comp);
                        _context.Add(compCharge);
                        await _context.SaveChangesAsync();

                    }
                    else
                    { //update

                        var comp = _context.DepartmentLists.FirstOrDefault(e => e.Id.Equals(id));
                        comp.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.ModifyDate = DateTime.Now;
                        comp.DepartmentName = departmentList.DepartmentName;
                        comp.ShortName = departmentList.ShortName;
                        comp.CompanyListId = departmentList.CompanyListId;
                        var currentUser = await _userManager.GetUserAsync(User);
                        var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                            .Select(e => e.CompanyGroupId).FirstOrDefault();
                        comp.CompanyGroupId = currentCompanyGroupId;


                        var compCharge = _context.ChargingDepartments.FirstOrDefault(e => e.Id.Equals(id));
                        compCharge.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.ModifyDate = DateTime.Now;
                        compCharge.DepartmentName = departmentList.DepartmentName;
                        compCharge.ShortName = departmentList.ShortName;
                        compCharge.ChargingCompanyId = departmentList.CompanyListId;
                        compCharge.Status = "1";

                        _context.Update(comp);
                        _context.Update(compCharge);
                        await _context.SaveChangesAsync();


                    }


                    /* Save Login Result */
                    Log _loginAttempt = new Log();
                    _loginAttempt.ProcessedBy = userIdentity.Domain + "/" + userIdentity.UserName;
                    _loginAttempt.Process = "Manage Department Lists";
                    _loginAttempt.ProcessedDate = DateTime.Today;
                    _context.Add(_loginAttempt);
                    await _context.SaveChangesAsync();


                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.InnerException.Message;
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

        // GET: DepartmentLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentList = await _context.DepartmentLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departmentList == null)
            {
                return NotFound();
            }

            return View(departmentList);
        }

        // POST: DepartmentLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departmentList = await _context.DepartmentLists.FindAsync(id);
            _context.DepartmentLists.Remove(departmentList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentListExists(int id)
        {
            return _context.DepartmentLists.Any(e => e.Id == id);
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ShowLists()
        {

            string search = Request.Form["search[value]"];
            string draw = Request.Form["draw"];
            string order = Request.Form["order[0][column]"];
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);

            var currentUser = await _userManager.GetUserAsync(User);
            var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                .Select(e => e.CompanyGroupId).FirstOrDefault();
            var DataList = _context.DepartmentLists.OrderByDescending(u => u.Id).
                            Select(u => new
                            {
                                Id = u.Id,
                                Name = u.DepartmentName,
                                u.ShortName,
                                EncodeDate = u.EncodeDate == null ? " " : u.EncodeDate.ToString("MM/dd/yyyy"),
                                EncodedBy = u.EncodedBy,
                                ModifyDate = u.ModifyDate == null ? " " : u.ModifyDate.ToString("MM/dd/yyyy"),
                                LastModifiedBy = u.LastModifiedBy,
                                CompanyListId = u.CompanyListId,
                                CompanyListName = _context.CompanyLists.FirstOrDefault(e=> e.Id.Equals(u.CompanyListId)).CompanyName,
                                CompanyGroupId = u.CompanyGroupId


                            });

            DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);

            // Total record count.
            int totalRecords = _context.DepartmentLists.Count();

            // Verification.
            if (!string.IsNullOrEmpty(search))
            {   // Apply search
                DataList = DataList.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.ShortName.Contains(search.ToLower()) || x.EncodedBy.Contains(search.ToLower()) || x.LastModifiedBy.Contains(search.ToLower()));
            }
            // Sorting.
            //string[] sort = new string[] { "Name", "Name" };
            //var sortfield = sort[int.Parse(order)];
            //DataList = DataList.OrderBy(x=> x.Name);

            // Filter record count.
            int recFilter = _context.DepartmentLists.Count();

            // DataList = DataList.OrderBy(o => o.FirstName +" "+ o.LastName);

            // Apply pagination.
            DataList = DataList.Skip(startRec).Take(pageSize);
            DataList = DataList.OrderBy(x => x.CompanyListName).ThenBy(x => x.Name);
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
        public ActionResult ShowData(int id)
        {
            string message_ = "";
            string DepartmentName_ = "";
            string ShortName_ = "";
            string CompanyListId_ = "";
            int success_ = 1;

            var departmentlist = _context.DepartmentLists.FirstOrDefault(e => e.Id.Equals(id));
            if (id == null)
            {
                success_ = 0;
                message_ = "Data not found";
            }
            DepartmentName_ = departmentlist.DepartmentName;
            ShortName_ = departmentlist.ShortName;
            CompanyListId_ = departmentlist.CompanyListId.ToString();



            var jsonData = new
            {
                recordId = id,
                message = message_,
                DepartmentName = DepartmentName_,
                ShortName = ShortName_,
                CompanyListId = CompanyListId_,
                success = success_
            };

            return new JsonResult(jsonData);

        }



    }
}
