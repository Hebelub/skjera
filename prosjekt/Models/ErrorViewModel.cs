namespace prosjekt.Models;

public class ErrorViewModel
{
    public int Id;
    
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}