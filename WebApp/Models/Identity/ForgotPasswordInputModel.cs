using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Identity
{
    public class ForgotPasswordInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}