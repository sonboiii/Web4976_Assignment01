using MEMS.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Data;

public class ElectionUser : IdentityUser
{
    public int? MunicipalityId { get; set; }
}

public class MunicipalityLogo
{
    public int MunicipalityId { get; set; }
    public byte[] Bytes { get; set; } = [];
}

public class CandidatePhoto
{
    public int CandidateId { get; set; }
    public byte[] Bytes { get; set; } = [];
}

public class ElectionDb(DbContextOptions<ElectionDb> options) : IdentityDbContext<ElectionUser>(options)
{
    public DbSet<Municipality> Municipalities => Set<Municipality>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<MunicipalityLogo> Logos => Set<MunicipalityLogo>();
    public DbSet<CandidatePhoto> Photos => Set<CandidatePhoto>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
        model.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
        model.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
        model.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(128);
        var municipality = model.Entity<Municipality>();
        municipality.HasIndex(m => m.Slug).IsUnique();
        municipality.HasIndex(m => m.Name).IsUnique();
        municipality.ToTable(t => t.HasCheckConstraint("CK_Municipalities_Name", "LEN(TRIM([Name])) > 0"));
        var position = model.Entity<Position>();
        position.HasIndex(p => new { p.MunicipalityId, p.Name }).IsUnique();
        position.HasAlternateKey(p => new { p.MunicipalityId, p.Id });
        position.HasOne<Municipality>().WithMany().HasForeignKey(p => p.MunicipalityId).OnDelete(DeleteBehavior.Restrict);
        position.ToTable(t => t.HasCheckConstraint("CK_Positions_Seats", "[SeatsAvailable] BETWEEN 1 AND 100"));
        model.Entity<Candidate>().HasOne<Position>().WithMany()
            .HasForeignKey(c => new { c.MunicipalityId, c.PositionId })
            .HasPrincipalKey(p => new { p.MunicipalityId, p.Id }).OnDelete(DeleteBehavior.Restrict);
        model.Entity<ElectionUser>().HasOne<Municipality>().WithMany()
            .HasForeignKey(u => u.MunicipalityId).OnDelete(DeleteBehavior.Restrict);
        model.Entity<MunicipalityLogo>().HasKey(x => x.MunicipalityId);
        model.Entity<MunicipalityLogo>().HasOne<Municipality>().WithOne().HasForeignKey<MunicipalityLogo>(x => x.MunicipalityId);
        model.Entity<CandidatePhoto>().HasKey(x => x.CandidateId);
        model.Entity<CandidatePhoto>().HasOne<Candidate>().WithOne().HasForeignKey<CandidatePhoto>(x => x.CandidateId);
        model.Entity<MunicipalityLogo>().ToTable(t => t.HasCheckConstraint("CK_Logos_Length", "DATALENGTH([Bytes]) BETWEEN 12 AND 2097152"));
        model.Entity<CandidatePhoto>().ToTable(t => t.HasCheckConstraint("CK_Photos_Length", "DATALENGTH([Bytes]) BETWEEN 12 AND 2097152"));
    }
}
