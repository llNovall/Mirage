using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITagRepository : IRepository<Tag>, IFind<Tag>
    {
        Task<IList<BlogPost>> GetPostsByTagAsync(string tagName);

        Task<int> GetTotalPostsByTagAsync(string tagName);
    }
}