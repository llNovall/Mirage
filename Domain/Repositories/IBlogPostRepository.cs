using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<BlogPost?> FindByIdIncludeNavigationAsync(Guid id);

        Task<BlogPost?> FindByIdIncludeNavigationAsync(string id);

        Task<IList<BlogPost>> FindAllBlogPostsIncludingNavigationAsync();
    }
}