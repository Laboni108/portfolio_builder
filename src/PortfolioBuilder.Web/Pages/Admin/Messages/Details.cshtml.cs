using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Messages;

public class DetailsModel : PageModel
{
    private readonly ContactMessageRepository _messages;

    public DetailsModel(ContactMessageRepository messages)
    {
        _messages = messages;
    }

    public ContactMessage? Item { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await _messages.GetByIdAsync(id);
        if (Item == null)
            return NotFound();

        if (!Item.IsRead)
        {
            await _messages.MarkAsReadAsync(id);
            Item.IsRead = true;
        }

        return Page();
    }
}