using Domain.Repositories;

namespace Domain.Services
{
    public interface IDBService
    {
        IBlogPostRepository PostRepository { get; }
        ICommentRepository CommentRepository { get; }
        ITagRepository TagRepository { get; }
        IAuthorRepository AuthorRepository { get; }
    }
}