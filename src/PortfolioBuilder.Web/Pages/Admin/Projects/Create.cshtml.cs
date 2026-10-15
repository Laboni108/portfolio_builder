using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Projects;

public class CreateModel : PageModel
{
    private readonly ProjectRepository _projects;

    public CreateModel(ProjectRepository projects)
    {
        _projects = projects;
    }

    [BindProperty]
    public ProjectInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var project = new Project
        {
            Title = Input.Title.Trim(),
            Description = Input.Description.Trim(),
            TechStack = Input.TechStack.Trim(),
            GithubUrl = string.IsNullOrWhiteSpace(Input.GithubUrl) ? null : Input.GithubUrl.Trim()
        };

        await _projects.AddAsync(project);

        StatusMessage = "Project added.";
        return RedirectToPage("Index");
    }

    public class ProjectInput
    {
        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(150, ErrorMessage = "Title must be 150 characters or fewer.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description.")]
        [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter at least one technology.")]
        [StringLength(300, ErrorMessage = "Tech stack must be 300 characters or fewer.")]
        public string TechStack { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a full link starting with http:// or https://")]
        [StringLength(300, ErrorMessage = "Link must be 300 characters or fewer.")]
        public string? GithubUrl { get; set; }
    }
}