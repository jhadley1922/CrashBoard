using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models.ViewModels
{
    public static class LoginCookieModel
    {
        public static string UserId { get; set; }
        public static bool Authorized = false;
    }
}
