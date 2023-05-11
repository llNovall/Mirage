using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
    public class BlogPostRepository : Repository<BlogPost, BlogPostRepository>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context, ILogger<BlogPostRepository> logger) : base(context, logger)
        {
        }

        public async Task<BlogPost?> FindAsync(Expression<Func<BlogPost, bool>> predicate) => await _context.BlogPosts.Where(predicate).FirstOrDefaultAsync();

        public async Task<BlogPost?> FindByIdAsync(Guid id) => await _context.BlogPosts.Where(c => c.Id == id.ToString()).FirstOrDefaultAsync();

        public async Task<BlogPost?> FindByIdAsync(string id) => await _context.BlogPosts.Where(c => c.Id == id).Include(c => c.Comments).Include(c => c.Tags).Include(c => c.Author).FirstOrDefaultAsync();

        public async Task<IList<BlogPost>> GetAllAsync() => await _context.BlogPosts.Include(c => c.Tags).Include(c => c.Comments).Include(c => c.Author).ToListAsync();

        public async Task<int> GetBlogPostCountAsync() => await _context.BlogPosts.CountAsync();

        public async Task<List<BlogPost>> GetBlogPostsAsync(int pageNum, int numOfBlogPostsPerPage)
        {
            if (pageNum < 1) pageNum = 1;
            if (numOfBlogPostsPerPage < 1) numOfBlogPostsPerPage = 1;

            var posts = await _context.BlogPosts.OrderByDescending(c => c.PostedOn).Include(c => c.Author).Include(c => c.Tags).ToListAsync();
            int startIndex = (pageNum - 1) * numOfBlogPostsPerPage;

            int endIndex = ((pageNum - 1) * numOfBlogPostsPerPage) + numOfBlogPostsPerPage;
            posts = posts.Take(new Range(startIndex, endIndex)).ToList();

            return posts;
        }

        public async Task<Dictionary<int, Dictionary<int, int>>> GetDictionaryOfPostedDateAsync()
        {
            List<BlogPost> posts = await _context.BlogPosts.OrderByDescending(c => c.PostedOn).ToListAsync();

            Dictionary<int, Dictionary<int, int>> dictPostedDate = new();

            await Task.Run(() =>
            {
                for (int i = 0; i < posts.Count; i++)
                {
                    DateTime postedOn = posts[i].PostedOn;

                    if (!dictPostedDate.ContainsKey(postedOn.Year))
                        dictPostedDate.Add(postedOn.Year, new Dictionary<int, int>());

                    if (!dictPostedDate[postedOn.Year].ContainsKey(postedOn.Month))
                        dictPostedDate[postedOn.Year].Add(postedOn.Month, 0);

                    dictPostedDate[postedOn.Year][postedOn.Month]++;
                }
            });

            return dictPostedDate;
        }
    }
}