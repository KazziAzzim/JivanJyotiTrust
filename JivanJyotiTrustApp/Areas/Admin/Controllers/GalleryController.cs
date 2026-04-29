using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class GalleryController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _context;

    public GalleryController(IWebHostEnvironment environment, ApplicationDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile image, string? title)
    {
        if (image is null || image.Length == 0)
        {
            TempData["Error"] = "Please choose an image to upload.";
            return RedirectToAction(nameof(Index));
        }

        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await image.CopyToAsync(stream);
        }

        _context.GalleryItems.Add(new GalleryItem
        {
            Title = title,
            ImagePath = $"/uploads/{fileName}"
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = "Image uploaded successfully.";
        return RedirectToAction(nameof(Index));
    }
}
