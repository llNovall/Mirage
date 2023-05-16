using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Contact
{
    public class ContactInputModel
    {
        [Required]
        [Display(Name = "Name")]
        [MinLength(1)]
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;

        [Required]
        [DataType(dataType: DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; } = null!;

        [Required]
        [Display(Name = "Subject")]
        [MinLength(5)]
        [DataType(DataType.Text)]
        public string Subject { get; set; } = null!;

        [Required]
        [Display(Name = "Message")]
        [MinLength(5)]
        [DataType(DataType.Text)]
        public string Message { get; set; } = null !;
    }
}
