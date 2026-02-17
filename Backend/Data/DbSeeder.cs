using Backend.Services;
using Backend.Models;

namespace Backend.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // =============================
        // RESET DATA (DEV ONLY)
        // =============================
        context.Bookings.RemoveRange(context.Bookings);
        context.Rooms.RemoveRange(context.Rooms);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var hasher = new PasswordHasher();

        // =============================
        // USERS
        // =============================
        var users = new List<User>();

        // 4 Admins (tidak membuat booking)
        for (int i = 1; i <= 4; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = $"admin@{i}",
                Name = $"admin{i}",
                Role = "admin",
                PasswordHash = hasher.Hash($"admin{i}"),
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        // 4 Normal Users
        for (int i = 1; i <= 4; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = $"user@{i}",
                Name = $"user{i}",
                Role = "user",
                PasswordHash = hasher.Hash($"user{i}"),
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        context.Users.AddRange(users);
        context.SaveChanges();

        // Ambil 3 user untuk booking
        var user1 = users.First(u => u.Email == "user@1");
        var user2 = users.First(u => u.Email == "user@2");
        var user3 = users.First(u => u.Email == "user@3");
        // user@4 tidak membuat booking

        // =============================
        // ROOMS
        // =============================
        var rooms = new[]
        {
            new Room { Name = "Meeting Room A", Capacity = 10 },
            new Room { Name = "Meeting Room B", Capacity = 20 },
            new Room { Name = "Conference Room", Capacity = 50 }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        // =============================
        // TIME SETUP (WIB)
        // =============================
        var wib = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var todayWib = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, wib).Date;

        DateTimeOffset ToUtc(int hour)
        {
            var local = todayWib.AddHours(hour);
            return TimeZoneInfo.ConvertTimeToUtc(local, wib);
        }

        // =============================
        // BOOKINGS
        // Slot: 7-8, 8-9, 9-10
        // =============================
        var bookings = new List<Booking>
        {
            // USER 1
            new Booking
            {
                RoomId = rooms[0].Id,
                UserId = user1.Id,
                StartTime = ToUtc(7),
                EndTime = ToUtc(8),
                Status = BookingStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Booking
            {
                RoomId = rooms[1].Id,
                UserId = user1.Id,
                StartTime = ToUtc(8),
                EndTime = ToUtc(9),
                Status = BookingStatus.Approved,
                CreatedAt = DateTimeOffset.UtcNow
            },

            // USER 2
            new Booking
            {
                RoomId = rooms[0].Id,
                UserId = user2.Id,
                StartTime = ToUtc(9),
                EndTime = ToUtc(10),
                Status = BookingStatus.Rejected,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Booking
            {
                RoomId = rooms[2].Id,
                UserId = user2.Id,
                StartTime = ToUtc(7),
                EndTime = ToUtc(8),
                Status = BookingStatus.Approved,
                CreatedAt = DateTimeOffset.UtcNow
            },

            // USER 3
            new Booking
            {
                RoomId = rooms[1].Id,
                UserId = user3.Id,
                StartTime = ToUtc(8),
                EndTime = ToUtc(9),
                Status = BookingStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Booking
            {
                RoomId = rooms[2].Id,
                UserId = user3.Id,
                StartTime = ToUtc(9),
                EndTime = ToUtc(10),
                Status = BookingStatus.Approved,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        context.Bookings.AddRange(bookings);
        context.SaveChanges();
    }
}
