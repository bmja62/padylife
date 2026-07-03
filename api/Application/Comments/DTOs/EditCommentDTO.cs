using Entities.Comments;

namespace Application.Comments.DTOs
{
    public class EditCommentDTO
    {
        public long CommentId { get; set; }
        public string NewText { get; set; }
    }

    public class ReactionDTO
    {
        public ReactionType type { get; set; }
    }
}
