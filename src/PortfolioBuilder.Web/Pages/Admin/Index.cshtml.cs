using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Services;

namespace PortfolioBuilder.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly AdminAuthService _auth;

    public IndexModel(AdminAuthService auth)
    {
        _auth = auth;
    }

    public string Username { get; private set; } = string.Empty;

    public void OnGet()
    {
        Username = _auth.CurrentUsername(HttpContext) ?? string.Empty;
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync(HttpContext);
        return Redirect("/Admin/Login");
    }
}