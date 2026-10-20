namespace PortfolioBuilder.Web.Models;

public class CodingHighlight
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}