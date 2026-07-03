// Ignore Spelling: Instagram Birthdate

using Entities.Addresses.ECommerce.Entities;
using Entities.Baskets;
using Entities.Blogs;
using Entities.Calendar;
using Entities.Challange;
using Entities.Chats;
using Entities.Comments;
using Entities.Common;
using Entities.Excersies;
using Entities.Hubs;
using Entities.Medals;
using Entities.Notifications;
using Entities.Orders;
using Entities.Plans;
using Entities.Products;
using Entities.Rates;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users;

public class User : IdentityUser<long>, IEntity<long>
{
    public User()
    {
        IsActive = true;
    }

    public User(string userName, string phoneNumber, string email) : base(userName)
    {
        PhoneNumber = phoneNumber;
        Email = email;
    }


    //Props


    /// <summary>
    /// نام و نام خانوادگی
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime CreateAt { get; set; } = DateTime.Now;


    /// <summary>
    /// سن
    /// </summary>
    public int Age { get; set; }


    /// <summary>
    /// جنسیت
    /// </summary>
    public GenderType Gender { get; set; }


    /// <summary>
    /// وضعیت فعالیت
    /// </summary>
    public bool IsActive { get; set; }


    /// <summary>
    /// آخرین ورود
    /// </summary>
    public DateTimeOffset? LastLoginDate { get; set; }



    /// <summary>
    /// تاریخ تولد
    /// </summary>
    public DateTime? Birthdate { get; set; }


    /// <summary>
    /// قد
    /// </summary>
    public int? Hight { get; set; }


    /// <summary>
    /// وزن
    /// </summary>
    public int? Wight { get; set; }


    /// <summary>
    /// وضعیت تاهل
    /// </summary>
    public MaritalStatus MaritalStatus { get; set; }


    /// <summary>
    /// اینستاگرام
    /// </summary>
    public string InstagramId { get; set; }


    /// <summary>
    /// شغل فرد
    /// </summary>
    public string JobTitle { get; set; }


    /// <summary>
    /// نمایه تصویری فرد
    /// </summary>
    public string ProfileImage { get; set; }

    //Navigations 


    /// <summary>
    /// نقش های کاربر
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; }


    /// <summary>
    /// رابطه بیماری های کاربر
    /// </summary>
    public List<UserDisease> UserDiseases { get; set; }
    /// <summary>
    /// یادآور
    /// </summary>
    public List<CalendarEvent> CalendarEvents { get; set; }


    /// <summary>
    /// پلن های ساخته شده توسط فرد
    /// </summary>
    public ICollection<Plan> Plans { get; set; }


    /// <summary>
    /// پلن هایی که فرد شرکت کرده
    /// </summary>
    public ICollection<UserPlan> UserPlans { get; set; }


    /// <summary>
    /// محصولات ساخته شده توسط فرد
    /// </summary>
    public ICollection<Product> Products { get; set; }


    /// <summary>
    /// تمرینات فرد
    /// </summary>
    public ICollection<UserExercise> UserExercises { get; set; }


    /// <summary>
    /// پیام های فرد
    /// </summary>
    public ICollection<Notification> NotificationSenders { get; set; }


    /// <summary>
    /// پیام های ارسالی به افراد
    /// </summary>
    public ICollection<NotificationReceiver> NotificationReceivers { get; set; }


    /// <summary>
    /// کانکشن نوتیفیکیشن
    /// </summary>
    public ICollection<NotifyConnection> NotifyConnections { get; set; }


    /// <summary>
    /// سفارشات
    /// </summary>
    public ICollection<Order> Orders { get; set; }


    /// <summary>
    /// آدرس ها
    /// </summary>
    public ICollection<Address> Addresses { get; set; }


    /// <summary>
    /// سبد خرید
    /// </summary>
    public ICollection<Basket> Baskets { get; set; }


    /// <summary>
    /// کامنت های فرد
    /// </summary>
    public ICollection<Comment> Comments { get; set; }


    /// <summary>
    /// ری اکشن های فرد به کامنت
    /// </summary>
    public ICollection<CommentReaction> CommentReactions { get; set; }


    /// <summary>
    /// ستاره های فرد
    /// </summary>
    public ICollection<Rate> Rates { get; set; }


    /// <summary>
    /// پرداخت ها
    /// </summary>
    public ICollection<Entities.Payments.Payment> Payments { get; set; }


    /// <summary>
    /// لاگ چالش های فرد
    /// </summary>
    public ICollection<ChallangeLog> ChallangeLogs { get; set; }


    /// <summary>
    /// همراهان پلن فرد
    /// </summary>
    public ICollection<UserPlanCompanion> Companions { get; set; }


    /// <summary>
    /// چت ها
    /// </summary>
    public ICollection<ChatRoomParticipant> ChatRoomParticipants { get; set; }


    /// <summary>
    /// امتیازات فرد
    /// </summary>
    public UserPoints UserPoints { get; set; }


    /// <summary>
    /// لیست بلاگ های فرد
    /// </summary>
    public ICollection<Blog> Blogs { get; set; }


    /// <summary>
    /// لیست مدال های فرد
    /// </summary>
    public ICollection<UserMedal> UserMedals { get; set; }

    //Factory Methods
    public static User RegisterUser(string userName, string phoneNumber, string email) => new(userName, phoneNumber, email);

    public void AddExcersie(UserExercise userExcersie)
    {
        UserExercises.Add(userExcersie);
    }


    //Methods
    public void SetAge(int age) => Age = age;

    public void SetBirthdate(DateTime? birthdate) => Birthdate = birthdate;

    public void SetFullName(string fullName) => FullName = fullName;

    public void SetGender(GenderType gender) => Gender = gender;

    public void SetHight(int? hight) => Hight = hight;

    public void SetInstagramId(string instagramId) => InstagramId = instagramId;

    public void SetIsActive(bool isActive) => IsActive = isActive;

    public void SetJobTitle(string jobTitle) => JobTitle = jobTitle;

    public void SetMaritalStatus(MaritalStatus maritalStatus) => MaritalStatus = maritalStatus;

    public void SetProfileImage(string profileImage) => ProfileImage = profileImage;

    public void SetWight(int? wight) => Wight = wight;
}
