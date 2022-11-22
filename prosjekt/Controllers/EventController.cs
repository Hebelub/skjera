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
            
            return View(await _context.EventModels
                .Include(e => e.Organizer)
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
                return Forbid();
            }

            // Creating the model with the correct organization as the parent
            var model = new EventModel();

            model.Organizer = organization;
            
            ViewData["OrganizationId"] = id;
            return View(model);
        }

        // POST: Event/Create/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Info,Date,StartTime,Duration,Days,Hours,Minutes")] EventModel eventModel, int id)
        {
            var organization = await _context.OrganizationModels.FindAsync(id);

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
            
            return RedirectToAction(nameof(Details), "Organization", new { id });
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
        public async Task<IActionResult> Edit(int id, int organizerId, [Bind("Title,Description,Date,Info,StartTime,Duration,Days,Hours,Minutes")] EventModel eventModel)
        {
            eventModel.LastTimeEdited = DateTime.Now;
            
            var organizer = await _context.OrganizationModels.FindAsync(organizerId);
            if (organizer == null)
            {
                return NotFound();
            }

            eventModel.Id = id;
            eventModel.Organizer = organizer;

            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }

            try
            {
                _context.EventModels.Update(eventModel);
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

            return RedirectToAction(nameof(Details), "Event", new { id });
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
                return RedirectToAction(nameof(Details), "Organization", new { id=organizationId });
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Organization", new { eventModel?.OrganizerId });
        }
        
        [Authorize]
        public async Task<IActionResult> Attend(int id, bool attending) // id: eventId
        {
            var user = await _userManager.GetUserAsync(User);
            var ev = await _context.EventModels.FindAsync(id);

            if (ev == null)
            {
                return NotFound();
            }

            var userEventRelation = await ev.GetUserEventRelationAsync(_context, user);

            userEventRelation.IsAttending = attending;

            if (userEventRelation.Id == 0)
            {
                await _context.AddAsync(userEventRelation);
            }
            else
            {
                _context.Update(userEventRelation);
            }

            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Details), "Event", new { id });
        }
        
        private bool EventModelExists(int id)
        {
            return _context.EventModels.Any(e => e.Id == id);
        }
        
        private AccessRight OrganizationAccess(int organizationId)
        {
            if (User.IsInRole("Admin"))
            {
                return AccessRight.FullAccess;
            }
            return _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(organizationId).Result.AccessRight;
        }
        
        private AccessRight EventAccess(int eventId)
        {
            return OrganizationAccess(_context.EventModels.FirstOrDefault(e => e.Id == eventId)?.OrganizerId ?? 0);
        }
    }
}
