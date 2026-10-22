using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.ViewModels;

public class CandidateForm
{
    public int CandidateId { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Display(Name = "Municipality")]
    public int MunicipalityId { get; set; }

    [Display(Name = "Position")]
    public int PositionId { get; set; }

    [Required, StringLength(3000), Display(Name = "Biography")]
    public string Profile { get; set; } = "";

    [Display(Name = "Campaign focus")]
    public string? Tagline { get; set; }

    [Url]
    public string? Website { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Url, Display(Name = "Social media URL")]
    public string? SocialUrl { get; set; }

    [Url, Display(Name = "Video URL")]
    public string? VideoUrl { get; set; }

    [Display(Name = "Candidate photograph")]
    public IFormFile? Image { get; set; }
}
