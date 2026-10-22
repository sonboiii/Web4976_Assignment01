using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.ViewModels;

public class MunicipalityForm
{
    public int MunicipalityId { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Province { get; set; } = "British Columbia";

    [Required, DataType(DataType.Date), Display(Name = "Election date")]
    public DateTime? ElectionDate { get; set; } = new(2026, 10, 17);

    [Required]
    public string Description { get; set; } = "";

    [Display(Name = "Municipality logo")]
    public IFormFile? Image { get; set; }
}
