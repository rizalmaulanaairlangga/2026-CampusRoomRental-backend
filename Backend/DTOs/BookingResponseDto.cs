namespace Backend.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
