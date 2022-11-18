using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using prosjekt.Models;

namespace prosjekt.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<OrganizationModel> OrganizationModels { get; set; }
    
    public DbSet<EventModel> EventModels { get; set; }
    
    public DbSet<UserOrganization> UserOrganization { get; set; }
    
    public DbSet<AccessRight> AccessRights { get; set; }
    
    
    public DbSet<Comment> Comments { get; set; }
    
    
    public DbSet<UserEventRelation> UserEventRelations { get; set; }
}