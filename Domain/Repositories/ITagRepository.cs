using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ITagRepository : IRepository<Tag>, IFind<Tag>
    {
        Task<IList<BlogPost>> GetPostsByTagAsync(string tagName);

        Task<int> GetTotalPostsByTagAsync(string tagName);
    }
}