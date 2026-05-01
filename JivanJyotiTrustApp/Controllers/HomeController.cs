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
        var model = new HomeViewModel
        {
            Banners = await _context.BannerItems.AsNoTracking().ToListAsync(),
            Classes = await _context.ClassItems.AsNoTracking().ToListAsync()
        };

        return View(model);
    }
}
