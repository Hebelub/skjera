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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace prosjekt.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private string UploadThumbnail(IFormFile file)
        {
            string uniqueFileName = string.Empty;
            
            if (file != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/uploads");
                uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
            }

            return uniqueFileName;
        }

        // GET: Event/
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventModels
                .Where(e => e.StartTime >= DateTime.Now)
                .OrderBy(e => e.StartTime)
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

            var eventModel = await _context.EventModels.FindAsync(id ?? 0);

            var organizerModel = await _context.OrganizationModels.FindAsync(eventModel.OrganizerId);
            if (eventModel == null || organizerModel == null)
            {
                return NotFound();
            }

            ViewBag.EventIncludeAllData = true;

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
            ViewBag.FormType = FormType.Create;
            return View("_EventForm", model);
        }

        // POST: Event/Create/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([FromForm][Bind("Title,Info,Date,StartTime,Duration,Days,Hours,Minutes,Location,Thumbnail")] EventModel eventModel, int id)
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

            eventModel.ThumbnailUrl = UploadThumbnail(eventModel.Thumbnail);
            
            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                ViewBag.FormType = FormType.Create;
                return View("_EventForm", eventModel);
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
                return Forbid();
            }

            var eventModel = await _context.EventModels.FindAsync(id);

            if (eventModel == null)
            {
                return NotFound();
            }
            
            ViewBag.FormType = FormType.Edit;
            return View("_EventForm", eventModel);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, int organizerId, [Bind("Title,Description,Date,Info,StartTime,Duration,Days,Hours,Minutes,Location,Thumbnail,ThumbnailUrl")] EventModel eventModel)
        {
            eventModel.LastTimeEdited = DateTime.Now;

            eventModel.ThumbnailUrl = UploadThumbnail(eventModel.Thumbnail);

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
                ViewBag.FormType = FormType.Edit;
                return View("_EventForm", eventModel);
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
                return Forbid();
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
                return Forbid();
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
