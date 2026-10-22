using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.Models;

public class Municipality
{
    public int MunicipalityId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(60)]
    public string Province { get; set; } = "British Columbia";

    [DataType(DataType.Date), Display(Name = "Election date")]
    public DateTime ElectionDate { get; set; } = new(2026, 10, 17);

    [StringLength(1500)]
    public string Description { get; set; } = "";

    public string? LogoFileName { get; set; }

    public ICollection<Position> Positions { get; set; } = [];
}
