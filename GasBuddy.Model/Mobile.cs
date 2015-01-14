using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using System.Net;

namespace GasBuddy.Model
{
    [Table("Mobile")]
    public partial class Mobile
    {
        [Required]
        [Key, ForeignKey("User")]
        public int MobileID { get; set; }

        public virtual User User { get; set; }

        public bool isLoggedIn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CookiesHolder) && !string.IsNullOrWhiteSpace(CheckLoginURL))
                {
                    try
                    {
                        CsQuery.CQ response = WebSpider.SpiderUse.GetResponse(CheckLoginURL, false, Cookies);
                        if (!response.IsNullOrEmpty() &&
                            (!response[string.Format(".login .profile:contains('{0}')", User.UserName.TrimEnd())].IsNullOrEmpty() || !response[string.Format("#ctl00_TB_lblLogInStatus .MPLink:contains('{0}')", User.UserName.TrimEnd())].IsNullOrEmpty())
                            )
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Gurock.SmartInspect.SiAuto.Main.LogException(e);
                    }
                }
                return false;
            }
        }

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

        public string CheckLoginURL { get; set; }
        public string CookiesHolder { get; set; }

        public string Response { get; set; }

        public Mobile() { }
    }
}
