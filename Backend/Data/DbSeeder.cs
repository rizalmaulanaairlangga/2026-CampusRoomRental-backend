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

        var wib = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var todayWib = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, wib).Date;

        // helper untuk bikin booking 1 jam
        Booking CreateBooking(int roomId, int startHour)
        {
            var startLocal = todayWib.AddHours(startHour);
            var endLocal = startLocal.AddHours(1);

            return new Booking
            {
                RoomId = roomId,
                StartTime = startLocal.ToUniversalTime(),
                EndTime = endLocal.ToUniversalTime(),
                Status = "booked",
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        var bookings = new List<Booking>
        {
            // Room 1
            CreateBooking(rooms[0].Id, 8),  // 08:00–09:00
            CreateBooking(rooms[0].Id, 10), // 10:00–11:00

            // Room 2
            CreateBooking(rooms[1].Id, 8),
            CreateBooking(rooms[1].Id, 10),

            // Room 3
            CreateBooking(rooms[2].Id, 9)
        };

        context.Bookings.AddRange(bookings);
        context.SaveChanges();
    }
}
