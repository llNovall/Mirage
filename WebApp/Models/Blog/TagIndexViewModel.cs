using Domain.DataTransferClasses;
using Domain.Entities;

namespace WebApp.Models.Blog
{
    public class TagIndexViewModel
    {
        public string TagId { get; set; }

        public string TagName { get; set; }
        public IList<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

        public Dictionary<int, Dictionary<int, int>> DictPostedOn { get; set; } = new();

        public List<TagBlogPostCountData> TagBlogPostCountDatas { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int NumOfPages { get; set; } = 1;

        public TagIndexViewModel(
            string tagId,
            string tagName,
            IList<BlogPost> blogPosts,
            Dictionary<int, Dictionary<int, int>> dictPostedOn,
            List<TagBlogPostCountData> tagBlogPostCountDatas,
            int currentPage,
            int numOfPages)
        {
            TagId = tagId;
            TagName = tagName;
            BlogPosts = blogPosts;
            DictPostedOn = dictPostedOn;
            TagBlogPostCountDatas = tagBlogPostCountDatas;
            CurrentPage = currentPage;
            NumOfPages = numOfPages;
        }
    }
}