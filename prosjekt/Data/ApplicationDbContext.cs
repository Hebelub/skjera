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
    
}