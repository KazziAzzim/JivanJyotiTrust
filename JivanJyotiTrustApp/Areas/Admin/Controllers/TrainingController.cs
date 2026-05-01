using JivanJyotiTrustApp.Areas.Admin.ViewModels;
using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TrainingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public TrainingController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index() => View(await _context.Trainings.Include(t => t.City).AsNoTracking().ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.Trainings.Include(x => x.City).Include(x => x.GalleryItems).FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create() => View("Form", await BuildFormAsync(new TrainingFormViewModel()));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainingFormViewModel model)
    {
        if (!ModelState.IsValid || model.CityId is null) return View("Form", await BuildFormAsync(model));
        var training = new Training { CityId = model.CityId.Value, Title = model.Title.Trim(), Location = model.Location.Trim(), Description = model.Description.Trim() };
        _context.Trainings.Add(training);
        await _context.SaveChangesAsync();
        await SaveImagesAsync(training.Id, model.Images);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Trainings.Include(x => x.GalleryItems).FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        return View("Form", await BuildFormAsync(new TrainingFormViewModel { Id = item.Id, CityId = item.CityId, Title = item.Title, Location = item.Location, Description = item.Description, ExistingGalleryItems = item.GalleryItems.ToList() }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainingFormViewModel model)
    {
        var item = await _context.Trainings.FindAsync(id);
        if (item is null) return NotFound();
        if (!ModelState.IsValid || model.CityId is null) { model.Id = id; return View("Form", await BuildFormAsync(model)); }
        item.CityId = model.CityId.Value; item.Title = model.Title.Trim(); item.Location = model.Location.Trim(); item.Description = model.Description.Trim();
        await _context.SaveChangesAsync();
        await SaveImagesAsync(item.Id, model.Images);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Trainings.Include(x => x.City).FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.Trainings.Include(x => x.GalleryItems).FirstOrDefaultAsync(x => x.Id == id);
        if (item is not null) _context.Trainings.Remove(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<TrainingFormViewModel> BuildFormAsync(TrainingFormViewModel model)
    {
        model.CityOptions = await _context.Cities.OrderBy(x => x.Name).ToListAsync();
        if (model.Id.HasValue && model.ExistingGalleryItems.Count == 0)
            model.ExistingGalleryItems = await _context.GalleryItems.Where(x => x.TrainingId == model.Id).ToListAsync();
        return model;
    }

    private async Task SaveImagesAsync(int trainingId, List<IFormFile> images)
    {
        if (images.Count == 0) return;
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "training");
        Directory.CreateDirectory(folder);
        foreach (var image in images.Where(i => i.Length > 0))
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(folder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);
            _context.GalleryItems.Add(new GalleryItem { TrainingId = trainingId, ImageUrl = $"/uploads/training/{fileName}" });
        }
        await _context.SaveChangesAsync();
    }
}
