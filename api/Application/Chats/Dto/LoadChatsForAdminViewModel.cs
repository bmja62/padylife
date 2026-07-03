namespace Application.Chats.Dto
{
    public class LoadChatsForAdminViewModel
    {
        public ChatInfoAdminViewModel Chat { get; set; }
        public int? MessageCount { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}
