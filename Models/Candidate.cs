using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.Models;

public class Candidate
{
    public int CandidateId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(3000), Display(Name = "Biography")]
    public string Profile { get; set; } = "";

    [StringLength(180), Display(Name = "Campaign focus")]
    public string Tagline { get; set; } = "";

    public string? PhotoFileName { get; set; }

    [StringLength(500)]
    public string? Website { get; set; }

    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(500), Display(Name = "Social media URL")]
    public string? SocialUrl { get; set; }

    [StringLength(500), Display(Name = "Video URL")]
    public string? VideoUrl { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Display(Name = "Position")]
    public int PositionId { get; set; }

    public Position Position { get; set; } = null!;

    public string PhotoUrl => PhotoFileName == null
        ? "/portraits.svg#default"
        : PhotoFileName.StartsWith("seed-portrait-")
            ? $"/portraits.svg#{Path.GetFileNameWithoutExtension(PhotoFileName)}"
            : $"/uploads/{PhotoFileName}";
}
