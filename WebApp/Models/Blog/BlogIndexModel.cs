using Domain.Entities;

namespace WebApp.Models.Blog
{
    public class BlogIndexModel
    {
        public BlogPost? BlogPost { get; set; }
        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public CommentCreateModel? CommentCreate { get; set; }

        public CommentEditModel? CommentEdit { get; set; }

        public CommentDeleteModel? CommentDelete { get; set; }

        public CommentReplyModel? CommentReply { get; set; }
    }
}