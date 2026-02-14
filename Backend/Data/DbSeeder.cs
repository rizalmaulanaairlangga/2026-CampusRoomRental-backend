using Backend.Services;
using Backend.Models;

namespace Backend.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // RESET DATA (DEV ONLY)
        context.Bookings.RemoveRange(context.Bookings);
        context.Rooms.RemoveRange(context.Rooms);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var hasher = new PasswordHasher();

        // USERS
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@local",
            Name = "Admin",
            Role = "admin",
            PasswordHash = hasher.Hash("admin123"),
            CreatedAt = DateTimeOffset.UtcNow
        };

        var normalUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@local",
            Name = "User",
            Role = "user",
            PasswordHash = hasher.Hash("user123"),
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Users.AddRange(adminUser, normalUser);
        context.SaveChanges();

        // ROOMS
        var rooms = new[]
        {
            new Room { Name = "Meeting Room A", Capacity = 10 },
            new Room { Name = "Meeting Room B", Capacity = 20 },
            new Room { Name = "Conference Room", Capacity = 50 }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        // TIME SETUP (WIB)
        var wib = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var todayWib = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, wib).Date;

        DateTimeOffset ToUtc(int hour)
        {
            var local = todayWib.AddHours(hour);
            return TimeZoneInfo.ConvertTimeToUtc(local, wib);
        }

        // BOOKINGS
        var bookingConfigs = new[]
        {
            (room: rooms[0], hour: 8, user: normalUser),
            (room: rooms[0], hour: 10, user: normalUser),
            (room: rooms[1], hour: 9, user: adminUser),
            (room: rooms[1], hour: 11, user: adminUser),
            (room: rooms[2], hour: 13, user: normalUser)
        };

        var bookings = bookingConfigs.Select(cfg => new Booking
        {
            RoomId = cfg.room.Id,
            UserId = cfg.user.Id,
            StartTime = ToUtc(cfg.hour),
            EndTime = ToUtc(cfg.hour + 1),
            Status = "confirmed",
            CreatedAt = DateTimeOffset.UtcNow
        });

        context.Bookings.AddRange(bookings);
        context.SaveChanges();
    }
}
