using Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Tag : BaseEntity, IAuthorable
    {
        [Required]
        public string AuthorId { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public virtual Author Author { get; set; } = null!;

        [StringLength(maximumLength: 20, MinimumLength = 1)]
        public string TagName { get; set; } = null!;

        [StringLength(maximumLength: 200)]
        public string TagDescription { get; set; } = null!;

        public IList<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}