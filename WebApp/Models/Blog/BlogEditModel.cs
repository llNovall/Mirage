using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class BlogEditModel
    {
        [Required]
        public string BlogId { get; set; } = null!;

        [StringLength(maximumLength: 100, MinimumLength = 10, ErrorMessage = "Title must have a length between 10 - 100 characters.")]
        public string Title { get; set; } = null!;

        [Display(Name = "Content")]
        [StringLength(maximumLength: 4000, MinimumLength = 100, ErrorMessage = "Content must have a length between 100 - 4000 characters, including the formats.")]
        public string BodyContent { get; set; } = null!;

        [Display(Name = "Select Tag / Tags from below")]
        public List<TagCheckItem> TagsList { get; set; } = new();
    }
}