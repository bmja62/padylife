namespace Application.Chats.Dto
{
    public class ChatInfoAdminViewModel
    {

        public long ChatId { get; set; }
        public string ChatRoomName { get; set; }
        public string ChatRoomUserPlanTitle { get; set; }
        public List<ChatUsersInfoViewModel> UserFullNames { get; set; }
        public bool IsGroupChatRoom { get; set; }
    }
}
