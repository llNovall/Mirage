using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DataTransferClasses
{
    public class TagBlogPostCountData
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public int BlogPostCount { get; set; }

        public TagBlogPostCountData(string tagId, string tagName, int blogPostCount)
        {
            TagId = tagId;
            TagName = tagName;
            BlogPostCount = blogPostCount;
        }
    }
}