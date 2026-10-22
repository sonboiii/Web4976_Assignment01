using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.ViewModels;

public class DirectoryViewModel
{
    public List<Candidate> Candidates { get; set; } = [];

    public List<Municipality> Municipalities { get; set; } = [];

    public List<Position> Positions { get; set; } = [];

    public int? MunicipalityId { get; set; }

    public int? PositionId { get; set; }

    public string? Search { get; set; }

    public string Sort { get; set; } = "name";

    public int Page { get; set; } = 1;

    public int Total { get; set; }

    public int TotalPages => (int)Math.Ceiling(Total / 6d);

    public HashSet<int> SelectedIds { get; set; } = [];

    public HashSet<int> CompareIds { get; set; } = [];
}
