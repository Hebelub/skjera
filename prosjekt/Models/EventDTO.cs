namespace prosjekt.Models;

public class EventDTO
{
    public int Id { get; set; }
    public DateTime TimeCreated { get; } = DateTime.Now;
    public string? Title { get; set; } = string.Empty;
    public string? Info { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Location { get; set; }
    public int OrganizerId { get; set; }
    public OrganizationModel Organizer { get; set; }
    public DateTime? LastTimeEdited { get; set; }
    public bool IsEdited { get => LastTimeEdited != null; }

}