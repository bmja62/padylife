using Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users
{
    public class Role : IdentityRole<long>, IEntity
    {
        public string Description { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
