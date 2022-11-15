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
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public IActionResult Index()
        {

        
            var comments = _context.Comments.ToList();
            return View(comments);
        }
        
        //get
        [Authorize]
        [HttpGet]


        // GET: comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: comment/Create/5
        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var events = await _context.EventModels.FirstOrDefaultAsync(o => o.Id == id);

            if (events == null)
            {
                return NotFound();
            }

          

            // Creating the model with the correct organization as the parent
            var model = new Comment();

            model.EventModel = events;
            
            ViewData["OrganizationId"] = id;
            return View(model);
        }

        // POST: comment/Create/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Comment comment, int id)
        {
            var ev = await _context.EventModels.FindAsync(id);

            if (ev == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            
            comment.EventModel = ev;
            comment.User = user;

            /*if (!OrganizationAccess(id).CanCreateEvents)
            {
                return NotFound();
            }

            ModelState.Clear();
          
       
        

            if (!OrganizationAccess(id).CanCreateEvents)
            {
                return NotFound();
            }  */
            
            _context.Add(comment);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Details), "Event", new { id });
        }
        
        
        
        
   

        
        
        
        
        
        
        
        
        

        // GET: comment/Edit/5
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
            
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }
            
            return View(comment);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, int eventId, Comment comment)
        {
            var events = await _context.EventModels.FindAsync(eventId);
            if (events == null)
            {
                return NotFound();
            }

            comment.Id = id;
            comment.EventModel = events;
            

            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                return View(comment);
            }

            try
            {
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentSectionExist(
                        comment.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Details), "Organization", new { id= eventId });
        }
    
        // GET: comment/Delete/5
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

            var comments = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // POST: comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (!EventAccess(id).CanDeleteEvents)
            {
                return NotFound();
            }

            if (comment != null)
            {
                var eventId = comment.UserId;
                _context.Comments.Remove(comment);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Event", new { id=eventId });
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Event", new { comment?.UserId });
        }

        private bool CommentSectionExist(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
        
        private AccessRight OrganizationAccess(int organizationId)
        {
            return _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(organizationId).Result.AccessRight;
        }
        
        private AccessRight EventAccess(int userId)
        {
            var eventId = _context.Comments.FirstOrDefault(e => e.Id == userId)?.UserId;
            return _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(eventId ?? 0).Result.AccessRight;
        }
    }
}