namespace Services.Services.CommentServices.DTOs
{
    public class GetCommentDTO
    {
        public long Id { get; internal set; }
        public string Text { get; internal set; }
        public int LikeCount { get; internal set; }
        public int DislikeCount { get; internal set; }
        public GetCommentUserInfoDTO UserInfo { get; internal set; }
        public IEnumerable<GetCommentDTO> Replies { get; internal set; }
        public bool IsApproved { get; internal set; }
        public bool IsReactedByLoginUser { get; internal set; }
        public bool IsMe { get; internal set; }
        public bool IsLikedByMe { get; internal set; }
    }

    public class GetCommentUserInfoDTO
    {
        public long Id { get; internal set; }
        public string FullName { get; internal set; }
    }
}
