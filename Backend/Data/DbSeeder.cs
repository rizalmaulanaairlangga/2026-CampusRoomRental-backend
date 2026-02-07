using Backend.Models;

namespace Backend.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Rooms.Any()) return;

        var rooms = new List<Room>
        {
            new() { Name = "Meeting Room A", Capacity = 10 },
            new() { Name = "Meeting Room B", Capacity = 20 },
            new() { Name = "Conference Room", Capacity = 50 }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        var bookings = new List<Booking>
        {
            new() {
                RoomId = rooms[0].Id,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            },
            new() {
                RoomId = rooms[0].Id,
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            },
            new() {
                RoomId = rooms[1].Id,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            },
            new() {
                RoomId = rooms[1].Id,
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            },
            new() {
                RoomId = rooms[2].Id,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            }
        };

        context.Bookings.AddRange(bookings);
        context.SaveChanges();
    }
}
