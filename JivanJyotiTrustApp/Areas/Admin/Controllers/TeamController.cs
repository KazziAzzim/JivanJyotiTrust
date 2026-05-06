using JivanJyotiTrustApp.Data;
using JivanJyotiTrustApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JivanJyotiTrustApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TeamController : Controller
{
    private readonly ApplicationDbContext _context;

    public TeamController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var teamMembers = await _context.TeamMembers.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        return View(teamMembers);
    }

    [HttpGet]
    public IActionResult Create() => View(new TeamMember());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamMember teamMember)
    {
        ValidateTeamMember(teamMember);

        if (!ModelState.IsValid)
        {
            return View(teamMember);
        }

        teamMember.Name = teamMember.Name.Trim();
        teamMember.Designation = teamMember.Designation.Trim();

        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Team member created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var teamMember = await _context.TeamMembers.FindAsync(id);
        return teamMember is null ? NotFound() : View(teamMember);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TeamMember teamMember)
    {
        if (id != teamMember.Id)
        {
            return NotFound();
        }

        ValidateTeamMember(teamMember);

        if (!ModelState.IsValid)
        {
            return View(teamMember);
        }

        var existingTeamMember = await _context.TeamMembers.FindAsync(id);
        if (existingTeamMember is null)
        {
            return NotFound();
        }

        existingTeamMember.Name = teamMember.Name.Trim();
        existingTeamMember.Designation = teamMember.Designation.Trim();

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Team member updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var teamMember = await _context.TeamMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return teamMember is null ? NotFound() : View(teamMember);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var teamMember = await _context.TeamMembers.FindAsync(id);
        if (teamMember is not null)
        {
            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Team member deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidateTeamMember(TeamMember teamMember)
    {
        if (string.IsNullOrWhiteSpace(teamMember.Name))
        {
            ModelState.AddModelError(nameof(teamMember.Name), "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(teamMember.Designation))
        {
            ModelState.AddModelError(nameof(teamMember.Designation), "Designation is required.");
        }
    }
}
