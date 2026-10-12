namespace PortfolioBuilder.Web.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
    public string? GithubUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}