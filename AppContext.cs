using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManagerGavr
{
    class AppContext
    {
        public static string CurrentUser { get; private set; }

        public static void Login(string username)
        {
            CurrentUser = username;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUser);
    }
}
