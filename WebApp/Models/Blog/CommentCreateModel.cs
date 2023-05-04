using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class CommentCreateModel
    {
        [Required]
        [StringLength(maximumLength: 500, MinimumLength = 5)]
        public string BodyContent { get; set; } = null!;

        public string? BlogId { get; set; }
    }
}