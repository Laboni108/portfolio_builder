namespace PortfolioBuilder.Web.Models;

public class AdminUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

        public string? RememberTokenHash { get; set; }
    public DateTime? RememberTokenExpiresAt { get; set; }
}

