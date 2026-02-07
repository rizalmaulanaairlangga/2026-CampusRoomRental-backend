namespace Backend.DTOs;

public class RoomResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
