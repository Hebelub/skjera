using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace prosjekt.Models;

public class OrganizationModel
{
    public OrganizationModel()
    {
    }

    public OrganizationModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public int Id { get; set; }
    
    [Required]
    [MaxLength(40)]
    [DisplayName ("Name")]
    public string Name { get; set; } = string.Empty;


    // TODO: public Image Logo { get; set; } = { ... }

    /// <summary>
    /// What should the users know about this Organization?
    /// </summary>
    [Required]
    [MaxLength(1400)]
    [DisplayName ("Description")]
    public string Description { get; set; } = string.Empty;
}