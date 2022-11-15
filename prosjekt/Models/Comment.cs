using System.ComponentModel.DataAnnotations;

namespace prosjekt.Models;

public class Comment
{

   
        
    public Comment(EventModel eventModel, string text,ApplicationUser user)
    {

        EventModel = eventModel;
        Text = text;
        User = user;
    }

    public Comment()
    {
        // throw new NotImplementedException();
    }


    public int Id { get; set; }

    [Range(1, 5)]
    public int Stars { get; set; }

    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;
        
    // Foreign key to the Book model. This is configured automatically because of the name: <Model>Id
    public int EventModelId { get; set; }

    // Navigation property to the linked book
    public EventModel EventModel { get; set; }
        
    //Application property to the user who made this review
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
}