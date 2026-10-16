using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Services;

namespace PortfolioBuilder.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly AdminAuthService _auth;
    private readonly ProjectRepository _projects;
    private readonly ContactMessageRepository _messages;

    public IndexModel(AdminAuthService auth, ProjectRepository projects, ContactMessageRepository messages)
    {
        _auth = auth;
        _projects = projects;
        _messages = messages;
    }

    public string Username { get; private set; } = string.Empty;
    public int ProjectCount { get; private set; }
    public int UnreadCount { get; private set; }

    public async Task OnGetAsync()
    {
        Username = _auth.CurrentUsername(HttpContext) ?? string.Empty;
        ProjectCount = await _projects.CountAsync();
        UnreadCount = await _messages.CountUnreadAsync();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync(HttpContext);
        return Redirect("/Admin/Login");
    }
}