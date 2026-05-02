using JivanJyotiTrustApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Controllers;

public class PublicContentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PublicContentController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Section(string sectionKey)
    {
        return PartialView("_DynamicContent", sectionKey);
    }

    [HttpGet("Content/Download/{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        var file = await _context.ContentFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (file is null) return NotFound();

        var fullPath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(fullPath)) return NotFound();

        return PhysicalFile(fullPath, "application/octet-stream", file.FileName);
    }
}
