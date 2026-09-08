using Microsoft.EntityFrameworkCore;
using SafeLink.API.Models;

namespace SafeLink.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TrustedContact> TrustedContacts => Set<TrustedContact>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
        b.Entity<User>().HasIndex(u => u.NationalId).IsUnique();
        b.Entity<OtpCode>().HasIndex(o => o.PhoneNumber);
    }
}
