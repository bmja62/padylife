namespace Application.Plans.DTOs
{
    public class GetPlanSubscribersDTO
    {
        public GetPlanSubscribersDTO(string userFullName, string profileImage)
        {
            UserFullName = userFullName;
            ImageUrl = profileImage;
        }

        public string UserFullName { get; set; }
        public string ImageUrl { get; set; }

        internal static GetPlanSubscribersDTO Create(string userFullName, string profileImage) =>
            new(userFullName, profileImage);
    }
}
