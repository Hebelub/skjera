using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using prosjekt.Data;

namespace prosjekt.Models;

public class EventModel
{
    public EventModel()
    {
    }

    public EventModel(OrganizationModel organizer, DateTime startTime, TimeSpan? duration, DateTime timeCreated, DateTime? lastTimeEdited, 
        string title, string info)
    {
        Organizer = organizer;
        TimeCreated = timeCreated;
        StartTime = startTime;
        Duration = duration;
        LastTimeEdited = lastTimeEdited;
        Title = title;
        Info = info;
    }

    
    
    public int Id { get; set; }
    
    
    
    [DisplayName ("TimeCreated")]
    public DateTime TimeCreated { get; } = DateTime.Now;


    
    [Required]
    [MaxLength(40)]
    [DisplayName ("Title")]
    public string Title { get; set; } = string.Empty;



    [MaxLength(2000)]
    [DisplayName ("Info")]
    public string Info { get; set; } = string.Empty;
    
    
    
    [DisplayName ("StartTime")]
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime {
        get => StartTime == null || Duration == null 
            ? null 
            : StartTime + Duration;
    }

    
    [DisplayName("Days")]
    public int Days { get; set; }
    [DisplayName("Hours")]
    public int Hours { get; set; }
    [DisplayName("Minutes")]
    public int Minutes { get; set; }

    [DisplayName("Duration")]
    public TimeSpan? Duration {
        get => TimeSpan.Parse($@"{Days}.{Hours}:{Minutes}");
        set
        {
            if (value == null)
            {
                return;
            }
            Minutes = value.Value.Minutes;
            Hours = value.Value.Hours;
            Days = value.Value.Days;
        }
    }
    
    

    [DisplayName("Location")]
    public string? Location { get; set; }
    

    public int OrganizerId { get; set; }
    public OrganizationModel Organizer { get; set; }
    
    
    [DisplayName ("LastTimeEdited")]
    public DateTime? LastTimeEdited { get; set; }

    

    public bool IsEdited { get => LastTimeEdited != null; }



    public async Task<UserEventRelation> GetUserEventRelationAsync(ApplicationDbContext db, ApplicationUser user)
    {
        return await db.UserEventRelations.FirstOrDefaultAsync(r => r.EventId == Id && r.User == user) 
               ?? new UserEventRelation(this, user);
    }

    public async Task<List<ApplicationUser>> GetAttendingUsersAsync(ApplicationDbContext db)
    {
        return await db.UserEventRelations
            .Where(r => r.EventId == Id && r.IsAttending)
            .Select(r => r.User)
            .ToListAsync();
    }
    
}