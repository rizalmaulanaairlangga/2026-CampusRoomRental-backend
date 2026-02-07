using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /bookings
    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var bookings = await _context.Bookings
            .Where(b => b.DeletedAt == null)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // GET /bookings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Where(b => b.Id == id && b.DeletedAt == null)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        return Ok(booking);
    }

    // POST /bookings
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.EndTime <= dto.StartTime)
            return BadRequest(new { message = "EndTime must be after StartTime" });

        var roomExists = await _context.Rooms
            .AnyAsync(r => r.Id == dto.RoomId && r.DeletedAt == null);

        if (!roomExists)
            return BadRequest(new { message = "Room does not exist" });

        // 🔴 OVERLAPPING CHECK
        var hasConflict = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.DeletedAt == null &&
            b.Status == "booked" &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );

        if (hasConflict)
            return Conflict(new { message = "Room is already booked in the selected time range" });

        var booking = new Booking
        {
            RoomId = dto.RoomId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    // PUT /bookings/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.EndTime <= dto.StartTime)
            return BadRequest(new { message = "EndTime must be after StartTime" });

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        // 🔴 OVERLAP CHECK (exclude current booking)
        var hasConflict = await _context.Bookings.AnyAsync(b =>
            b.RoomId == booking.RoomId &&
            b.Id != id &&
            b.DeletedAt == null &&
            b.Status == "booked" &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );

        if (hasConflict)
            return Conflict(new { message = "Booking time conflicts with another booking" });

        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.Status = dto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(booking);
    }

    // DELETE /bookings/{id} (Cancel booking)
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        booking.Status = "cancelled";
        booking.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
