using JivanJyotiTrustApp.Areas.Admin.ViewModels;
using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContentController : Controller
{
    private static readonly HashSet<string> AllowedExtensions = [".pdf", ".docx", ".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ContentController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index() => View(await _context.Contents.Include(x => x.ContentFiles).OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync());

    public IActionResult Create() => View("Form", new ContentFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContentFormViewModel model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        var entity = new Content { Title = model.Title.Trim(), Description = model.Description.Trim(), SectionKey = model.SectionKey.Trim() };
        _context.Contents.Add(entity);
        await _context.SaveChangesAsync();
        await SaveFilesAsync(entity.Id, model.Files);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Contents.Include(x => x.ContentFiles).FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();

        return View("Form", new ContentFormViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            SectionKey = item.SectionKey,
            ExistingFiles = item.ContentFiles.ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContentFormViewModel model)
    {
        var item = await _context.Contents.Include(x => x.ContentFiles).FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        if (!ModelState.IsValid)
        {
            model.Id = id;
            model.ExistingFiles = item.ContentFiles.ToList();
            return View("Form", model);
        }

        item.Title = model.Title.Trim();
        item.Description = model.Description.Trim();
        item.SectionKey = model.SectionKey.Trim();
        await _context.SaveChangesAsync();
        await SaveFilesAsync(item.Id, model.Files);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Contents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Contents.Include(x => x.ContentFiles).FirstOrDefaultAsync(x => x.Id == id);
        if (item is not null)
        {
            foreach (var file in item.ContentFiles)
            {
                var fullPath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
            _context.Contents.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task SaveFilesAsync(int contentId, IEnumerable<IFormFile> files)
    {
        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "content");
        Directory.CreateDirectory(uploadFolder);

        foreach (var file in files.Where(x => x.Length > 0))
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                ModelState.AddModelError(nameof(ContentFormViewModel.Files), $"Unsupported file type: {file.FileName}");
                continue;
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadFolder, fileName);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            _context.ContentFiles.Add(new ContentFile
            {
                ContentId = contentId,
                FileName = file.FileName,
                FilePath = $"/uploads/content/{fileName}"
            });
        }

        await _context.SaveChangesAsync();
    }
}
