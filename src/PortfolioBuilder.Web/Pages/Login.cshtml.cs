using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Services;

namespace PortfolioBuilder.Web.Pages.Admin;

public class LoginModel : PageModel
{
    private static readonly string DummyHash = PasswordHasher.Hash("not-a-real-password");

    private readonly AdminUserRepository _admins;
    private readonly AdminAuthService _auth;

    public LoginModel(AdminUserRepository admins, AdminAuthService auth)
    {
        _admins = admins;
        _auth = auth;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (await _auth.TryRestoreFromCookieAsync(HttpContext))
            return LocalRedirect(SafeReturnUrl());

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _admins.GetByUsernameAsync(Input.Username.Trim());
        bool passwordOk = PasswordHasher.Verify(Input.Password, user?.PasswordHash ?? DummyHash);

        if (user == null || !passwordOk)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        await _auth.SignInAsync(HttpContext, user, Input.RememberMe);
        return LocalRedirect(SafeReturnUrl());
    }

    private string SafeReturnUrl() =>
        !string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/Admin";

    public class LoginInput
    {
        [Required(ErrorMessage = "Please enter your username.")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}