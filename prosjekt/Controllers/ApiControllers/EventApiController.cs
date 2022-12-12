using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
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
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventModels(DateTime fromDate, DateTime toDate)
        {
            var events = await _context.EventModels
                .Where(e => e.StartTime >= fromDate 
                            && e.StartTime <= toDate)
                .Include(e => e.Organizer)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            // This way of doing it is probably not the most efficient
            var eventDTOs = new List<EventDTO>();
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                foreach (var ev in events)
                {
                    eventDTOs.Add(new EventDTO(ev, false, false, ev.Organizer.Name));
                }   
            }
            else
            {
                foreach (var ev in events)
                {
                    var userEventRelation = await ev.GetUserEventRelationAsync(_context, user);
                    var userOrganizationRelation = await user.GetRelationToOrganizationAsync(ev.OrganizerId);
                    eventDTOs.Add(new EventDTO(
                            ev, 
                            userEventRelation.IsAttending, 
                            userOrganizationRelation.IsFollowing,
                            ev.Organizer.Name
                        )
                    );
                }   
            }

            return eventDTOs;
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
