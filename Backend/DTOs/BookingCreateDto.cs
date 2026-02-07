using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class BookingCreateDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}
