using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class CommentDeleteModel
    {
        [Required]
        public Guid CommentId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
    }
}