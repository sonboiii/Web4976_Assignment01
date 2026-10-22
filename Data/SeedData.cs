using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Models;
using Microsoft.AspNetCore.Identity;

namespace Web4976_Assignment01.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, IWebHostEnvironment env, IConfiguration config)
    {
        if (!env.IsDevelopment() || !config.GetValue<bool>("SeedDemo"))
            return;
        var db = services.GetRequiredService<ApplicationDbContext>();

        if (!await db.Municipalities.AnyAsync())
        {
            var municipalities = new[]
            {
                ("Pitt Meadows", 6, 2),
                ("Maple Ridge", 6, 2),
                ("Burnaby", 8, 7),
                ("Surrey", 8, 6)
            };
            foreach (var (name, council, trustee) in municipalities)
            {
                var m = new Municipality
                {
                    Name = name,
                    Province = "British Columbia",
                    ElectionDate = new(2026, 10, 17),
                    Description = $"Municipal election for {name}."
                };
                m.Positions.Add(new Position { Name = "Mayor", SeatsAvailable = 1 });
                m.Positions.Add(new Position { Name = "Councilor", SeatsAvailable = council });
                m.Positions.Add(new Position { Name = "School Trustee", SeatsAvailable = trustee });
                db.Municipalities.Add(m);
            }

            await db.SaveChangesAsync();
            var pm = await db.Municipalities.Include(m => m.Positions).SingleAsync(m => m.Name == "Pitt Meadows");
            var people = new[]
            {
                ("Amelia Chen", "Mayor", "A connected community. A brighter tomorrow."),
                ("Daniel Brooks", "Mayor", "Practical leadership. Lasting progress."),
                ("Aisha Patel", "Councilor", "Room to grow. A place to belong."),
                ("Benjamin Lee", "Councilor", "Moving our community forward, together."),
                ("Clara Wilson", "Councilor", "Protecting the places we call home."),
                ("David Okafor", "Councilor", "Local businesses. Stronger neighbourhoods."),
                ("Elena Martinez", "Councilor", "A city that works for everyone."),
                ("Finn Robertson", "Councilor", "Thoughtful growth, grounded in community."),
                ("Grace Kim", "Councilor", "Your voice at the council table."),
                ("Hannah Singh", "School Trustee", "Every student deserves a strong start."),
                ("Isaac Thompson", "School Trustee", "Listening to families. Supporting learning."),
                ("Julia Nguyen", "School Trustee", "Opening doors for the next generation.")
            };
            int index = 0;
            foreach (var (name, office, tagline) in people)
                Add(name, pm.Positions.Single(p => p.Name == office), tagline);
            var otherNames = new[]
            {
                "Liam Anderson",
                "Maya Desai",
                "Noah Williams",
                "Olivia Park",
                "Peter Johnson",
                "Quinn Taylor",
                "Rose Ahmed",
                "Samuel Wright",
                "Tara Morrison"
            };
            foreach (var m in await db.Municipalities.Include(m => m.Positions)
                .Where(m => m.Name != "Pitt Meadows")
                .OrderBy(m => m.Name)
                .ToListAsync())
            {
                foreach (var p in m.Positions.OrderBy(p => p.PositionId))
                    Add(otherNames[index - 12], p, "A stronger community starts with all of us.");
            }

            await db.SaveChangesAsync();
            void Add(string name, Position position, string tagline)
            {
                var slug = string.Join('-', name.ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries));

                db.Candidates.Add(new Candidate
                {
                    Name = name,
                    PositionId = position.PositionId,
                    Tagline = tagline,
                    Profile = $"Fictional candidate for {position.Name}. {tagline}",
                    PhotoFileName = $"seed-portrait-{index++ % 12 + 1}.svg",
                    Website = $"https://example.com/candidates/{slug}",
                    Email = $"{slug}@example.com",
                    SocialUrl = $"https://social.example/@{slug}",
                    VideoUrl = $"https://video.example/watch/{slug}",
                    DateCreated = new DateTime(2026, 8, 1).AddDays(index)
                });
            }
        }

        var demoCandidates = await db.Candidates
            .Where(c => c.PhotoFileName != null && c.PhotoFileName.StartsWith("seed-portrait-"))
            .ToListAsync();
        foreach (var candidate in demoCandidates)
        {
            var slug = string.Join('-', candidate.Name.ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            candidate.Website ??= $"https://example.com/candidates/{slug}";
            candidate.Email ??= $"{slug}@example.com";
            candidate.SocialUrl ??= $"https://social.example/@{slug}";
            candidate.VideoUrl ??= $"https://video.example/watch/{slug}";
        }

        if (demoCandidates.Count > 0)
            await db.SaveChangesAsync();

        var surreyMayor = await db.Positions
            .Include(p => p.Municipality)
            .SingleOrDefaultAsync(p => p.Municipality.Name == "Surrey" && p.Name == "Mayor");
        if (surreyMayor != null && !await db.Candidates.AnyAsync(c =>
            c.Name == "Jordan Williams" && c.PositionId == surreyMayor.PositionId))
        {
            db.Candidates.Add(new Candidate
            {
                Name = "Jordan Williams",
                PositionId = surreyMayor.PositionId,
                Profile = "Fictional candidate for Surrey Mayor. Building a connected, welcoming Surrey.",
                Tagline = "A connected, welcoming Surrey.",
                Website = "https://example.com/candidates/jordan-williams",
                Email = "jordan-williams@example.com",
                SocialUrl = "https://social.example/@jordan-williams",
                VideoUrl = "https://video.example/watch/jordan-williams",
                DateCreated = new DateTime(2026, 8, 20)
            });
            await db.SaveChangesAsync();
        }

        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var seeded = await db.Municipalities.ToListAsync();
        foreach (var name in seeded.Select(m => Roles.ForMunicipality(m.MunicipalityId))
            .Append(Roles.SuperAdmin)
            .Append("Pitt Meadows Admin"))
            if (!await roles.RoleExistsAsync(name))
                Ensure(await roles.CreateAsync(new IdentityRole(name)));
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        await User("super", null, Roles.SuperAdmin);

        if (seeded.Find(m => m.Name == "Pitt Meadows") is { } pitt)
            await User("pm", pitt.MunicipalityId, Roles.ForMunicipality(pitt.MunicipalityId), "Pitt Meadows Admin");

        async Task User(string username, int? municipality, params string[] requiredRoles)
        {
            var user = await users.FindByNameAsync(username);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    MunicipalityId = municipality
                };
                Ensure(await users.CreateAsync(user, "P@$$w0rd"));
            }

            var missing = requiredRoles.Except(await users.GetRolesAsync(user)).ToArray();

            if (missing.Length > 0)
                Ensure(await users.AddToRolesAsync(user, missing));
        }
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
