using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Projects;

public class IndexModel : PageModel
{
    private readonly ProjectRepository _projects;

    public IndexModel(ProjectRepository projects)
    {
        _projects = projects;
    }

    public List<Project> Items { get; private set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Items = (await _projects.GetAllAsync()).ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        bool deleted = await _projects.DeleteAsync(id);
        StatusMessage = deleted ? "Project deleted." : "That project no longer exists.";
        return RedirectToPage();
    }
}