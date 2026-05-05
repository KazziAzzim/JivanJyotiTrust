using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ApiController(ApplicationDbContext context) => _context = context;

    [HttpGet("/api/cities")]
    public async Task<IActionResult> GetCities([FromQuery] string? q)
    {
        var query = _context.Cities.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Name.Contains(q));
        return Ok(await query.OrderBy(x => x.Name).Take(20).Select(x => new { id = x.Id, text = x.Name }).ToListAsync());
    }

    [HttpPost("/api/cities")]
    public async Task<IActionResult> AddCity([FromBody] City input)
    {
        if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest();
        var name = input.Name.Trim();
        var existing = await _context.Cities.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        if (existing is not null) return Ok(new { id = existing.Id, name = existing.Name, text = existing.Name });
        var city = new City { Name = name };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();
        return Ok(new { id = city.Id, name = city.Name, text = city.Name });
    }

    [HttpGet("/api/gallery/random")]
    public async Task<IActionResult> RandomGallery() => Ok(await _context.GalleryItems.Include(x => x.Training).OrderBy(x => Guid.NewGuid()).Take(10).Select(x => new { x.Id, x.ImageUrl, trainingId = x.TrainingId, location = x.Training!.Location }).ToListAsync());

    [HttpGet("/api/training/gallery")]
    public async Task<IActionResult> TrainingGallery([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 8 : pageSize;

        var skip = (page - 1) * pageSize;

        var items = await _context.GalleryItems
            .AsNoTracking()
            .Include(x => x.Training)
            .OrderByDescending(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new
            {
                imageUrl = x.ImageUrl,
                trainingTitle = x.Training != null ? (x.Training.Title ?? x.Training.Location) : string.Empty
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("/api/classes/random")]
    public async Task<IActionResult> RandomClasses() => Ok(await _context.ClassItems.AsNoTracking().OrderBy(x => Guid.NewGuid()).Take(10).Select(x => new { x.Id, title = x.Name, imageUrl = x.ImagePath }).ToListAsync());
}
