using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BannerController : Controller
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg"
    };

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BannerController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var banners = await _context.BannerItems.AsNoTracking().OrderByDescending(b => b.Id).ToListAsync();
        return View(banners);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string? title, List<IFormFile> images)
    {
        if (images.Count == 0)
        {
            ModelState.AddModelError("images", "At least one image is required.");
            return View();
        }

        var validationError = images.FirstOrDefault(file => !IsValidImage(file));
        if (validationError is not null)
        {
            ModelState.AddModelError("images", "Only image files are allowed.");
            return View();
        }

        foreach (var image in images)
        {
            var imagePath = await SaveImageAsync(image);
            _context.BannerItems.Add(new BannerItem
            {
                Title = title,
                ImagePath = imagePath
            });
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Banner(s) created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var banner = await _context.BannerItems.FindAsync(id);
        if (banner is null) return NotFound();

        return View(banner);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string? title, IFormFile? image)
    {
        var banner = await _context.BannerItems.FindAsync(id);
        if (banner is null) return NotFound();

        banner.Title = title;

        if (image is not null)
        {
            if (!IsValidImage(image))
            {
                ModelState.AddModelError("image", "Only image files are allowed.");
                return View(banner);
            }

            DeleteImageIfExists(banner.ImagePath);
            banner.ImagePath = await SaveImageAsync(image);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Banner updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var banner = await _context.BannerItems.FindAsync(id);
        if (banner is null) return RedirectToAction(nameof(Index));

        DeleteImageIfExists(banner.ImagePath);
        _context.BannerItems.Remove(banner);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Banner deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private static bool IsValidImage(IFormFile file)
    {
        if (file.Length <= 0) return false;
        var extension = Path.GetExtension(file.FileName);
        return !string.IsNullOrWhiteSpace(extension) && AllowedImageExtensions.Contains(extension);
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var folderPath = Path.Combine(_environment.WebRootPath, "images", "banners");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/banners/{fileName}";
    }

    private void DeleteImageIfExists(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return;
        var fileName = Path.GetFileName(imagePath);
        var physicalPath = Path.Combine(_environment.WebRootPath, "images", "banners", fileName);

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
