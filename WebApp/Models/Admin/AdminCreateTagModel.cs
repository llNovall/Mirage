using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Admin
{
    public class AdminCreateTagModel
    {
        [StringLength(maximumLength: 20, MinimumLength = 1)]
        public string TagName { get; set; } = null!;

        [StringLength(maximumLength: 200)]
        public string TagDescription { get; set; } = null!;
    }
}