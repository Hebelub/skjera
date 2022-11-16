using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace prosjekt.Models;

public class Comment
{
    public Comment()
    {
        
    }
        
    
    public Comment(EventModel eventModel, ApplicationUser user, string text)
    {
        EventModel = eventModel;
        Text = text;
        PostedBy = user;
    }
    

    public int Id { get; set; }
    
    
    // The comment that it is answering to
    //  public Comment? ParentComment { get; set; } = null;
    
    public DateTime PostTime = DateTime.Now;

    public DateTime? EditTime = null;


    [StringLength(1000)]
    [Display(Name = "Text")]
    [Required]
    public string Text { get; set; } = string.Empty;
    
    
    
    // Foreign key to the post.
    public int EventModelId { get; set; }

    // Navigation property to the event.
    public EventModel EventModel { get; set; }
    
    
    
    public ApplicationUser PostedBy { get; set; }
}