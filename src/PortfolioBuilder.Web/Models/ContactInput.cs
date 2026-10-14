using System.ComponentModel.DataAnnotations;

namespace PortfolioBuilder.Web.Models;

public class ContactInput
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(150, ErrorMessage = "Email must be 150 characters or fewer.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Subject must be 200 characters or fewer.")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Please write a message.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
    public string Message { get; set; } = string.Empty;
}