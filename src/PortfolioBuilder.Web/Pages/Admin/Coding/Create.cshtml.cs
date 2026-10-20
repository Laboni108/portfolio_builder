using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Coding;

public class CreateModel : PageModel
{
    private readonly CodingHighlightRepository _highlights;

    public CreateModel(CodingHighlightRepository highlights)
    {
        _highlights = highlights;
    }

    [BindProperty]
    public HighlightInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var highlight = new CodingHighlight
        {
            Icon = Input.Icon.Trim(),
            Title = Input.Title.Trim(),
            DisplayOrder = Input.DisplayOrder
        };

        await _highlights.AddAsync(highlight);

        StatusMessage = "Highlight added.";
        return RedirectToPage("Index");
    }

    public class HighlightInput
    {
        [Required(ErrorMessage = "Please enter an icon (an emoji works well).")]
        [StringLength(20, ErrorMessage = "Icon must be 20 characters or fewer.")]
        public string Icon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(200, ErrorMessage = "Title must be 200 characters or fewer.")]
        public string Title { get; set; } = string.Empty;

        [Range(0, 999, ErrorMessage = "Order must be a number from 0 to 999.")]
        public int DisplayOrder { get; set; }
    }
}