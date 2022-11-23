namespace prosjekt.Models;

public class EventFormModel
{
    public EventFormModel(EventModel ev, FormType formType)
    { 
        Event = ev;
        FormType = formType;
    }
    
    public EventModel Event { get; set; }

    public FormType FormType { get; set; }
}