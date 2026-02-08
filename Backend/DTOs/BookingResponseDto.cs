namespace Backend.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTimeOffset  StartTime { get; set; }
    public DateTimeOffset  EndTime { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset  CreatedAt { get; set; }
}
