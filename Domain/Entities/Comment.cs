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
        public DateTime PostedOn { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        public Post Post { get; set; } = null!;
    }
}