﻿using System.ComponentModel.DataAnnotations;

namespace prosjekt.Models;

public class UserOrganizationRelation
{
    public UserOrganizationRelation()
    {
        
    }
    
    public UserOrganizationRelation(ApplicationUser user, OrganizationModel organization, AccessRight accessRight)
    {
        User = user;
        Organization = organization;
        AccessRight = accessRight;
    }
    
    public UserOrganizationRelation(ApplicationUser user, OrganizationModel organization)
    {
        User = user;
        OrganizationId = organization.Id;
        Organization = organization;
    }

    public int Id { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int OrganizationId { get; set; }
    public OrganizationModel Organization { get; set; }


    public bool IsFollowing { get; set; }


    private AccessRight? _accessRight;
    public int AccessRightId { get; set; }
    public AccessRight AccessRight
    {
        get => _accessRight ?? AccessRight.NoAccess;
        set => _accessRight = value;
    }
}