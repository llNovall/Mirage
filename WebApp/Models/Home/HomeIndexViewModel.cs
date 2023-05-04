using Domain.Entities;

namespace WebApp.Models.Home
{
    public class HomeIndexViewModel
    {
        public IList<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}