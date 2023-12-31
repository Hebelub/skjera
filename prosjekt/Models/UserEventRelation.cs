﻿using Microsoft.AspNetCore.Mvc;
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
    
    
    public ApplicationUser User { get; set; }

    public int EventId { get; set; }
    public EventModel Event { get; set; }
    
    
    private UserOrganizationRelation UserOrganizationRelation
    {
        get => User.GetRelationToOrganizationAsync(Event.OrganizerId).Result;
    }
}