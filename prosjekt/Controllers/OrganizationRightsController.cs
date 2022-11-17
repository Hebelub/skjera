using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;
using prosjekt.Models;

namespace prosjekt.Controllers;

public class OrganizationRightsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationRightsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    
    // GET
    // Listing the users
    [Authorize]
    public async Task<IActionResult> Index(int id)
    {
        if (!CanUserManageOrganization(id))
        {
            return NotFound();
        }
        
        var organization = await _context.OrganizationModels.FindAsync(id);
        if (organization == null)
        {
            return NotFound();
        }
            
        return View(organization);
    }
    
    // GET: OrganizationRights/Manage/5
    public async Task<IActionResult> Manage(int id)
    {
        
        var userOrganizationRelation = await _context.UserOrganization
            .Include(uo => uo.Organization)
            .Include(uo => uo.User)
            .Include(uo => uo.AccessRight)
            .FirstOrDefaultAsync(uo => uo.Id == id);

        if (userOrganizationRelation == null || !CanUserManageOrganization(userOrganizationRelation.OrganizationId))
        {
            return NotFound();
        }

        return View(userOrganizationRelation);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Manage(int id, int organizationId, string userId, [Bind("CanDeleteOrganization", "CanEditOrganization", 
        "CanCreateEvents", "CanEditEvents", "CanDeleteEvents", "CanManageUsers")] 
        AccessRight accessRight)
    {
        var organization = await _context.OrganizationModels.FindAsync(organizationId);
        if (organization == null || !CanUserManageOrganization(organizationId))
        {
            return NotFound();
        }

        accessRight.Id = id;

        // Check if you try to change your own values
        if (await _userManager.GetUserAsync(User) == await _userManager.FindByIdAsync(userId))
        {
            // You are not allowed to turn off you own CanManageUsers AccessRight.
            // That could result in no users having that access to the organization. 
            accessRight.CanManageUsers = true;
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.AccessRights.Update(accessRight);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccessRightExist(accessRight.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        return RedirectToAction(nameof(Index), "OrganizationRights", new { id = organization.Id });
    }
    
    private bool AccessRightExist(int id)
    {
        return _context.AccessRights.Any(e => e.Id == id);
    }
    
    [Authorize]
    public async Task<IActionResult> Add(int id)
    {
        if (!CanUserManageOrganization(id))
        {
            return NotFound();
        }
        
        var organization = await _context.OrganizationModels.FindAsync(id);

        if (organization == null)
        {
            return NotFound();
        }
        
        return View(organization);
    }

    public bool CanUserManageOrganization(int organizationId)
    {
        return _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(organizationId, false).Result.AccessRight
            .CanManageUsers;
    }
}