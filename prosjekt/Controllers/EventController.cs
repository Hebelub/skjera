using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;
using prosjekt.Models;

namespace prosjekt.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event/
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "All Events";
            
            return View(await _context.EventModels.ToListAsync());
        }
        
        // GET: Event/Organization/5
        public async Task<IActionResult> Organization(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var organization = await _context.OrganizationModels
                .FirstOrDefaultAsync(m => m.Id == id);

            if (organization == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Events Of Organization: " + organization.Name;

            return View("Index", await _context.EventModels
                .Where(e => e.OrganizerId == id)
                .ToListAsync());
        }
        
        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Info,StartTime,EndTime")] EventModel eventModel)
        {
            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }
            
            _context.Add(eventModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels.FindAsync(id);
            
            if (eventModel == null)
            {
                return NotFound();
            }
            
            return View(eventModel);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Info,StartTime,EndTime,LastTimeEdited")] EventModel eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }

            try
            {
                _context.Update(eventModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventModelExists(eventModel.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.EventModels.FindAsync(id);

            if (eventModel != null)
            {
                var organizationId = eventModel.OrganizerId;
                _context.EventModels.Remove(eventModel);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Organization), new { id=organizationId });
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventModelExists(int id)
        {
            return _context.EventModels.Any(e => e.Id == id);
        }
    }
}
