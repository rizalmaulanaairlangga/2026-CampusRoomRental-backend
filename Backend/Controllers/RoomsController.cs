using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /rooms
    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _context.Rooms
            .Where(r => r.DeletedAt == null)
            .Select(r => new RoomResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(rooms);
    }

    // GET /rooms/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _context.Rooms
            .Where(r => r.Id == id && r.DeletedAt == null)
            .Select(r => new RoomResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (room == null)
            return NotFound(new { message = "Room not found" });

        return Ok(room);
    }

    // POST /rooms
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var room = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            Status = dto.Status
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    // PUT /rooms/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

        if (room == null)
            return NotFound(new { message = "Room not found" });

        room.Name = dto.Name;
        room.Capacity = dto.Capacity;
        room.Status = dto.Status;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(room);
    }

    // DELETE /rooms/{id} (Soft Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

        if (room == null)
            return NotFound(new { message = "Room not found" });

        room.DeletedAt = DateTime.UtcNow;
        room.Status = "inactive";
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // PUT /rooms/{id}/restore
    [HttpPut("{id}/restore")]
    public async Task<IActionResult> RestoreRoom(int id)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt != null);

        if (room == null)
            return NotFound(new { message = "Room not found or not deleted" });

        room.DeletedAt = null;
        room.Status = "active";
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Room restored successfully" });
    }

}
