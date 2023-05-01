using Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment : BaseEntity, IAuthorable
    {
        [StringLength(maximumLength: 500, MinimumLength = 5)]
        public string BodyContent { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime PostedOn { get; set; } = DateTime.Now;

        [Required]
        public Guid AuthorId { get; set; }

        public BlogPost BlogPost { get; set; } = null!;

        public IList<Comment> Replies { get; set; } = new List<Comment>();
    }
}