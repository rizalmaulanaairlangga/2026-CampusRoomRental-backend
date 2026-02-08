using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class BookingUpdateDto
{
    [Required]
    public DateTimeOffset  StartTime { get; set; }

    [Required]
    public DateTimeOffset  EndTime { get; set; }

    public string Status { get; set; } = "booked";
}
