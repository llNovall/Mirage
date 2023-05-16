using Domain.Entities;

namespace Domain.Repositories
{
    public interface IAuthorRepository : IRepository<Author>, IFind<Author>
    {
    }
}