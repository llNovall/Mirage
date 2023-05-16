using Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Comment : BaseEntity, IAuthorable
    {
        [StringLength(maximumLength: 500, MinimumLength = 5)]
        public string BodyContent { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime PostedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string AuthorId { get; set; } = null!;

        public string? BlogId { get; set; }

        public string? ParentCommentId { get; set; }

        //Navigation

        [ForeignKey(nameof(AuthorId))]
        public virtual Author Author { get; set; } = null!;

        [ForeignKey(nameof(BlogId))]
        public virtual BlogPost? BlogPost { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public virtual Comment? ParentComment { get; set; }

        [InverseProperty(nameof(ParentComment))]
        public IList<Comment> Replies { get; set; } = new List<Comment>();
    }
}