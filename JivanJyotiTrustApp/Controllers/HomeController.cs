using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var studentsTrained = await _context.Trainings.AsNoTracking().SumAsync(x => x.StudentCount);
        var totalPlaces = await _context.Trainings
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => x.Location.Trim().ToLower())
            .Distinct()
            .CountAsync();
        var totalTrainings = await _context.Trainings.AsNoTracking().CountAsync();

        var model = new HomeViewModel
        {
            Banners = await _context.BannerItems.AsNoTracking().ToListAsync(),
            Classes = await _context.ClassItems.AsNoTracking().ToListAsync(),
            TeamMembers = await _context.TeamMembers.AsNoTracking().OrderBy(x => x.Id).ToListAsync(),
            AchievementsCertificatesContent = await _context.Contents
                .AsNoTracking()
                .Include(x => x.ContentFiles)
                .Where(x => x.SectionKey == "achievementsCertificates")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(),
            Stats = new StatsViewModel
            {
                StudentsTrained = studentsTrained,
                TotalPlaces = totalPlaces,
                TotalTrainings = totalTrainings
            }
        };

        return View(model);
    }
}
