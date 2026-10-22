using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.Models;

public static class Roles
{
    public const string SuperAdmin = "Super Admin";

    public static string ForMunicipality(int id) => $"Municipality:{id}:Admin";
}
