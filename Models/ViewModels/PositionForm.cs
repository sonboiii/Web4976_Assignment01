using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.ViewModels;

public class PositionForm
{
    public int PositionId { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Range(1, 100), Display(Name = "Seats available")]
    public int SeatsAvailable { get; set; } = 1;

    [Display(Name = "Municipality")]
    public int MunicipalityId { get; set; }
}
