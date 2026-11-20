using System.ComponentModel.DataAnnotations;

namespace MEMS.Shared;

public static class Roles
{
    public const string Super = "Super Admin";
    public const string Municipal = "Municipality Administrator";
    public const string TenantClaim = "municipality_id";
}

public class MunicipalityForm : IValidatableObject
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(60), RegularExpression(@"[a-z0-9]+(-[a-z0-9]+)*", ErrorMessage = "Use lowercase letters, numbers and single hyphens for the slug.")]
    public string Slug { get; set; } = "";
    [Required, StringLength(60)] public string Province { get; set; } = "British Columbia";
    [Required, StringLength(1500)] public string Description { get; set; } = "";
    public DateOnly ElectionDate { get; set; } = new(2026, 10, 17);
    [Required, StringLength(7), RegularExpression("#[0-9A-Fa-f]{6}", ErrorMessage = "Choose a six-digit hex colour.")] public string Accent { get; set; } = "#236559";

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Slug is "admin" or "admin-assets" or "guide" or "api" or "health" or "assets" or "alive")
            yield return new("That URL is reserved.", [nameof(Slug)]);
        if (ElectionDate == default)
            yield return new("Choose an election date.", [nameof(ElectionDate)]);
    }
}

public class Municipality : MunicipalityForm
{
    public int Id { get; set; }
}

public class PositionForm
{
    [Required, StringLength(80)] public string Name { get; set; } = "";
    [Range(1, 100)] public int SeatsAvailable { get; set; } = 1;
}

public class Position : PositionForm
{
    public int Id { get; set; }
    [ConcurrencyCheck] public int MunicipalityId { get; set; }
}

public class CandidateForm
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(3000)] public string Profile { get; set; } = "";
    [StringLength(180)] public string? Tagline { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Choose a position.")] public int PositionId { get; set; }
    [HttpUrl, StringLength(500)] public string? Website { get; set; }
    [EmailAddress, StringLength(254)] public string? Email { get; set; }
    [HttpUrl, StringLength(500)] public string? SocialUrl { get; set; }
    [HttpUrl, StringLength(500)] public string? VideoUrl { get; set; }
}

public class Candidate : CandidateForm
{
    public int Id { get; set; }
    [ConcurrencyCheck] public int MunicipalityId { get; set; }
    public DateTime DateCreated { get; set; }
    public int? PortraitNumber { get; set; }
}

public sealed class HttpUrlAttribute : ValidationAttribute
{
    public HttpUrlAttribute() : base("Enter a full HTTP or HTTPS website address.") { }
    public override bool IsValid(object? value) => value is null or "" ||
        (value is string s && Uri.TryCreate(s, UriKind.Absolute, out var url) &&
         url.Scheme is "http" or "https" && url.Host.Length > 0);
}

public class AdministratorForm
{
    [Required, StringLength(100)] public string Username { get; set; } = "";
    [Required, StringLength(128, MinimumLength = 6)] public string Password { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "Choose a municipality.")] public int MunicipalityId { get; set; }
}

public record Administrator(string Id, string Username, int? MunicipalityId);
public record Totals(int Municipalities, int Positions, int Candidates);
public record Credentials([Required] string Username, [Required] string Password);
public record Account(string Username, string Role, int? MunicipalityId);

public class TransferForm
{
    [Range(1, int.MaxValue, ErrorMessage = "Choose a municipality.")] public int MunicipalityId { get; set; }
    public int? PositionId { get; set; }
}
