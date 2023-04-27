using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class BlogCreateModel
    {
        [StringLength(maximumLength: 100, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [Display(Name = "Content")]
        [StringLength(maximumLength: 4000, MinimumLength = 100)]
        public string BodyContent { get; set; } = null!;

        [Display(Name = "Select Tag / Tags from below")]
        public List<TagCheckItem> TagsList { get; set; } = new();
    }

    public class TagCheckItem
    {
        public string TagId { get; set; } = null!;
        public string TagName { get; set; } = null!;
        public bool IsChecked { get; set; }
    }
}