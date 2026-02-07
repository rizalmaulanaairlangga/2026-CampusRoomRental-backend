using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class BookingUpdateDto
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public string Status { get; set; } = "booked";
}
