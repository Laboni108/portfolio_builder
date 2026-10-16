using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Pages.Admin.Messages;

public class IndexModel : PageModel
{
    private readonly ContactMessageRepository _messages;

    public IndexModel(ContactMessageRepository messages)
    {
        _messages = messages;
    }

    public List<ContactMessage> Items { get; private set; } = new();

    public int UnreadCount { get; private set; }

    public async Task OnGetAsync()
    {
        Items = (await _messages.GetAllAsync()).ToList();
        UnreadCount = Items.Count(m => !m.IsRead);
    }
}