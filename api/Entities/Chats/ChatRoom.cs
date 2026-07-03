using Entities.Common;
using Entities.Users;

namespace Entities.Chats
{
    public class ChatRoom : BaseEntity<long>
    {
        private ChatRoom() { }

        public ChatRoom(string name, bool isGroup, long? createdBy = null)
        {
            Name = name;
            IsGroup = isGroup;
            CreatedBy = createdBy;
            Participants = new List<ChatRoomParticipant>();
            Messages = new List<ChatMessage>();
        }
        /// <summary>
        /// عنوان چت (برای گروه‌ها)
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// آیا چت گروهی است؟
        /// </summary>
        public bool IsGroup { get; private set; }
        /// <summary>
        /// ارتباط با پلن (در صورت نیاز)
        /// </summary>
        public long? UserPlanId { get; private set; }
        public UserPlan? UserPlan { get; private set; }

        public long? CreatedBy { get; private set; }

        /// <summary>
        /// اعضا
        /// </summary>
        public ICollection<ChatRoomParticipant> Participants { get; private set; } = new List<ChatRoomParticipant>();
        /// <summary>
        /// پیام‌ها
        /// </summary>
        public ICollection<ChatMessage> Messages { get; private set; }
        public static ChatRoom CreatePrivateChat(long user1Id, long user2Id)
        {
            var room = new ChatRoom
            {
                IsGroup = false,
            };
            room.AddParticipant(user1Id);
            room.AddParticipant(user2Id);


            return room;
        }

        public static ChatRoom CreateGroupChat(string name, List<long> participantIds, long? userPlanId = null)
        {
            var room = new ChatRoom
            {
                Name = name,
                IsGroup = true,
                UserPlanId = userPlanId
            };

            foreach (var id in participantIds)
            {
                room.Participants.Add(ChatRoomParticipant.Create(id));
            }

            return room;
        }
        public void AddParticipant(long userId)
        {
            if (!Participants.Any(p => p.UserId == userId))
                Participants.Add(new ChatRoomParticipant(this.Id, userId));
        }
    }

}
