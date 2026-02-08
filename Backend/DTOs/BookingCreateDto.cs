using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class BookingCreateDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTimeOffset  StartTime { get; set; }

    [Required]
    public DateTimeOffset  EndTime { get; set; }
}
