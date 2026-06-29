using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Models;

using Microsoft.AspNetCore.Http;
namespace ShuttleService.Controllers
{
    public class DefaultApproversController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DefaultApproversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DefaultApprovers
        public async Task<IActionResult> Index()
        {
            return View(await _context.DefaultApprovers.ToListAsync());
        }

        // GET: DefaultApprovers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultApprover = await _context.DefaultApprovers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (defaultApprover == null)
            {
                return NotFound();
            }

            return View(defaultApprover);
        }

        // GET: DefaultApprovers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DefaultApprovers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApproverEmployeeNo,EmployeeNo,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] DefaultApprover defaultApprover)
        {
            if (ModelState.IsValid)
            {
                _context.Add(defaultApprover);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(defaultApprover);
        }

        // GET: DefaultApprovers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultApprover = await _context.DefaultApprovers.FindAsync(id);
            if (defaultApprover == null)
            {
                return NotFound();
            }
            return View(defaultApprover);
        }

        // POST: DefaultApprovers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApproverEmployeeNo,EmployeeNo,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] DefaultApprover defaultApprover)
        {
            if (id != defaultApprover.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(defaultApprover);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DefaultApproverExists(defaultApprover.Id))
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
            return View(defaultApprover);
        }

        // GET: DefaultApprovers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var defaultApprover = await _context.DefaultApprovers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (defaultApprover == null)
            {
                return NotFound();
            }

            return View(defaultApprover);
        }

        // POST: DefaultApprovers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var defaultApprover = await _context.DefaultApprovers.FindAsync(id);
            _context.DefaultApprovers.Remove(defaultApprover);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DefaultApproverExists(int id)
        {
            return _context.DefaultApprovers.Any(e => e.Id == id);
        }
    }
}
