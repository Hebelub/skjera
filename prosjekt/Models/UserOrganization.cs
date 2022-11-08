namespace prosjekt.Models;

public class UserOrganization
{
    public UserOrganization()
    {
    }
    
    public UserOrganization(ApplicationUser user, OrganizationModel organization, AccessRight? accessRight)
    {
        User = user;
        Organization = organization;
        AccessRight = accessRight;
    }

    public int Id { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int OrganizationId { get; set; }
    public OrganizationModel Organization { get; set; }
    
    
    
    
    private AccessRight? _accessRight;
    public int AccessRightId { get; set; }
    public AccessRight AccessRight
    {
        get
        {
            Console.WriteLine("Getting AccessRight: " + _accessRight);
            return _accessRight ?? AccessRight.NoAccess;
        }
        set
        {
            Console.WriteLine("Setting AccessRight value: " + value);
            _accessRight = value;
        }
    }

}