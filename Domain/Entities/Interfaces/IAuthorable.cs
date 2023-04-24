using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Interfaces
{
    public interface IAuthorable
    {
        Guid AuthorId { get; set; }
    }
}