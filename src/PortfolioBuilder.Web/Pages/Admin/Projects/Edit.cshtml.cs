using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Projects;

public class EditModel : PageModel
{
    private readonly ProjectRepository _projects;

    public EditModel(ProjectRepository projects)
    {
        _projects = projects;
    }

    [BindProperty]
    public CreateModel.ProjectInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null)
            return NotFound();

        Input = new CreateModel.ProjectInput
        {
            Title = project.Title,
            Description = project.Description,
            TechStack = project.TechStack,
            GithubUrl = project.GithubUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var project = new Project
        {
            Id = id,
            Title = Input.Title.Trim(),
            Description = Input.Description.Trim(),
            TechStack = Input.TechStack.Trim(),
            GithubUrl = string.IsNullOrWhiteSpace(Input.GithubUrl) ? null : Input.GithubUrl.Trim()
        };

        bool updated = await _projects.UpdateAsync(project);
        if (!updated)
            return NotFound();

        StatusMessage = "Project updated.";
        return RedirectToPage("Index");
    }
}