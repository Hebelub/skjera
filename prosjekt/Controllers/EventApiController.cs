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
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        
        [HttpPut("{id:int}/attend")]
        public async void AttendEvent(int id, [FromBody]bool attend)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return;
            }
            
            var eventModel = await _context.EventModels.FindAsync(id);

            if (eventModel == null)
            {
                return;
            }

            var userEventRelation = await eventModel.GetUserEventRelationAsync(_context, user);
            userEventRelation.IsAttending = attend;

            if (_context.UserEventRelations.Any(e => e.EventId == id && e.User == user))
            {
                _context.UserEventRelations.Update(userEventRelation);
            }
            else
            {
                await _context.UserEventRelations.AddAsync(userEventRelation);
            }
        
            await _context.SaveChangesAsync();
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
    }
}
