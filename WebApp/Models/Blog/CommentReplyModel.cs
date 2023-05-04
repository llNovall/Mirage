using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Blog
{
    public class CommentReplyModel
    {
        /// <summary>
        /// The id of the comment that is replied to.
        /// </summary>
        public Guid ReplyCommentId { get; set; }

        [StringLength(maximumLength: 500, MinimumLength = 5)]
        public string BodyContent { get; set; } = null!;
    }
}