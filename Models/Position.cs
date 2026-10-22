using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.Models;

public class Position
{
    public int PositionId { get; set; }

    [StringLength(80)]
    public string Name { get; set; } = "";

    [Display(Name = "Seats available")]
    public int SeatsAvailable { get; set; } = 1;

    [Display(Name = "Municipality")]
    public int MunicipalityId { get; set; }

    public Municipality Municipality { get; set; } = null!;

    public ICollection<Candidate> Candidates { get; set; } = [];
}
