using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Models;
using Microsoft.AspNetCore.Identity;

namespace Web4976_Assignment01.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Municipality> Municipalities => Set<Municipality>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<Candidate> Candidates => Set<Candidate>();

    public IQueryable<Candidate> Profiles => Candidates.Include(c => c.Position).ThenInclude(p => p.Municipality);

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Municipality>().HasIndex(x => x.Name).IsUnique();
        b.Entity<Position>().HasIndex(x => new { x.MunicipalityId, x.Name }).IsUnique();
        b.Entity<Position>()
            .ToTable(t => t.HasCheckConstraint("CK_Position_Seats", "SeatsAvailable BETWEEN 1 AND 100"));
        b.Entity<Position>()
            .HasOne(x => x.Municipality)
            .WithMany(x => x.Positions)
            .HasForeignKey(x => x.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);
        b.Entity<Candidate>()
            .HasOne(x => x.Position)
            .WithMany(x => x.Candidates)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
        b.Entity<ApplicationUser>()
            .HasOne<Municipality>()
            .WithMany()
            .HasForeignKey(x => x.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);
        b.Entity<Candidate>().Ignore(x => x.PhotoUrl);
    }
}
