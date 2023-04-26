using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Models.Identity
{
    public class LoginInputModel
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}