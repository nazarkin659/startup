using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasBuddy.Model
{
    [Serializable]
    [Table("Users")]
    public class User
    {
        [Key]
        [Required]
        public int UserID { get; set; }
        [Column("UserName", TypeName = "char")]
        [MaxLength(24)]
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public bool isLoggedIn { get; set; }

        public string CookiesHolder { get; set; }

        [NotMapped]
        public CookieCollection Cookies
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CookiesHolder))
                    return HelperFunctions.JSONHelper.FromJSON<CookieCollection>(CookiesHolder, new List<Type> { typeof(System.Net.Cookie) });
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this.CookiesHolder = HelperFunctions.JSONHelper.ToJSON(value, typeof(CookieCollection), new List<Type> { typeof(System.Net.Cookie) });
                else
                    this.CookiesHolder = null;
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
