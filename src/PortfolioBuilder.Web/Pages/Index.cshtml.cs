using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ProjectRepository _projects;
    private readonly SkillCategoryRepository _skills;
    private readonly CodingHighlightRepository _highlights;
    private readonly ContactMessageRepository _messages;

    public IndexModel(
        ProjectRepository projects,
        SkillCategoryRepository skills,
        CodingHighlightRepository highlights,
        ContactMessageRepository messages)
    {
        _projects = projects;
        _skills = skills;
        _highlights = highlights;
        _messages = messages;
    }

    public List<Project> Projects { get; private set; } = new();

    public List<SkillCategory> Skills { get; private set; } = new();

    public List<CodingHighlight> Highlights { get; private set; } = new();

    [BindProperty]
    public ContactInput Contact { get; set; } = new();

    [TempData]
    public string? ContactSuccess { get; set; }

    public async Task OnGetAsync()
    {
        await LoadContentAsync();
    }

    public async Task<IActionResult> OnPostContactAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadContentAsync();
            return Page();
        }

        var message = new ContactMessage
        {
            Name = Contact.Name.Trim(),
            Email = Contact.Email.Trim(),
            Subject = string.IsNullOrWhiteSpace(Contact.Subject) ? null : Contact.Subject.Trim(),
            Message = Contact.Message.Trim()
        };

        await _messages.AddAsync(message);

        ContactSuccess = "Thanks! Your message has been sent.";
        return Redirect("/#contact");
    }

    private async Task LoadContentAsync()
    {
        Projects = (await _projects.GetAllAsync()).ToList();
        Skills = (await _skills.GetAllAsync()).ToList();
        Highlights = (await _highlights.GetAllAsync()).ToList();
    }
}