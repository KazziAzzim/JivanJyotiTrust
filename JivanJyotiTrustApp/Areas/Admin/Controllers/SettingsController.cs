using JivanJyotiTrustApp.Areas.Admin.ViewModels;
using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new SettingsViewModel
        {
            Phone = await GetSetting("Phone"),
            Email = await GetSetting("Email"),
            Address = await GetSetting("Address"),
            Facebook = await GetSetting("Facebook"),
            Instagram = await GetSetting("Instagram"),
            Twitter = await GetSetting("Twitter"),
            YouTube = await GetSetting("YouTube")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model)
    {
        await SaveSetting("Phone", model.Phone);
        await SaveSetting("Email", model.Email);
        await SaveSetting("Address", model.Address);
        await SaveSetting("Facebook", model.Facebook);
        await SaveSetting("Instagram", model.Instagram);
        await SaveSetting("Twitter", model.Twitter);
        await SaveSetting("YouTube", model.YouTube);

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Settings saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GetSetting(string key)
    {
        return await _context.Settings
            .Where(x => x.Key == key)
            .Select(x => x.Value)
            .FirstOrDefaultAsync() ?? string.Empty;
    }

    private async Task SaveSetting(string key, string value)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Key == key);
        if (setting is null)
        {
            _context.Settings.Add(new Setting { Key = key, Value = value ?? string.Empty });
            return;
        }

        setting.Value = value ?? string.Empty;
    }
}
