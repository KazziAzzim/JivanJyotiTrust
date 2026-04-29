using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ClassController : Controller
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg"
    };

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ClassController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var classes = await _context.ClassItems.AsNoTracking().OrderByDescending(c => c.Id).ToListAsync();
        return View(classes);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string shortDescription, IFormFile image)
    {
        ValidateClassInputs(name, shortDescription);

        if (image is null || image.Length == 0)
        {
            ModelState.AddModelError("image", "Image is required.");
        }
        else if (!IsValidImage(image))
        {
            ModelState.AddModelError("image", "Only image files are allowed.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        var imagePath = await SaveImageAsync(image);

        _context.ClassItems.Add(new ClassItem
        {
            Name = name.Trim(),
            ShortDescription = shortDescription.Trim(),
            ImagePath = imagePath
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Class created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var classItem = await _context.ClassItems.FindAsync(id);
        if (classItem is null) return NotFound();

        return View(classItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, string shortDescription, IFormFile? image)
    {
        var classItem = await _context.ClassItems.FindAsync(id);
        if (classItem is null) return NotFound();

        ValidateClassInputs(name, shortDescription);

        if (image is not null && !IsValidImage(image))
        {
            ModelState.AddModelError("image", "Only image files are allowed.");
        }

        if (!ModelState.IsValid)
        {
            classItem.Name = name;
            classItem.ShortDescription = shortDescription;
            return View(classItem);
        }

        classItem.Name = name.Trim();
        classItem.ShortDescription = shortDescription.Trim();

        if (image is not null)
        {
            DeleteImageIfExists(classItem.ImagePath);
            classItem.ImagePath = await SaveImageAsync(image);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Class updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var classItem = await _context.ClassItems.FindAsync(id);
        if (classItem is null) return RedirectToAction(nameof(Index));

        DeleteImageIfExists(classItem.ImagePath);
        _context.ClassItems.Remove(classItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Class deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateClassInputs(string? name, string? shortDescription)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(shortDescription))
        {
            ModelState.AddModelError("shortDescription", "Description is required.");
        }
    }

    private static bool IsValidImage(IFormFile file)
    {
        if (file.Length <= 0) return false;
        var extension = Path.GetExtension(file.FileName);
        return !string.IsNullOrWhiteSpace(extension) && AllowedImageExtensions.Contains(extension);
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var folderPath = Path.Combine(_environment.WebRootPath, "images", "classes");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/classes/{fileName}";
    }

    private void DeleteImageIfExists(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return;

        var fileName = Path.GetFileName(imagePath);
        var physicalPath = Path.Combine(_environment.WebRootPath, "images", "classes", fileName);

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
