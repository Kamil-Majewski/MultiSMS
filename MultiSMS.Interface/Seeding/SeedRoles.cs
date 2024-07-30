using Microsoft.AspNetCore.Identity;

namespace MultiSMS.Interface.Seeding
{
    internal static class SeedRoles
    {
        internal static IEnumerable<IdentityRole<int>> Seed()
        {
            return new IdentityRole<int>[]
            {
                new IdentityRole<int>
                {
                    Name = "Owner",
                    NormalizedName = "OWNER",
                    Id = 1,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole<int>
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = 2,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole<int>
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = 3,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            };
        }
    }
}
