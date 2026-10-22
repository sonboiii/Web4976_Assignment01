using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web4976_Assignment01.Data.Migrations
{
    // Frozen migration metadata. Future migrations must leave this schema unchanged.
    internal static class InitialSchema
    {
        internal static void Build(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.12");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Web4976_Assignment01.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.Property<int?>("MunicipalityId").HasColumnType("INTEGER");
                    b.HasIndex("MunicipalityId");
                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Web4976_Assignment01.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Web4976_Assignment01.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web4976_Assignment01.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Web4976_Assignment01.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Web4976_Assignment01.Models.Municipality", b => {
                b.Property<int>("MunicipalityId").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("TEXT");
                b.Property<string>("Province").IsRequired().HasMaxLength(60).HasColumnType("TEXT");
                b.Property<DateTime>("ElectionDate").HasColumnType("TEXT");
                b.Property<string>("Description").IsRequired().HasMaxLength(1500).HasColumnType("TEXT");
                b.Property<string>("LogoFileName").HasColumnType("TEXT");
                b.HasKey("MunicipalityId");
                b.HasIndex("Name").IsUnique();
                b.ToTable("Municipalities");
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.Position", b => {
                b.Property<int>("PositionId").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                b.Property<string>("Name").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
                b.Property<int>("SeatsAvailable").HasColumnType("INTEGER");
                b.Property<int>("MunicipalityId").HasColumnType("INTEGER");
                b.HasKey("PositionId");
                b.HasIndex("MunicipalityId","Name").IsUnique();
                b.ToTable("Positions", t=>t.HasCheckConstraint("CK_Position_Seats","SeatsAvailable BETWEEN 1 AND 100"));
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.Candidate", b => {
                b.Property<int>("CandidateId").ValueGeneratedOnAdd().HasColumnType("INTEGER");
                b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("TEXT");
                b.Property<string>("Profile").IsRequired().HasMaxLength(3000).HasColumnType("TEXT");
                b.Property<string>("Tagline").IsRequired().HasMaxLength(180).HasColumnType("TEXT");
                b.Property<string>("PhotoFileName").HasColumnType("TEXT");
                b.Property<string>("Website").HasMaxLength(500).HasColumnType("TEXT");
                b.Property<string>("Email").HasMaxLength(254).HasColumnType("TEXT");
                b.Property<string>("SocialUrl").HasMaxLength(500).HasColumnType("TEXT");
                b.Property<string>("VideoUrl").HasMaxLength(500).HasColumnType("TEXT");
                b.Property<DateTime>("DateCreated").HasColumnType("TEXT");
                b.Property<int>("PositionId").HasColumnType("INTEGER");
                b.HasKey("CandidateId");
                b.HasIndex("PositionId");
                b.ToTable("Candidates");
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.ApplicationUser", b=> {
                b.HasOne("Web4976_Assignment01.Models.Municipality",null).WithMany().HasForeignKey("MunicipalityId").OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.Position", b=> {
                b.HasOne("Web4976_Assignment01.Models.Municipality","Municipality").WithMany("Positions").HasForeignKey("MunicipalityId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.Navigation("Municipality").IsRequired();
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.Candidate", b=> {
                b.HasOne("Web4976_Assignment01.Models.Position","Position").WithMany("Candidates").HasForeignKey("PositionId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.Navigation("Position").IsRequired();
            });
            modelBuilder.Entity("Web4976_Assignment01.Models.Municipality", b=> { b.Navigation("Positions"); });
            modelBuilder.Entity("Web4976_Assignment01.Models.Position", b=> { b.Navigation("Candidates"); });
#pragma warning restore 612, 618
        }
    }
}
