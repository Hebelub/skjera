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


    // GET: OrganizationRights
    [Authorize]
    public async Task<IActionResult> Add(int id, string searchString = "")
    {
        if (!CanUserManageOrganization(id))
        {
            return NotFound();
        }

        // Does the Organization exist?
        if (await _context.OrganizationModels.FindAsync(id) == null)
        {
            return NotFound();
        }

        ViewData["OrganizationId"] = id;

        // Search for Username and Email
        return View(await _userManager.Users.Where(
            u => u.UserName.ToLower().Contains(searchString.ToLower())
                 || u.Email.ToLower().Contains(searchString.ToLower())
        ).ToListAsync());
    }
    

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUser(int id, string userId) // id: organizationId
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

        var userToAdd = await _userManager.FindByIdAsync(userId);
        if (userToAdd == null)
        {
            return NotFound();
        }
        
        var relation = await userToAdd.GetRelationToOrganizationAsync(id, true, true);

        // We have to create a new relation
        if (relation == null)
        {
            relation = new UserOrganization();
            relation.Organization = organization;
            relation.User = userToAdd;
            relation.AccessRight = AccessRight.MinimalAccess;

            _context.AccessRights.Add(relation.AccessRight);
            _context.UserOrganization.Add(relation);
            await _context.SaveChangesAsync();
        }
        // There is already a relation
        else if (!relation.AccessRight.HasAnyAccess)
        {
            relation.AccessRight.CanCreateEvents = true;
            relation.AccessRight.CanEditEvents = true;
            
            _context.Update(relation);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Manage), "OrganizationRights", new { relation.Id });
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
    public async Task<IActionResult> Manage(int id) // Id: UserOrganization
    {
        var userOrganizationRelation = await _context.UserOrganization
            .Include(uo => uo.Organization)
            .Include(uo => uo.User)
            .Include(uo => uo.AccessRight)
            .FirstOrDefaultAsync(uo => uo.Id == id);
        
        if (userOrganizationRelation == null 
            || !CanUserManageOrganization(userOrganizationRelation.OrganizationId)
            || !userOrganizationRelation.AccessRight.HasAnyAccess)
        {
            return NotFound();
        }

        return View(userOrganizationRelation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Manage(int id, int organizationId, string userId, [Bind("CanDeleteOrganization",
            "CanEditOrganization",
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

    public bool CanUserManageOrganization(int organizationId)
    {
        return _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(organizationId, false)
            .Result.AccessRight.CanManageUsers;
    }
}