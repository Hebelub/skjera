namespace prosjekt.Models;

public class EventDTO
{
    public EventDTO(EventModel ev, bool isUserAttending, bool isUserFollowingOrganizer, string organizerName)
    {
        Id = ev.Id;
        TimeCreated = ev.TimeCreated;
        Title = ev.Title;
        Info = ev.Info;
        StartTime = ev.StartTime;
        Duration = ev.Duration;
        Location = ev.Location;
        LastTimeEdited = ev.LastTimeEdited;
        IsUserAttending = isUserAttending;
        OrganizerId = ev.OrganizerId;
        OrganizerName = organizerName;
        IsUserFollowingOrganizer = isUserFollowingOrganizer;
    }
    
    public int Id { get; set; }
    
    public DateTime TimeCreated { get; set; } = DateTime.Now;
    
    public string? Title { get; set; } = string.Empty;
    
    public string? Info { get; set; } = string.Empty;
    
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime {
        get => StartTime == null
            ? null 
            : StartTime + Duration;
    }
    
    public TimeSpan Duration { get; set; }
    
    public string? Location { get; set; }

    public DateTime? LastTimeEdited { get; set; }

    public bool IsUserAttending { get; set; }
    
    
    // Data of Organizer
    public int OrganizerId { get; set; }

    public string OrganizerName { get; set; }

    public bool IsUserFollowingOrganizer { get; set; }
}