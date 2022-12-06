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
        
        public IActionResult Details() {
            return NotFound();
        }

        // POST: comment/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Text")] Comment comment, int id)
        {
            var ev = await _context.EventModels.FindAsync(id);

            if (ev == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            comment.EventModel = ev;
            comment.PostedBy = user;

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

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

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }
            
            return View(comment);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, int eventId, [Bind("Text", "PostTime")] Comment comment)
        {
            var ev = await _context.EventModels.FindAsync(eventId);

            if (ev == null)
            {
                return NotFound();
            }

            comment.Id = id;

            comment.EventModel = ev;
            comment.EventModelId = eventId;
            comment.PostedBy = await _userManager.GetUserAsync(User);

            ModelState.Clear();

            if (ModelState.IsValid)
            {
                comment.EditTime = DateTime.Now;

                try
                {
                    _context.Comments.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentSectionExist(comment.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return RedirectToAction(nameof(Details), "Event", new { id = eventId });
        }


        // Get: Comment/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            // If the poster of the comment was the one posting it, they can delete it
            if (await _userManager.GetUserAsync(User) != comment.PostedBy)
            {
                return NotFound();
            }

            // Any case, redirect back to the event
            return View(comment);
        }


        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            // If the poster of the comment was the one posting it, they can delete it
            if (await _userManager.GetUserAsync(User) == comment.PostedBy)
            {
                _context.Comments.Remove(comment);

                await _context.SaveChangesAsync();
            }

            // Any case, redirect back to the event
            return RedirectToAction(nameof(Details), "Event", new { id = comment.EventModelId });
        }

        private bool CommentSectionExist(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}