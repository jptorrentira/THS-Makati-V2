using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
//using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Hosting;
using ShuttleService.Data;
using ShuttleService.Models;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace ShuttleService.Controllers
{
    public class CompanyListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CompanyLists
        [Authorize(Policy = "RequireAdminAGSRole")]
        public async Task<IActionResult> Index()
        {

            // var blogs = _context.Database.("EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser=@user", User);

            //_context.Database.ExecuteSqlCommand("execute ResetAutoIncrement");

            var CompanyGroupList = _context.CompanyGroup.OrderBy(m => m.Id).ToList();
            ViewData["CompanyGroupId"] = new SelectList(CompanyGroupList, "Id", "CompanyGroupName");

            return View(await _context.CompanyLists.ToListAsync());
        }

        // GET: CompanyLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyList = await _context.CompanyLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyList == null)
            {
                return NotFound();
            }

            return View(companyList);
        }

        // GET: CompanyLists/Create
        public IActionResult Create()
        {

//            _context.Database.ExecuteSqlCommand("execute ResetAutoIncrement");
            return View();
        }

        // POST: CompanyLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyList companyList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyList);
        }

        // GET: CompanyLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyList = await _context.CompanyLists.FindAsync(id);
            if (companyList == null)
            {
                return NotFound();
            }
            return View(companyList);
        }

        // POST: CompanyLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CompanyList companyList)
        {
           
            var returnSuccess = 1;
            var returnMessage = "";

            bool isAlreadyExist = await _context.CompanyLists
                .AnyAsync(c => c.CompanyName.Trim() == companyList.CompanyName.Trim() && c.CompanyGroupId == companyList.CompanyGroupId);
            if (isAlreadyExist)
            {
                var jsonDataError = new
                {
                    message = "Company Name already exists within the Company Group.",
                    success = 0
                };
                return new JsonResult(jsonDataError);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdentity = _context.Users.FirstOrDefault(e => e.Id == userId);
                    var comp_ = _context.CompanyLists.FirstOrDefault(e => e.Id.Equals(id));

                    if (comp_ == null) //add
                    {

                        //_context.Database.ExecuteSqlCommand("execute ResetAutoIncrement");
                        var comp = new CompanyList();
                        comp.EncodedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.ModifyDate = DateTime.Now;
                        comp.EncodeDate = DateTime.Now;
                        comp.CompanyName = companyList.CompanyName;
                        comp.ShortName = companyList.ShortName;

                        var currentUser = await _userManager.GetUserAsync(User);
                        var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                            .Select(e => e.CompanyGroupId).FirstOrDefault();
                        comp.CompanyGroupId = companyList.CompanyGroupId;

                        //_context.Add(comp);
                        // await _context.SaveChangesAsync();

                        var compCharge = new ChargingCompany();
                        compCharge.EncodedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.ModifyDate = DateTime.Now;
                        compCharge.EncodeDate = DateTime.Now;
                        compCharge.CompanyName = companyList.CompanyName;
                        compCharge.CompanyGroupId = companyList.CompanyGroupId;
                        compCharge.ShortName = companyList.ShortName;
                        compCharge.Status = "1";

                        _context.Add(comp);
                        _context.Add(compCharge);
                        await _context.SaveChangesAsync();

                    }
                    else
                    { //update

                        var comp = _context.CompanyLists.FirstOrDefault(e => e.Id.Equals(id));
                        comp.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        comp.ModifyDate = DateTime.Now;
                        comp.CompanyName = companyList.CompanyName;
                        comp.ShortName = companyList.ShortName;

                        var currentUser = await _userManager.GetUserAsync(User);
                        var currentCompanyGroupId = _context.Employees.Where(emp => emp.Id == currentUser.EmployeeId)
                            .Select(e => e.CompanyGroupId).FirstOrDefault();
                        comp.CompanyGroupId = currentCompanyGroupId;

                        var compCharge = _context.ChargingCompanys.FirstOrDefault(e => e.Id.Equals(id));
                        compCharge.LastModifiedBy = userIdentity.Domain + "\\" + userIdentity.UserName;
                        compCharge.ModifyDate = DateTime.Now;
                        compCharge.CompanyName = companyList.CompanyName;
                        compCharge.ShortName = companyList.ShortName;
                        compCharge.Status = "1";

                        _context.Update(comp);
                        _context.Update(compCharge);
                        await _context.SaveChangesAsync();


                    }


                    /* Save Login Result */
                    Log _loginAttempt = new Log();
                    _loginAttempt.ProcessedBy = userIdentity.Domain + "/" + userIdentity.UserName;
                    _loginAttempt.Process = "Manage Company Lists";
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

        // GET: CompanyLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyList = await _context.CompanyLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyList == null)
            {
                return NotFound();
            }

            return View(companyList);
        }

        // POST: CompanyLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyList = await _context.CompanyLists.FindAsync(id);
            _context.CompanyLists.Remove(companyList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyListExists(int id)
        {
            return _context.CompanyLists.Any(e => e.Id == id);
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

            var DataList = _context.CompanyLists.OrderByDescending(u => u.Id)
                            .Select(u => new
                            {
                                Id = u.Id,
                                Name = u.CompanyName,
                                u.ShortName,
                                EncodeDate = u.EncodeDate == null ? " " : u.EncodeDate.ToString("MM/dd/yyyy"),
                                EncodedBy = u.EncodedBy,
                                ModifyDate = u.ModifyDate == null ? " " : u.ModifyDate.ToString("MM/dd/yyyy"),
                                LastModifiedBy = u.LastModifiedBy,
                                CompanyGroupId = u.CompanyGroupId
                            });

            DataList = DataList.Where(d => d.CompanyGroupId == currentCompanyGroupId);

            // Total record count.
            int totalRecords = DataList.Count();

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
            int recFilter = DataList.Count();

            // DataList = DataList.OrderBy(o => o.FirstName +" "+ o.LastName);

            // Apply pagination.
            DataList = DataList.Skip(startRec).Take(pageSize);
            DataList = DataList.OrderBy(x => x.Name);
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
            string CompanyName_ = "";
            string ShortName_ = "";
            int success_ = 1;

            var companyList = _context.CompanyLists.FirstOrDefault(e => e.Id.Equals(id));
            if (id == null)
            {
                success_ = 0;
                message_ = "Data not found";
            }
            CompanyName_ = companyList.CompanyName;
            ShortName_ = companyList.ShortName;
           


            var jsonData = new
            {
                recordId = id,
                message = message_,
                CompanyName = CompanyName_,
                ShortName = ShortName_,
                success = success_
            };

            return new JsonResult(jsonData);

        }



        [HttpPost]
        public async Task<JsonResult> getCompanyList()
        {

            var _keyword = string.IsNullOrWhiteSpace(Request.Form["q"]) ? "" : Request.Form["q"].ToString().ToLower();
            var CompanyList = _context.CompanyLists.Where(r =>  r.CompanyName.Contains(_keyword))
                                                           .Select(u => new
                                                           {
                                                               Id = u.Id,
                                                               Text = u.CompanyName
                                                           });



            var jsonData = new
            {
                Items = CompanyList.ToList().ToArray(),
                Count = CompanyList.Count()
            };

            return new JsonResult(jsonData);

        }
    }
}
