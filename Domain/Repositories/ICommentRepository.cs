using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommentRepository : IRepository<Comment>, IFind<Comment>
    {
        Task<IList<Comment>> GetAllAsync(Guid blogId);
    }
}