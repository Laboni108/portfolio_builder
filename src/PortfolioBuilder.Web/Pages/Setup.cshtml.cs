using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;
using PortfolioBuilder.Web.Services;

namespace PortfolioBuilder.Web.Pages;

public class SetupModel : PageModel
{
    private readonly AdminUserRepository _admins;

    public SetupModel(AdminUserRepository admins)
    {
        _admins = admins;
    }

    [BindProperty]
    public SetupInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (await _admins.CountAsync() > 0)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _admins.CountAsync() > 0)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var user = new AdminUser
        {
            Username = Input.Username.Trim(),
            PasswordHash = PasswordHasher.Hash(Input.Password)
        };

        await _admins.AddAsync(user);
        return Redirect("/Admin/Login");
    }

    public class SetupInput
    {
        [Required(ErrorMessage = "Please choose a username.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3 to 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a password.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Password must be at least 10 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please repeat the password.")]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}