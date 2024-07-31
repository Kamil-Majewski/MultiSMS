﻿using Microsoft.AspNetCore.Identity;
using MultiSMS.Interface.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MultiSMS.Interface.Seeding
{
    internal static class SeedUsers
    {
        internal static IEnumerable<User> GetUserSeed()
        {
            IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

            var users = new User[]
            {
                new User
                {
                    Id = 1,
                    Name = "Giga",
                    Surname = "Admin",
                    Email = "gigaadmin@gmail.com",
                    UserName = "gigaadmin@gmail.com",
                    NormalizedUserName = "GIGAADMIN@GMAIL.COM",
                    NormalizedEmail = "GIGAADMIN@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = "",
                    SecurityStamp = GetSecurityStamp(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                 new User
                {
                    Id = 2,
                    Name = "Normal",
                    Surname = "Admin",
                    Email = "admin@gmailcom",
                    UserName = "admin@gmailcom",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = GetSecurityStamp(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new User
                {
                    Id = 3,
                    Name = "Casual",
                    Surname = "User",
                    Email = "user@gmail.com",
                    UserName = "user@gmail.com",
                    NormalizedUserName = "USER@GMAIL.COM",
                    NormalizedEmail = "USER@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = GetSecurityStamp(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                }
            };

            users[0].PasswordHash = _passwordHasher.HashPassword(users[0], "admin$");
            users[1].PasswordHash = _passwordHasher.HashPassword(users[1], "admin");
            users[2].PasswordHash = _passwordHasher.HashPassword(users[2], "user");
            return users;
        }

        private const string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        private static string GetSecurityStamp()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return ToBase32(bytes);
        }
        private static string ToBase32(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            StringBuilder sb = new StringBuilder();
            for (int offset = 0; offset < input.Length;)
            {
                byte a, b, c, d, e, f, g, h;
                int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                sb.Append(numCharsToOutput >= 1 ? _base32Chars[a] : '=');
                sb.Append(numCharsToOutput >= 2 ? _base32Chars[b] : '=');
                sb.Append(numCharsToOutput >= 3 ? _base32Chars[c] : '=');
                sb.Append(numCharsToOutput >= 4 ? _base32Chars[d] : '=');
                sb.Append(numCharsToOutput >= 5 ? _base32Chars[e] : '=');
                sb.Append(numCharsToOutput >= 6 ? _base32Chars[f] : '=');
                sb.Append(numCharsToOutput >= 7 ? _base32Chars[g] : '=');
                sb.Append(numCharsToOutput >= 8 ? _base32Chars[h] : '=');
            }
            return sb.ToString();
        }
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            uint b1, b2, b3, b4, b5;

            int retVal;
            switch (input.Length - offset)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }

            b1 = offset < input.Length ? input[offset++] : 0U;
            b2 = offset < input.Length ? input[offset++] : 0U;
            b3 = offset < input.Length ? input[offset++] : 0U;
            b4 = offset < input.Length ? input[offset++] : 0U;
            b5 = offset < input.Length ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)((b1 & 0x07) << 2 | b2 >> 6);
            c = (byte)(b2 >> 1 & 0x1f);
            d = (byte)((b2 & 0x01) << 4 | b3 >> 4);
            e = (byte)((b3 & 0x0f) << 1 | b4 >> 7);
            f = (byte)(b4 >> 2 & 0x1f);
            g = (byte)((b4 & 0x3) << 3 | b5 >> 5);
            h = (byte)(b5 & 0x1f);

            return retVal;
        }
    }
}
