namespace prosjekt.Models;

public class EventModel
{
    public EventModel()
    {
    }


    /// <summary>
    /// The DateTime when this event was posted
    /// </summary>
    private DateTime TimeCreated { get; set; } = DateTime.Now;

    /// <summary>
    /// This is null if the event has never been altered
    /// It is DateTime of the time it was last edited 
    /// </summary>
    private DateTime? LastTimeEdited { get; set; }

    private bool IsEdited { get => LastTimeEdited != null; }

    /// <summary>
    /// The organization who organizes this Event
    /// </summary>
    private OrganizationModel organizer { get; set; }
}