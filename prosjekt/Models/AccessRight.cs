namespace prosjekt.Models;

public class AccessRight
{
    public int Id { get; set; }
    
    public static AccessRight NoAccess
    {
        get
        {
            return new AccessRight()
            {
                CanDeleteOrganization = false,
                CanEditOrganization = false,
                CanCreateEvents = false,
                CanEditEvents = false,
                CanDeleteEvents = false,
                CanChangeUserRights = false,
                CanAddUsers = false
            };
        }
    }
    
    public static AccessRight FullAccess
    {
        get
        {
            return new AccessRight()
            {
                CanDeleteOrganization = true,
                CanEditOrganization = true,
                CanCreateEvents = true,
                CanEditEvents = true,
                CanDeleteEvents = true,
                CanChangeUserRights = true,
                CanAddUsers = true
            };
        }
    }
    

    public bool CanDeleteOrganization { get; set; }
    public bool CanEditOrganization { get; set; }

    public bool CanCreateEvents { get; set; }
    public bool CanEditEvents { get; set; }
    public bool CanDeleteEvents { get; set; }
    
    public bool CanChangeUserRights { get; set; }
    public bool CanAddUsers { get; set; }
}