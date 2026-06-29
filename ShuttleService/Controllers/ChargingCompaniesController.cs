using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Models;
using System.Security.Claims;

namespace ShuttleService.Controllers
{
    public class ChargingCompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChargingCompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChargingCompanies
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChargingCompanys.ToListAsync());
        }

        // GET: ChargingCompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chargingCompany = await _context.ChargingCompanys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chargingCompany == null)
            {
                return NotFound();
            }

            return View(chargingCompany);
        }

        // GET: ChargingCompanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChargingCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,ShortName,Status,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] ChargingCompany chargingCompany)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chargingCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chargingCompany);
        }

        // GET: ChargingCompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chargingCompany = await _context.ChargingCompanys.FindAsync(id);
            if (chargingCompany == null)
            {
                return NotFound();
            }
            return View(chargingCompany);
        }

        // POST: ChargingCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,ShortName,Status,EncodedBy,LastModifiedBy,EncodeDate,ModifyDate")] ChargingCompany chargingCompany)
        {
            if (id != chargingCompany.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chargingCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChargingCompanyExists(chargingCompany.Id))
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
            return View(chargingCompany);
        }

        // GET: ChargingCompanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chargingCompany = await _context.ChargingCompanys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chargingCompany == null)
            {
                return NotFound();
            }

            return View(chargingCompany);
        }

        // POST: ChargingCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chargingCompany = await _context.ChargingCompanys.FindAsync(id);
            _context.ChargingCompanys.Remove(chargingCompany);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChargingCompanyExists(int id)
        {
            return _context.ChargingCompanys.Any(e => e.Id == id);
        }
    }
}
