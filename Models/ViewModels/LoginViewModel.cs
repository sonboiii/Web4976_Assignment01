using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Models;

namespace Web4976_Assignment01.ViewModels;

public class LoginViewModel
{
    [Required, Display(Name = "Username")]
    public string Username { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}
