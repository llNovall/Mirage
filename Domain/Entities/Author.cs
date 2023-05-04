using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Author : BaseEntity
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        public IList<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();
    }
}