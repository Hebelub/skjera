namespace prosjekt.Models;

public class AccessRight
{
    public int Id { get; set; }
    
    public static AccessRight NoAccess
    {
        get => new()
        {
            CanDeleteOrganization = false,
            CanEditOrganization = false,
            CanCreateEvents = false,
            CanEditEvents = false,
            CanDeleteEvents = false,
            CanManageUsers = false
        };
    }
    
    public static AccessRight FullAccess
    {
        get => new()
        {
            CanDeleteOrganization = true,
            CanEditOrganization = true,
            CanCreateEvents = true,
            CanEditEvents = true,
            CanDeleteEvents = true,
            CanManageUsers = true
        };
    }

    public static AccessRight MinimalAccess
    {
        get => new()
        {
            CanDeleteOrganization = false,
            CanEditOrganization = false,
            CanCreateEvents = true,
            CanEditEvents = true,
            CanDeleteEvents = false,
            CanManageUsers = false
        };
    }
    
    public bool HasAnyAccess
    {
        get => CanDeleteOrganization || CanEditOrganization || CanCreateEvents || CanEditEvents || CanDeleteEvents ||
               CanManageUsers;
    }

    public bool CanDeleteOrganization { get; set; }
    public bool CanEditOrganization { get; set; }

    public bool CanCreateEvents { get; set; }
    public bool CanEditEvents { get; set; }
    public bool CanDeleteEvents { get; set; }
    
    public bool CanManageUsers { get; set; }
}