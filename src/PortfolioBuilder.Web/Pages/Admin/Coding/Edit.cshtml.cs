using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Coding;

public class EditModel : PageModel
{
    private readonly CodingHighlightRepository _highlights;

    public EditModel(CodingHighlightRepository highlights)
    {
        _highlights = highlights;
    }

    [BindProperty]
    public CreateModel.HighlightInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var highlight = await _highlights.GetByIdAsync(id);
        if (highlight == null)
            return NotFound();

        Input = new CreateModel.HighlightInput
        {
            Icon = highlight.Icon,
            Title = highlight.Title,
            DisplayOrder = highlight.DisplayOrder
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var highlight = new CodingHighlight
        {
            Id = id,
            Icon = Input.Icon.Trim(),
            Title = Input.Title.Trim(),
            DisplayOrder = Input.DisplayOrder
        };

        bool updated = await _highlights.UpdateAsync(highlight);
        if (!updated)
            return NotFound();

        StatusMessage = "Highlight updated.";
        return RedirectToPage("Index");
    }
}