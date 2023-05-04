using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Interfaces
{
    public interface IAuthorable
    {
        string AuthorId { get; set; }
        Author Author { get; set; }
    }
}