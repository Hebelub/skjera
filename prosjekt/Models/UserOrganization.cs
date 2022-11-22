using System.ComponentModel.DataAnnotations;

namespace prosjekt.Models;

public class UserOrganization
{
    public UserOrganization()
    {
    }
    
    public UserOrganization(ApplicationUser user, OrganizationModel organization, AccessRight accessRight)
    {
        User = user;
        Organization = organization;
        AccessRight = accessRight;
    }
    
    public UserOrganization(ApplicationUser user, OrganizationModel organization)
    {
        User = user;
        Organization = organization;
    }

    public int Id { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int OrganizationId { get; set; }
    public OrganizationModel Organization { get; set; }


    public bool IsFollowing { get; set; }

    
    public bool IsInterested { get; set; }

    
    private AccessRight? _accessRight;
    public int AccessRightId { get; set; }
    public AccessRight AccessRight
    {
        get => _accessRight ?? AccessRight.NoAccess;
        set => _accessRight = value;
    }
}