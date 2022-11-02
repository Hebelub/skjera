namespace prosjekt.Models;

public class UserOrganizationAccess
{
    public UserOrganizationAccess()
    {
    }
    
    public UserOrganizationAccess(ApplicationUser user, OrganizationModel organization)
    {
        User = user;
        Organization = organization;
    }

    public int Id { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int OrganizationId { get; set; }
    public OrganizationModel Organization { get; set; }

    public bool CanDeleteOrganization { get; set; } = false;
    public bool CanEditOrganization { get; set; } = false;

    public bool CanCreateEvents { get; set; } = false;
    public bool CanEditEvents { get; set; } = false;
    public bool CanDeleteEvents { get; set; } = false;
    
    public bool CanChangeUserRights { get; set; } = false;
    public bool CanAddUsers { get; set; } = false;

}