using Common.GridResults;

namespace Application.Chats.Dto
{
    public class GetAllChatRequestDto : GlobalGrid
    {

        public string Search { get; set; }
    }
}
