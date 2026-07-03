using Common.GridResults;

namespace Application.Chats.Dto
{
    public class ChatByIdRequestDto : GlobalGrid
    {
        public long RoomId { get; set; }
    }
}
