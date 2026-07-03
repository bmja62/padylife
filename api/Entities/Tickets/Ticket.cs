using Entities.Common;
using Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Entities.Tickets
{

    public class Ticket : BaseEntity<long>
    {
        public string Title { get; private set; }
        public long UserId { get; private set; }
        public User User { get; set; }
        public bool Closed { get; private set; }
        public TicketType TicketType { get; set; }
        public TicketStatus Status { get; private set; }
        public SupportType SupportType { get; private set; }
        public ICollection<TicketDetail> TicketDetails { get; private set; } = new List<TicketDetail>();


        #region Factory Methodes
        //factory methodes
        public static Ticket Create(string title, long userId, TicketStatus status, TicketType ticketType, SupportType support) => new()
        {
            Title = title,
            UserId = userId,
            Status = status,
            TicketType = ticketType,
            SupportType = support

        };


        public void AddUserResponse(string content)
        {
            TicketDetails.Add(new TicketDetail
            {
                Content = content,
                ResponseType = TicketDetailResponseType.UserResponse,
            });
            Status = TicketStatus.WaitingForSupport;
            UpdatedAt = DateTime.Now;
        }



        public void AddSystemResponse(string content, TicketStatus status)
        {
            TicketDetails.Add(new TicketDetail
            {
                Content = content,
                ResponseType = TicketDetailResponseType.SystemResponse,
            });
            Status = status;
            UpdatedAt = DateTime.Now;
        }



        public void AddSupportResponse(string content, long supportUserId)
        {
            TicketDetails.Add(new TicketDetail
            {
                Content = content,
                ResponseType = TicketDetailResponseType.SupportResponse,
                SupportUserId = supportUserId,
            });
            Status = TicketStatus.WaitingForUser;
            UpdatedAt = DateTime.Now;
        }

        public void Close()
        {
            Closed = true;
            Status = TicketStatus.Closed;
        }

        #endregion
    }
    public enum SupportType
    {
        [Display(Name = "پشتیبانی")]
        Support = 1
    }


}
