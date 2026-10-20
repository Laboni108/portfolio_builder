using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Skills;

public class IndexModel : PageModel
{
    private readonly SkillCategoryRepository _skills;

    public IndexModel(SkillCategoryRepository skills)
    {
        _skills = skills;
    }

    public List<SkillCategory> Items { get; private set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Items = (await _skills.GetAllAsync()).ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        bool deleted = await _skills.DeleteAsync(id);
        StatusMessage = deleted ? "Category deleted." : "That category no longer exists.";
        return RedirectToPage();
    }
}