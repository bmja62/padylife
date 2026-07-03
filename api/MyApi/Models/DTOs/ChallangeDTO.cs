using Entities.Challange;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class ChallangeDTO : BaseDto<ChallangeDTO, Challange, long>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int ParticipantCount { get; internal set; }
        public ChallengeType Type { get; set; }
        public bool HasParticipantByMe { get; set; }
        public long? CreatedByUserId { get; set; }


    }

}
