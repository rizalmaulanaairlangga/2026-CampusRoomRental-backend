using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RoomUpdateDto
{
    [Required]
    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = "active";
}
