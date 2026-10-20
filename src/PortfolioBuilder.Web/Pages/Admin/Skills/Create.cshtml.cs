using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Skills;

public class CreateModel : PageModel
{
    private readonly SkillCategoryRepository _skills;

    public CreateModel(SkillCategoryRepository skills)
    {
        _skills = skills;
    }

    [BindProperty]
    public SkillInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var category = new SkillCategory
        {
            Name = Input.Name.Trim(),
            IconClass = Input.IconClass.Trim(),
            Items = Input.Items.Trim(),
            DisplayOrder = Input.DisplayOrder
        };

        await _skills.AddAsync(category);

        StatusMessage = "Category added.";
        return RedirectToPage("Index");
    }

    public class SkillInput
    {
        [Required(ErrorMessage = "Please enter a name.")]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an icon class.")]
        [StringLength(60, ErrorMessage = "Icon class must be 60 characters or fewer.")]
        [RegularExpression(@"^[a-zA-Z0-9\- ]+$", ErrorMessage = "Use only letters, numbers, spaces and hyphens, like: fas fa-code")]
        public string IconClass { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter at least one item.")]
        [StringLength(500, ErrorMessage = "Items must be 500 characters or fewer.")]
        public string Items { get; set; } = string.Empty;

        [Range(0, 999, ErrorMessage = "Order must be a number from 0 to 999.")]
        public int DisplayOrder { get; set; }
    }
}