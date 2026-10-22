using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public static class Selection
{
    public static HashSet<int> Read(ISession session, string key) => System.Text.Json.JsonSerializer.Deserialize<HashSet<int>>(session.GetString(key) ?? "[]") ?? [];

    public static void Write(ISession session, string key, HashSet<int> ids) => session.SetString(key, System.Text.Json.JsonSerializer.Serialize(ids));
}
