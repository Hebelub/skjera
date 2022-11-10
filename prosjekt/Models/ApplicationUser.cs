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

    public async Task<UserOrganization> GetRelationToOrganizationAsync(int organizationId)
    {
        var organizationRelation = await _context.UserOrganization
            .Include(o => o.AccessRight)
            .FirstOrDefaultAsync(access => access.User == this && access.OrganizationId == organizationId);

        if (organizationRelation != null && organizationRelation.Id != 0)
        {
            return organizationRelation;
        }

        var organization = await _context.OrganizationModels.FindAsync(organizationId);

        if (organization == null)
        {   
            throw new Exception("Organization not found");
        }

        var a = new UserOrganization(this, organization, AccessRight.NoAccess);
        return a;
    } 
    
    public async Task<List<UserOrganization>> GetOrganizationRelationsAsync()
    {
        return await _context.UserOrganization
            .Where(relation => relation.User == this)
            .Include(relation => relation.Organization)
            .Include(relation => relation.AccessRight)
            .ToListAsync();
    }
}