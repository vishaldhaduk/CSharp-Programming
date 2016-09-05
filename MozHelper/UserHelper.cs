using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers
{
    public static class UserHelper
    {
        public static void DeleteUserFile(this string user)
        {
            user.Delete();
        }
        private static void Delete(this string user)
        {
            if (File.Exists(user))
            {
                File.Delete(user);
            }
        }
    }
}
