using System.Security.Claims;
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

    public string? NickName { get; set; }

    public async Task<UserOrganizationRelation> GetRelationToOrganizationAsync(int organizationId, bool withTracking=true, bool returnNullIfNotFound=false)
    {
        var userOrganizationWithOrWithoutTracking = withTracking 
            ? _context.UserOrganization.AsTracking() 
            : _context.UserOrganization.AsNoTracking();
        
        var organizationRelation = await userOrganizationWithOrWithoutTracking
            .Include(o => o.AccessRight)
            .Include(o => o.User)
            .FirstOrDefaultAsync(access => access.User == this && access.OrganizationId == organizationId);

        if (organizationRelation != null && organizationRelation.Id != 0)
        {
            return organizationRelation;
        }

        if (returnNullIfNotFound)
        {
            return null;
        }

        var organization = await _context.OrganizationModels.FindAsync(organizationId);

        if (organization == null)
        {   
            throw new Exception("Organization not found");
        }

        var userOrganization = new UserOrganizationRelation(this, organization, AccessRight.NoAccess);
        userOrganization.UserId = Id;
        userOrganization.OrganizationId = organizationId;
        return userOrganization;
    } 
    
    public async Task<List<UserOrganizationRelation>> GetOrganizationRelationsAsync()
    {
        return await _context.UserOrganization
            .Where(relation => relation.User == this)
            .Include(relation => relation.Organization)
            .Include(relation => relation.AccessRight)
            .ToListAsync();
    }
}