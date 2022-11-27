using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using prosjekt.Data;
using prosjekt.Models;

namespace prosjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: api/EventApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventModels()
        {
            return await _context.EventModels.ToListAsync();
        }
        
        [HttpGet("date/{fromDate:DateTime}/{toDate:DateTime}")]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventModels(DateTime fromDate, DateTime toDate)
        {
            return await _context.EventModels
                .Where(e => e.StartTime >= fromDate 
                            && e.StartTime <= toDate)
                // .OrderByDescending( time )
                .ToListAsync();
        }

        [HttpPut("attend/{id:int}")]
        public async void AttendEvent(int id, bool attend)
        {
            var user = await _userManager.GetUserAsync(User);
            
            var eventModel = await _context.EventModels.FindAsync(id);

            if (eventModel == null)
            {
                return;
            }

            var relation = await eventModel.GetUserEventRelationAsync(_context, user);
            relation.IsAttending = attend;
        }

        // GET: api/EventApi/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventModel>> GetEventModel(int id)
        {
            var eventModel = await _context.EventModels.FindAsync(id);

            if (eventModel == null)
            {
                return NotFound();
            }
            
            return eventModel;
        }

        // PUT: api/EventApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutEventModel(int id, EventModel eventModel)
        {
            if (id != eventModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventModelExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/EventApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventModel>> PostEventModel(EventModel eventModel)
        {
            _context.EventModels.Add(eventModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEventModel", new { id = eventModel.Id }, eventModel);
        }

        // DELETE: api/EventApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEventModel(int id)
        {
            var eventModel = await _context.EventModels.FindAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            _context.EventModels.Remove(eventModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventModelExists(int id)
        {
            return _context.EventModels.Any(e => e.Id == id);
        }
    }
}
