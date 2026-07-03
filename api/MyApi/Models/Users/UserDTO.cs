using Application.Questions.DTOs;
using Entities.Users;
using WebFramework.Api;

namespace PadyLife.Api.Models.Users
{
    /// <summary>
    /// مدل دریافت کاربر
    /// </summary>
    public class UserDTO : BaseDto<UserDTO, User, long>
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDTO()
        {

        }
        private UserDTO(
            long id,
            string fullName,
            bool isActive,
            string phoneNumber,
            GenderType gender,
            string email,
            bool emailConfirmed,
            RoleDTO[] roles,
            string userName,
            List<UserPlanDTO> userPlans,
            MaritalStatus maritalStatus,
            string jobTitle,
            int? wight,
            int? hight,
            DateTime? birthdate,
            string instagramId,
            List<string> addresses,
            int age,
            string profileImage,
            string iconUrl)
        {
            Id = id;
            FullName = fullName;
            IsActive = isActive;
            PhoneNumber = phoneNumber;
            Gender = gender;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Roles = roles;
            UserName = userName;
            UserPlans = userPlans;
            MaritalStatus = maritalStatus;
            JobTitle = jobTitle;
            Wight = wight;
            Hight = hight;
            Birthdate = birthdate;
            InstagramId = instagramId;
            Addresses = addresses;
            Age = age;
            ProfileImage = profileImage;
            IconUrl = iconUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RoleDTO[] Roles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal WalletCredit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShabaNo { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntroduceCode { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public GenderType? Gender { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public bool EmailConfirmed { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        public List<UserPlanDTO> UserPlans { get; internal set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string JobTitle { get; set; }
        public int? Wight { get; set; }
        public int? Hight { get; set; }
        public DateTime? Birthdate { get; set; }
        public string InstagramId { get; set; }
        public List<string> Addresses { get; set; }
        public UserSignUpPlanInfoDTO SignUpPlanInfo { get; set; }
        public int Age { get; internal set; }
        public string ProfileImage { get; set; }
        public string IconUrl { get; set; }
        public int Accompanied { get; internal set; }
        public int Supported { get; internal set; }
        public int TotalUniqueCompanions { get; internal set; }
        public List<CompanionSummaryDTO> CompanionSummary { get; internal set; }
        public List<CompanionDetailDTO> CompanionDetails { get; internal set; }
        public List<AccompaniedSummaryDTO> AccompaniedSummary { get; internal set; }
        public List<AccompaniedDetailDTO> AccompaniedDetails { get; internal set; }
        public int TotalUniquePlanOwners { get; internal set; }

        internal static UserDTO Create
            (
            long id,
            string fullName,
            bool isActive,
            string phoneNumber,
            GenderType gender,
            string email,
            bool emailConfirmed,
            RoleDTO[] roles,
            string userName,
            List<UserPlanDTO> userPlans,
            MaritalStatus maritalStatus,
            string jobTitle,
            int? wight,
            int? hight,
            DateTime? birthdate,
            string instagramId,
            List<string> addresses,
            int age,
            string profileImage,
            string iconUrl)
            => new(
                id,
                fullName,
                isActive,
                phoneNumber,
                gender,
                email,
                emailConfirmed,
                roles,
                userName,
                userPlans,
                maritalStatus,
                jobTitle,
                wight,
                hight,
                birthdate,
                instagramId,
                addresses,
                age,
                profileImage,
                iconUrl);
    }

    public class UserSignUpPlanInfoDTO
    {
        public UserSignUpPlanInfoDTO()
        {

        }
        public UserSignUpPlanInfoDTO(bool hasSignUpPlan, GetByIdQuestionDTO lastQuestion)
        {
            HasSignUpPlan = hasSignUpPlan;
            LastQuestion = lastQuestion;
        }

        public bool HasSignUpPlan { get; set; }
        public GetByIdQuestionDTO LastQuestion { get; set; }

        public static UserSignUpPlanInfoDTO Create(bool hasSignUpPlan, GetByIdQuestionDTO lastQuestion) => new(hasSignUpPlan, lastQuestion);
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoleDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public RoleDTO()
        {

        }

        public RoleDTO(string name)
        {
            Name = name;
        }

        private RoleDTO(string name, string description)
        {
            Name = name;
            Description = description;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        internal static RoleDTO Create(string name, string description) => new(name, description);
        internal static RoleDTO Create(string name) => new(name);
    }

    public class UserPlanDTO
    {
        public UserPlanDTO(long id, long planId, long userId)
        {
            Id = id;
            PlanId = planId;
            UserId = userId;
        }

        public long Id { get; }
        public long PlanId { get; }
        public long UserId { get; }

        internal static UserPlanDTO Create(long id, long planId, long userId)
        => new(id, planId, userId);
    }

    public class CompanionSummaryDTO
    {
        public long UserId { get; internal set; }
        public int Count { get; internal set; }
    }
    public class CompanionDetailDTO
    {
        public long UserPlanId { get; internal set; }
        public long PlanId { get; internal set; }
        public long PlanOwnerUserId { get; internal set; }
        public long CompanionUserId { get; internal set; }
        public DateTime CompanionAddedDate { get; internal set; }
    }
    public class AccompaniedSummaryDTO
    {
        public long PlanOwnerUserId { get; set; }
        public string PlanOwnerName { get; set; }
        public int Count { get; set; }
    }

    public class AccompaniedDetailDTO
    {
        public long UserPlanId { get; set; }
        public long PlanId { get; set; }
        public long PlanOwnerUserId { get; set; }
        public string PlanOwnerName { get; set; }
        public DateTime CompanionSince { get; set; }
        public UserPlanBasicDTO PlanInfo { get; set; }
    }

    public class UserPlanBasicDTO
    {
        public string PlanName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
