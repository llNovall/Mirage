using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using EFDataAccess.Contexts;
using EFDataAccess.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Services
{
    public class DBService : IDBService
    {
        private readonly IBlogPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IAuthorRepository _authorRepository;

        public DBService(ApplicationDbContext context, ILogger<AuthorRepository> authorLogger, ILogger<BlogPostRepository> blogPostLogger, ILogger<TagRepository> tagLogger, ILogger<CommentRepository> commentLogger)
        {
            _postRepository = new BlogPostRepository(context, blogPostLogger);
            _commentRepository = new CommentRepository(context, commentLogger);
            _tagRepository = new TagRepository(context, tagLogger);
            _authorRepository = new AuthorRepository(context, authorLogger);
        }

        public IBlogPostRepository PostRepository
        { get { return _postRepository; } }

        public ICommentRepository CommentRepository
        { get { return _commentRepository; } }

        public ITagRepository TagRepository
        { get { return _tagRepository; } }

        public IAuthorRepository AuthorRepository
        { get { return _authorRepository; } }
    }
}