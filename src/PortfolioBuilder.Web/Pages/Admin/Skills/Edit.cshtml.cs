using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Skills;

public class EditModel : PageModel
{
    private readonly SkillCategoryRepository _skills;

    public EditModel(SkillCategoryRepository skills)
    {
        _skills = skills;
    }

    [BindProperty]
    public CreateModel.SkillInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _skills.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        Input = new CreateModel.SkillInput
        {
            Name = category.Name,
            IconClass = category.IconClass,
            Items = category.Items,
            DisplayOrder = category.DisplayOrder
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var category = new SkillCategory
        {
            Id = id,
            Name = Input.Name.Trim(),
            IconClass = Input.IconClass.Trim(),
            Items = Input.Items.Trim(),
            DisplayOrder = Input.DisplayOrder
        };

        bool updated = await _skills.UpdateAsync(category);
        if (!updated)
            return NotFound();

        StatusMessage = "Category updated.";
        return RedirectToPage("Index");
    }
}