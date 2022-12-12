using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using prosjekt.Data;
using prosjekt.Models;

namespace prosjekt.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrganizationApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    
    [HttpPut("{id:int}/follow")]
    public async void FollowOrganization(int id, [FromBody]bool follow)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return;
        }

        var organization = await _context.OrganizationModels.FindAsync(id);

        if (organization == null)
        {
            return;
        }

        var userOrganizationRelation = await user.GetRelationToOrganizationAsync(id);
        userOrganizationRelation.IsFollowing = follow;

        if (_context.UserOrganization.Any(e => e.OrganizationId == id && e.User == user))
        {
            _context.UserOrganization.Update(userOrganizationRelation);
        }
        else
        {
            await _context.UserOrganization.AddAsync(userOrganizationRelation);
        }
        
        await _context.SaveChangesAsync();
    }
}