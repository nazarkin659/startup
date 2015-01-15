using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;

namespace GasBuddy.Model
{
    [Table("WebSite")]
    public partial class WebSite
    {
        [Required]
        [Key]
        [ForeignKey("User")]
        public int WebID { get; set; }

        public virtual User User { get; set; }

        public bool isLoggedIn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CookiesHolder) && !string.IsNullOrWhiteSpace(CheckLoginURL))
                {
                    try
                    {
                        Response = WebSpider.SpiderUse.GetResponse(CheckLoginURL, false, Cookies);
                        CsQuery.CQ cq = Response;
                        if (!cq.IsNullOrEmpty() &&
                            (!cq[string.Format(".login .profile:contains('{0}')", User.UserName.TrimEnd())].IsNullOrEmpty() || !cq[string.Format("#ctl00_TB_lblLogInStatus .MPLink:contains('{0}')", User.UserName.TrimEnd())].IsNullOrEmpty())
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
        public string CookiesHolder { get; set; }
        public string CheckLoginURL { get; set; }

        public string URL { get; set; }

        public string Response { get; set; }

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
    }
}
