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


    public int Id { get; set; }
    
    
    
    [DisplayName ("TimeCreated")]
    public DateTime TimeCreated { get; set; } = DateTime.Now;


    
    [Required]
    [MaxLength(40)]
    [DisplayName ("Title")]
    public string? Title { get; set; } = string.Empty;


    [Required]
    [MaxLength(2000)]
    [DisplayName ("Info")]
    public string? Info { get; set; } = string.Empty;
    
    
    [Required]
    [DisplayName ("StartTime")]
    public DateTime StartTime { get; set; }

    public DateTime EndTime {
        get => StartTime + Duration;
    }

    
    [DisplayName("Days")]
    [Range(0, 364, ErrorMessage = "Min {0}, Max {1}")]
    public int Days { get; set; }
    
    
    [DisplayName("Hours")]
    [Range(0, 23, ErrorMessage = "Min {0}, Max {1}")]
    public int Hours { get; set; }
    
    
    [DisplayName("Minutes")]
    [Range(0, 59, ErrorMessage = "Min {0}, Max {1}")]
    public int Minutes { get; set; }

    
    [DisplayName("Duration")]
    public TimeSpan Duration {
        get => TimeSpan.Parse($@"{Days}.{Hours}:{Minutes}");
        set
        {
            Minutes = value.Minutes;
            Hours = value.Hours;
            Days = value.Days;
        }
    }
    
    

    [DisplayName("Location")]
    [MaxLength(120)]
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