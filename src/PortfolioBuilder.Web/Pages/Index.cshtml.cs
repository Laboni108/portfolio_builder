using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ProjectRepository _projects;

    public IndexModel(ProjectRepository projects)
    {
        _projects = projects;
    }

    public List<Project> Projects { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Projects = (await _projects.GetAllAsync()).ToList();
    }
}