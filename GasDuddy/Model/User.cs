using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasDuddy
{
    [Serializable]
    public class User
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool isLoggedIn { get; set; }

        public string CookiesHolder { get; set; }

        [NotMapped]
        public CookieCollection Cookies
        {
            get
            {
                return HelperFunctions.JSONHelper.FromJSON<CookieCollection>(CookiesHolder, new List<Type> { typeof(System.Net.Cookie) });
            }
            set
            {
                this.CookiesHolder = HelperFunctions.JSONHelper.ToJSON(value, typeof(CookieCollection), new List<Type> { typeof(System.Net.Cookie) });
            }
        }

        [NonSerialized]
        [NotMapped]
        public string Response;

        public User()
        {
            this.Response = string.Empty;
            this.CookiesHolder = string.Empty;
        }
    }
}
