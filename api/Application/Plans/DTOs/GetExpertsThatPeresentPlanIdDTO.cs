using Entities.Users;

namespace Application.Plans.DTOs
{
    public class GetExpertsThatPeresentPlanIdDTO
    {
        public GetExpertsThatPeresentPlanIdDTO(long id, string fullName, string profileImage)
        {
            Id = id;
            FullName = fullName;
            ProfileImage = profileImage;
        }

        public long Id { get; }
        public string FullName { get; }
        public string ProfileImage { get; }

        internal static GetExpertsThatPeresentPlanIdDTO Create(Expert t) => new(t.Id, t.FullName ,t.ProfileImage);
    }
}
