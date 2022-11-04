using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public EventController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Event/
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "All Events";
            
            return View(await _context.EventModels.ToListAsync());
        }
        
        // GET: Event/Organization/5
        public async Task<IActionResult> Organization(int id)
        {
            var organization = await _context.OrganizationModels
                .FirstOrDefaultAsync(m => m.Id == id);

            if (organization == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Events Of Organization: " + organization.Name;
            ViewData["OrganizationId"] = id;

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

        // GET: Event/Create/5
        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var organization = await _context.OrganizationModels.FirstOrDefaultAsync(o => o.Id == id);

            if (organization == null)
            {
                return NotFound();
            }

            if (!OrganizationAccess(id).CanCreateEvents)
            {
                return NotFound();
            }

            // Creating the model with the correct organization as the parent
            var model = new EventModel();

            model.Organizer = organization;
            
            return View(model);
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Info,StartTime,EndTime")] EventModel eventModel, int id)
        {
            var organization = await _context.OrganizationModels.FirstOrDefaultAsync(o => o.Id == id);

            if (organization == null)
            {
                return NotFound();
            }

            if (!OrganizationAccess(id).CanCreateEvents)
            {
                return NotFound();
            }

            eventModel.Organizer = organization;
            
            ModelState.Clear();
            
            // Barnabas: Skjera-7 -Checks if Endtime is starttime is greater than endtime.
            // For more details, see https://learn.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=net-7.0
            if (eventModel.StartTime != null && eventModel.EndTime != null && DateTime.Compare(eventModel.StartTime!.Value, eventModel.EndTime!.Value) >= 0)
            {
                ModelState.AddModelError(nameof(eventModel.EndTime), "Error: Select end time to be after start time");
            }
            
            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }
            

            if (!OrganizationAccess(id).CanCreateEvents)
            {
                return NotFound();
            }
            
            _context.Add(eventModel);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Organization), new { id=id });
        }

        // GET: Event/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (!EventAccess(id ?? 0).CanEditEvents)
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
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Info,StartTime,EndTime,LastTimeEdited")] EventModel eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }
            
            if (!EventAccess(eventModel.Id).CanEditEvents)
            {
                //return NotFound();
            }

            // Barnabas: Skjera-7 -Checks if Endtime is starttime is greater than endtime.
            // For more details, see https://learn.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=net-7.0
            
            if (eventModel.StartTime != null && eventModel.EndTime != null && DateTime.Compare(eventModel.StartTime!.Value, eventModel.EndTime!.Value) >= 0)
            {
                ModelState.AddModelError(nameof(eventModel.EndTime), "Error: Select end time to be after start time");
                return View(eventModel);
            }
            
            //Barnabas:  Clear
            ModelState.Clear();
            
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

            return RedirectToAction(nameof(Organization), new { id=eventModel.OrganizerId });
        }

        // GET: Event/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (!EventAccess(id ?? 0).CanDeleteEvents)
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
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.EventModels.FindAsync(id);

            if (!EventAccess(id).CanDeleteEvents)
            {
                return NotFound();
            }

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
        
        private UserOrganizationAccess OrganizationAccess(int organizationId)
        {
            return _userManager.GetUserAsync(User).Result.AccessToOrganizationAsync(organizationId).Result;
        }
        
        private UserOrganizationAccess EventAccess(int eventId)
        {
            var organizerId = _context.EventModels.FirstOrDefault(e => e.Id == eventId)?.OrganizerId;
            return _userManager.GetUserAsync(User).Result.AccessToOrganizationAsync(organizerId ?? 0).Result;
        }
    }
}
