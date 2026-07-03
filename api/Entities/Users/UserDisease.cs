// Ignore Spelling: Instagram Birthdate

using Entities.Common;

namespace Entities.Users;

public class UserDisease : BaseEntity<long>
{
    //FKs
    public long UserId { get; set; }

    //Props
    public string Name { get; set; }

    //Navigations
    public User User { get; set; }
}
