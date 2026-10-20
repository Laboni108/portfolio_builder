using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Coding;

public class IndexModel : PageModel
{
    private readonly CodingHighlightRepository _highlights;

    public IndexModel(CodingHighlightRepository highlights)
    {
        _highlights = highlights;
    }

    public List<CodingHighlight> Items { get; private set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Items = (await _highlights.GetAllAsync()).ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        bool deleted = await _highlights.DeleteAsync(id);
        StatusMessage = deleted ? "Highlight deleted." : "That highlight no longer exists.";
        return RedirectToPage();
    }
}