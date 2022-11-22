using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace prosjekt.Models;

public class EventModel
{
    public EventModel()
    {
    }

    public EventModel(OrganizationModel organizer, DateTime timeCreated, DateTime? lastTimeEdited, 
        string title, string description, string info)
    {
        Organizer = organizer;
        TimeCreated = timeCreated;
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
    
    
    //[Required(ErrorMessage = "Start time should not be greater than End time.")]
    [DisplayName ("StartTime")]
    public DateTime? StartTime { get; set; }
    
    [DisplayName("EndTime")]
    public DateTime? EndTime { get; set; }

    [DisplayName("Location")]
    public string? Location { get; set; }
    

    public int OrganizerId { get; set; }
    public OrganizationModel Organizer { get; set; }
    
    
    [DisplayName ("LastTimeEdited")]
    public DateTime? LastTimeEdited { get; set; }

    

    public bool IsEdited { get => LastTimeEdited != null; }
    
    
    
    public List<ApplicationUser> Attending = new(); 
    
    
    
    public int NumAttending
    {
        get => Attending.Count;
    }

}