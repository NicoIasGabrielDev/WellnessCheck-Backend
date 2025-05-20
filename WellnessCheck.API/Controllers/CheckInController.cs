using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WellnessCheck.API.Data;
using WellnessCheck.API.Dtos;
using WellnessCheck.API.Entities;

[ApiController]
[Route("checkins")]
public class CheckInController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheckInController(AppDbContext context)
    {
        _context = context;
    }

    // POST /checkins
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCheckInDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var checkIn = new CheckIn
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Mood = dto.Mood,
            Productivity = dto.Productivity,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.CheckIns.Add(checkIn);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Check-in criado com sucesso!" });
    }

    // GET /checkins
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetForAuthenticatedUser()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var checkIns = await _context.CheckIns
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CheckInResponseDto
            {
                Id = c.Id,
                Mood = c.Mood,
                Productivity = c.Productivity,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(checkIns);
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchCheckIns([FromQuery] CheckInFilterDto filter)
    {
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        if (roleClaim != "Admin")
            return Forbid();

        var query = _context.CheckIns.AsQueryable();

        if (filter.UserId.HasValue)
            query = query.Where(c => c.UserId == filter.UserId.Value);

        if (filter.From.HasValue)
            query = query.Where(c => c.CreatedAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(c => c.CreatedAt <= filter.To.Value);

        if (filter.Mood.HasValue)
            query = query.Where(c => c.Mood == filter.Mood.Value);

        if (filter.Productivity.HasValue)
            query = query.Where(c => c.Productivity == filter.Productivity.Value);

        var result = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CheckInResponseDto
            {
                Id = c.Id,
                Mood = c.Mood,
                Productivity = c.Productivity,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("alerts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAlertUsers()
    {
        var users = await _context.Users
            .Include(u => u.CheckIns)
            .ToListAsync();

        var alertList = new List<AlertResponseDto>();

        foreach (var user in users)
        {
            var orderedCheckIns = user.CheckIns
                .OrderByDescending(c => c.CreatedAt)
                .Take(7) // últimos dias, ajustável
                .ToList();

            int consecutive = 0;
            foreach (var checkIn in orderedCheckIns)
            {
                if (checkIn.Mood <= 2)
                    consecutive++;
                else
                    break;
            }

            if (consecutive >= 2)
            {
                alertList.Add(new AlertResponseDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    ConsecutiveBadDays = consecutive
                });
            }
        }

        return Ok(alertList);
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats(
    [FromQuery] Guid? userId,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] string groupBy = "week")
    {
        var query = _context.CheckIns.AsQueryable();

        if (userId.HasValue)
            query = query.Where(c => c.UserId == userId.Value);

        if (from.HasValue)
            query = query.Where(c => c.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.CreatedAt <= to.Value);

        var stats = await query
            .GroupBy(c => groupBy.ToLower() == "month"
                ? $"{c.CreatedAt.Year}-{c.CreatedAt.Month:D2}"
                : $"{c.CreatedAt.Year}-W{System.Globalization.ISOWeek.GetWeekOfYear(c.CreatedAt)}")
            .Select(g => new CheckInStatsDto
            {
                Period = g.Key,
                AvgMood = Math.Round(g.Average(c => c.Mood), 2),
                AvgProductivity = Math.Round(g.Average(c => (int)c.Productivity), 2)
            })
            .OrderByDescending(g => g.Period)
            .ToListAsync();

        return Ok(stats);
    }

}