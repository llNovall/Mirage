using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class CommentEditModel
    {
        [Required]
        public Guid CommentId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [StringLength(maximumLength: 500, MinimumLength = 5)]
        public string BodyContent { get; set; } = null!;
    }
}