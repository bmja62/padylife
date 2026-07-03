using Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public UserDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void InitializeData()
        {
            if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new Role { Name = "Admin", Description = "Admin role" }).GetAwaiter().GetResult();
            }

            var adminUser = userManager.Users.SingleOrDefault(p => p.UserName == "admin");

            if (adminUser == null)
            {
                var user = new User
                {
                    Age = 25,
                    FullName = "admin",
                    Gender = GenderType.Male,
                    UserName = "admin",
                    Email = "admin@site.com",

                };
                var craetedUserResualt = userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();

                userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
            else
            {
                var token = userManager.GeneratePasswordResetTokenAsync(adminUser).GetAwaiter().GetResult();
                userManager.ResetPasswordAsync(adminUser, token, "Admin@200236").GetAwaiter().GetResult();
            }
        }
    }
}