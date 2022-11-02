using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;
using prosjekt.Models;

namespace prosjekt.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organization
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrganizationModels.ToListAsync());
        }

        // GET: Organization/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            return RedirectToAction("Organization", "Event", new { area = "", id=id });
        }

        // GET: Organization/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Organization/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,Description")] OrganizationModel organizationModel)
        {
            if (!ModelState.IsValid)
            {
                return View(organizationModel);
            }
            
            _context.Add(organizationModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Organization/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var organizationModel = await _context.OrganizationModels.FindAsync(id);
            
            if (organizationModel == null)
            {
                return NotFound();
            }
            
            return View(organizationModel);
        }

        // POST: Organization/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description")] OrganizationModel organizationModel)
        {
            if (id != organizationModel.Id)
            {
                return NotFound();
            }

            if (! ModelState.IsValid)
            {
                return View(organizationModel);
            }
            
            try
            {
                _context.Update(organizationModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationModelExists(organizationModel.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Organization/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var organizationModel = await _context.OrganizationModels
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (organizationModel == null)
            {
                return NotFound();
            }
            
            return View(organizationModel);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organizationModel = await _context.OrganizationModels.FindAsync(id);
      
            if (organizationModel != null)
            {
                _context.OrganizationModels.Remove(organizationModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationModelExists(int id)
        {
            return _context.OrganizationModels.Any(e => e.Id == id);
        }
    }
}
