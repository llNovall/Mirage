using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Admin
{
    public class AdminEditTagModel
    {
        [Display(Name = "Tag ID")]
        public string? TagId { get; set; } = null!;

        [Display(Name = "Tag Name")]
        [StringLength(maximumLength: 20, MinimumLength = 1)]
        public string? TagName { get; set; } = null!;

        [Display(Name = "Tag Description")]
        [StringLength(maximumLength: 200)]
        public string? TagDescription { get; set; } = null!;

        public AdminEditTagModel()
        {
        }

        public AdminEditTagModel(Tag tag)
        {
            TagId = tag.Id;
            TagName = tag.TagName;
            TagDescription = tag.TagDescription;
        }
    }
}