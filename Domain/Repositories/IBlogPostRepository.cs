using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>, IFind<BlogPost>
    {
        Task<Dictionary<int, Dictionary<int, int>>> GetDictionaryOfPostedDateAsync();

        Task<int> GetBlogPostCountAsync();

        Task<List<BlogPost>> GetBlogPostsAsync(int pageNum, int numOfBlogPostsPerPage);
    }
}