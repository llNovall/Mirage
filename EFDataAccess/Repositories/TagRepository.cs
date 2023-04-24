using Domain.Entities;
using Domain.Repositories;
using EFDataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IList<Post>> GetPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return new List<Post>();

            Tag? tag = await _context.Tags.Where(c => c.TagName == tagName).Include(c => c.Posts).FirstOrDefaultAsync();

            if (tag == null)
                return new List<Post>();

            return tag.Posts;
        }

        public async Task<int> GetTotalPostsByTagAsync(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return 0;

            Tag? tag = await _context.Tags.Where(c => c.TagName == tagName).Include(c => c.Posts).FirstOrDefaultAsync();

            if (tag == null)
                return 0;

            return tag.Posts.Count;
        }
    }
}