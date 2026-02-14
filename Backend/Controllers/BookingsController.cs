using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        var query = _context.Bookings
            .Where(b => b.DeletedAt == null);

        if (role != "admin")
        {
            query = query.Where(b => b.UserId == userId);
        }

        var bookings = await query
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
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (booking.UserId != userId && role != "admin")
            return Forbid();

        return Ok(new BookingResponseDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        });
    }

    // POST /bookings
    [Authorize(Roles = "user")]
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

        var hasConflict = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.DeletedAt == null &&
            b.Status == "booked" &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );

        if (hasConflict)
            return Conflict(new { message = "Room is already booked in the selected time range" });

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = new Booking
        {
            RoomId = dto.RoomId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            UserId = userId,
            Status = "booked",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    // PUT /bookings/{id}
    [Authorize(Roles = "user")]
    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> RescheduleBooking(int id, BookingRescheduleDto dto)
    {
        if (dto.EndTime <= dto.StartTime)
            return BadRequest("EndTime must be after StartTime");

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (booking.UserId != userId)
            return Forbid();

        var hasConflict = await _context.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.RoomId == dto.RoomId &&
            b.DeletedAt == null &&
            b.Status == "booked" &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );

        if (hasConflict)
            return Conflict("Room already booked");

        booking.RoomId = dto.RoomId;
        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(booking);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatusUpdateDto dto)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound();

        booking.Status = dto.Status;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(booking);
    }

    // DELETE /bookings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (booking.UserId != userId && role != "admin")
            return Forbid();

        booking.Status = "cancelled";
        booking.DeletedAt = DateTimeOffset.UtcNow;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
