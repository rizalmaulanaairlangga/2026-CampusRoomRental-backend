using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // other modelBuilder configurations...

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20); // optional: set length for string column

        base.OnModelCreating(modelBuilder);
    }
}
