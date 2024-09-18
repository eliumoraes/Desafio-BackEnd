using Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //DbSets para entidades do Domain
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        #region User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>().HasData(
            new User(
                Guid.NewGuid(),
                "admin",
                "$2y$10$mWs/f1yA29KsATYQZeAjbu5VPP0bG/SJ.WX3qXjmqbbeaXUjPtrhO", //admin
                UserRole.Admin,
                DateTime.UtcNow,
                DateTime.UtcNow
            )
        );
        #endregion

        #region UserProfile
        modelBuilder.Entity<UserProfile>()
            .HasKey(up => up.UserProfileId);

        modelBuilder.Entity<UserProfile>()
            .HasOne(up => up.User)
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProfile>()
        .HasIndex(up => up.BusinessIdentificationNumber)
        .IsUnique();

        modelBuilder.Entity<UserProfile>()
        .HasIndex(up => up.DriverLicenseNumber)
        .IsUnique();

        modelBuilder.Entity<UserProfile>()
        .Property(up => up.DriverLicenseType)
        .HasConversion(
            v => string.Join(',', v.Select(dlt => dlt.ToString())), //Hashset salva como string
            v => StringToDriverLicenseTypeHashSet(v)
        )
        .Metadata.SetValueComparer(
        new ValueComparer<HashSet<DriverLicenseType>>(
            (c1, c2) => c1.SetEquals(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => new HashSet<DriverLicenseType>(c)
        )
    );
        #endregion
    }

    private static HashSet<DriverLicenseType> StringToDriverLicenseTypeHashSet(string value)
    {
        return new HashSet<DriverLicenseType>(value.Split(',')
            .Select(dlt => Enum.Parse<DriverLicenseType>(dlt))); // Volta para HashSet
    }
}