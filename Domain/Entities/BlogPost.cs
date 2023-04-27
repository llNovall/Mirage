using Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BlogPost : BaseEntity, IAuthorable
    {
        [StringLength(maximumLength: 100, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [StringLength(maximumLength: 4000, MinimumLength = 100)]
        public string BodyContent { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime PostedOn { get; set; } = DateTime.Now;

        [Required]
        public Guid AuthorId { get; set; }

        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public IList<Tag> Tags { get; set; } = new List<Tag>();
    }
}