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

        public DBService(ApplicationDbContext context)
        {
            _postRepository = new BlogPostRepository(context);
            _commentRepository = new CommentRepository(context);
            _tagRepository = new TagRepository(context);
        }

        public IBlogPostRepository PostRepository
        { get { return _postRepository; } }

        public ICommentRepository CommentRepository
        { get { return _commentRepository; } }

        public ITagRepository TagRepository
        { get { return _tagRepository; } }
    }
}