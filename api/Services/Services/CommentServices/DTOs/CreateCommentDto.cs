using Entities.Comments;
using Entities.Common;

namespace Services.Services.CommentServices.DTOs
{
    public class CreateCommentDTO
    {
        public long EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public string Text { get; set; }
        public long? ParentCommentId { get; set; }
    }

    public class CreateRateDTO
    {
        public long EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public int RatingValue { get; set; }
    }

    public class ReactToCommentDTO
    {
        public ReactionType ReactionType { get; set; }
    }
}
