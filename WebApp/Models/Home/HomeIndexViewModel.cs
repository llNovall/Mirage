using Domain.Entities;

namespace WebApp.Models.Home
{
    public class HomeIndexViewModel
    {
        public IList<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

        public Dictionary<int, Dictionary<int, int>> DictPostedOn { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int NumOfPages { get; set; } = 1;
    }
}