using Diva2.Core.Main.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Diva2.Core
{
    public class SHA1PasswordHasher : IPasswordHasher<User8>
    {
        public string HashPassword(User8 user, string password)
        {
            return ReversePassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User8 user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == ReversePassword(providedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        private string ReversePassword(string value)
        {
            using (var sha1 = new SHA1Managed())
            {
                return System.BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLowerInvariant();
            }
        }
    }

}
