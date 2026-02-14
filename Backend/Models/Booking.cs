namespace Backend.Models;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTimeOffset  StartTime { get; set; }
    public DateTimeOffset  EndTime { get; set; }

    public string Status { get; set; } = "booked";

    public DateTimeOffset  CreatedAt { get; set; } = DateTimeOffset .UtcNow;
    public DateTimeOffset  UpdatedAt { get; set; } = DateTimeOffset .UtcNow;
    public DateTimeOffset ? DeletedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

}
