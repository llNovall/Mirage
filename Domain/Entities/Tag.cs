using Domain.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tag : BaseEntity, IAuthorable
    {
        [Required]
        public Guid AuthorId { get; set; }

        [StringLength(maximumLength: 20, MinimumLength = 1)]
        public string TagName { get; set; } = null!;

        [StringLength(maximumLength: 200)]
        public string? TagDescription { get; set; }

        public IList<Post> Posts { get; set; } = new List<Post>();
    }
}