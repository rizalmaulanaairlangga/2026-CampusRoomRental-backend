// Backend/DTOs/BookingResponseDto.cs
namespace Backend.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }

    public RoomResponseDto? Room { get; set; }
    public UserResponseDto? User { get; set; }

    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }

    // change to string
    public string Status { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
}
