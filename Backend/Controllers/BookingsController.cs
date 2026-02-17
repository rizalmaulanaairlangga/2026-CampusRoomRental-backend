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

    // GET: api/bookings
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
    .Include(b => b.Room)
    .Include(b => b.User)
    .Select(b => new BookingResponseDto
    {
        Id = b.Id,
        RoomId = b.RoomId,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        Status = b.Status.ToString().ToLower(),
        CreatedAt = b.CreatedAt,

        Room = new RoomResponseDto
        {
            Id = b.Room!.Id,
            Name = b.Room.Name,
            Capacity = b.Room.Capacity
        },

        User = new UserResponseDto
        {
            Id = b.User.Id,
            Name = b.User.Name,
            Email = b.User.Email,
            Role = b.User.Role
        }
    })
    .ToListAsync();


        return Ok(bookings);
    }

    // GET: api/bookings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
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
            Status = booking.Status.ToString().ToLower(),
            CreatedAt = booking.CreatedAt,

            Room = new RoomResponseDto
            {
                Id = booking.Room!.Id,
                Name = booking.Room.Name,
                Capacity = booking.Room.Capacity
            },

            User = new UserResponseDto
            {
                Id = booking.User.Id,
                Name = booking.User.Name,
                Email = booking.User.Email,
                Role = booking.User.Role
            }
        });
    }


    // POST: api/bookings
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

        // Conflict only if existing booking is Approved
        var hasConflict = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.DeletedAt == null &&
            (b.Status == BookingStatus.Approved ||
            b.Status == BookingStatus.Pending) &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );


        if (hasConflict)
            {
                return Conflict(new 
                { 
                    message = "Slot already booked or pending approval" 
                });
            }


        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = new Booking
        {
            RoomId = dto.RoomId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            UserId = userId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, null);
    }

    // PUT: api/bookings/{id}/reschedule
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
            b.Status == BookingStatus.Approved &&
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
        return Ok();
    }

    // PUT: api/bookings/{id}/status
    [Authorize(Roles = "admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatusUpdateDto dto)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

        if (booking == null)
            return NotFound();

        if (!Enum.TryParse<BookingStatus>(dto.Status, true, out var status))
            return BadRequest("Invalid status");

        booking.Status = status;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return Ok();
    }

    // DELETE: api/bookings/{id}
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

        booking.Status = BookingStatus.Cancelled;
        booking.DeletedAt = DateTimeOffset.UtcNow;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PATCH: api/bookings/{id}/approve
    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (booking.Status != BookingStatus.Pending)
            return BadRequest("Only pending bookings can be approved");

        booking.Status = BookingStatus.Approved;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    // PATCH: api/bookings/{id}/reject
    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (booking.Status != BookingStatus.Pending)
            return BadRequest("Only pending bookings can be rejected");

        booking.Status = BookingStatus.Rejected;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }
}
