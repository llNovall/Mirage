using Domain.Entities;

namespace Domain.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>, IFind<BlogPost>
    {
        Task<Dictionary<int, Dictionary<int, int>>> GetDictionaryOfPostedDateAsync();

        Task<int> GetBlogPostCountAsync();

        Task<List<BlogPost>> GetBlogPostsAsync(int pageNum, int numOfBlogPostsPerPage);
    }
}