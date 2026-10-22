using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.Models;

public class ApplicationUser : IdentityUser
{
    public int? MunicipalityId { get; set; }
}
