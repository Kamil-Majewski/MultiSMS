using Microsoft.AspNetCore.Identity;

namespace MultiSMS.Interface.Seeding
{
    internal class AssignRoles
    {
        internal static IdentityUserRole<int>[] GrantRoles()
        {
            return
            [
                new IdentityUserRole<int>
                {
                    RoleId = 1,
                    UserId = 1
                },
                new IdentityUserRole<int>
                {
                    RoleId = 2,
                    UserId = 2
                },
                new IdentityUserRole<int>
                {
                    RoleId = 3,
                    UserId = 3
                },
            ];
        }
    }
}
