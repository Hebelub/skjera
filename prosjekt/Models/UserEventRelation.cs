using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;

namespace prosjekt.Models;

public class UserEventRelation
{
    public UserEventRelation() {}

    public UserEventRelation(EventModel ev, ApplicationUser user)
    {
        Event = ev;
        User = user;
    }
    
    public int Id { get; set; }

    public bool IsAttending { get; set; } = false;

    private bool? _isInterested = null;
    public bool IsInterested
    {
        get => _isInterested ?? UserOrganizationRelation.IsInterested;
        set
        {
            _isInterested = value;
            IsAttending = false;
        }
    }

    
    public ApplicationUser User { get; set; }

    public int EventId { get; set; }
    public EventModel Event { get; set; }
    
    
    private UserOrganization UserOrganizationRelation
    {
        get => User.GetRelationToOrganizationAsync(Event.OrganizerId).Result;
    }
}