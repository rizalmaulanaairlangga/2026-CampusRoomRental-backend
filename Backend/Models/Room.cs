namespace Backend.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Status { get; set; } = "active";

    public DateTimeOffset  CreatedAt { get; set; } = DateTimeOffset .UtcNow;
    public DateTimeOffset  UpdatedAt { get; set; } = DateTimeOffset .UtcNow;
    public DateTimeOffset ? DeletedAt { get; set; }

    public ICollection<Booking>? Bookings { get; set; }
}
