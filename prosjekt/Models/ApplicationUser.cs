using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;

namespace prosjekt.Models;

public class ApplicationUser : IdentityUser
{
    private readonly ApplicationDbContext _context;

    public ApplicationUser()
    {
        
    }
    
    public ApplicationUser(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserOrganizationAccess> AccessToOrganizationAsync(int organizationId)
    {
        var access = await _context.UserOrganizationAccess
            .FirstOrDefaultAsync(access => access.User == this && access.OrganizationId == organizationId);

        return access ?? new UserOrganizationAccess();
    } 
    
    public async Task<List<UserOrganizationAccess>> AllOrganizationAccessRightsAsync()
    {
        return await _context.UserOrganizationAccess.Where(access => access.User == this).ToListAsync();
    }
}