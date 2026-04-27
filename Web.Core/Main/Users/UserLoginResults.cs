using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public enum UserLoginResults
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,

        /// <summary>
        /// User does not exist (email or nick)
        /// </summary>
        UserNotExist = 2,

        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,

        NotActive = 4,

        Deleted = 5,
    }
}
