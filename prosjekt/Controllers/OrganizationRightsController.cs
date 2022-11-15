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
    public async Task<IActionResult> Index(int id)
    {
        var organization = await _context.OrganizationModels.FindAsync(id);
        if (organization == null)
        {
            return NotFound();
        }
            
        return View(organization);
    }
    
    public async Task<IActionResult> Manage(int id)
    {
        var userOrganizationRelation = await _context.UserOrganization
            .Include(uo => uo.Organization)
            .Include(uo => uo.User)
            .FirstOrDefaultAsync(uo => uo.Id == id);

        if (userOrganizationRelation == null)
        {
            return NotFound();
        }

        return View(userOrganizationRelation);
    }
    
    public async Task<IActionResult> Add(int id)
    {
        var organization = await _context.OrganizationModels.FindAsync(id);

        if (organization == null)
        {
            return NotFound();
        }
        
        return View(organization);
    }
}