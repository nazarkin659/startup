using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GasDuddy
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set;}

        public bool isLoggedIn { get; set; }

        public CookieCollection Cookies { get; set; }

        public string Response { get; set; }
    }
}
