using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;

namespace GasBuddy.Model.ComplexTypes
{
    [ComplexType]
    public abstract class LoginType
    {
        [Column("CookiesHolder")]
        public virtual string CookiesHolder { get; set; }
        [NotMapped]
        public virtual CookieCollection Cookies
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

        [Column("CheckLoginURL")]
        public virtual string CheckLoginURL { get; set; }

        [NonSerializedAttribute]
        [NotMapped]
        public string Response = "";

        public abstract bool isLoggedIn { get; }
    }

    [ComplexType]
    public class Mobile : LoginType
    {
        [NotMapped]
        public override bool isLoggedIn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CookiesHolder) && !string.IsNullOrWhiteSpace(CheckLoginURL))
                {
                    try
                    {
                        CsQuery.CQ response = WebSpider.SpiderUse.GetResponse(CheckLoginURL, false, Cookies);
                        if (!response.IsNullOrEmpty() &&
                            (!response[string.Format(".login .profile:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty() || !response[string.Format("#ctl00_TB_lblLogInStatus .MPLink:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty())
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

        public Mobile()
        { }

        public Mobile(string userName)
        {
            this.UserName = userName;
        }
        private string UserName;
    }
    [ComplexType]
    public class WebSite : LoginType
    {
        public override bool isLoggedIn
        {
            get { return false; }
        }

    //    [NotMapped]
    //    public override bool isLoggedIn
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrWhiteSpace(CookiesHolder) && !string.IsNullOrWhiteSpace(CheckLoginURL))
    //            {
    //                try
    //                {
    //                    CsQuery.CQ response = WebSpider.SpiderUse.GetResponse(CheckLoginURL, false, Cookies);
    //                    if (!response.IsNullOrEmpty() &&
    //                        (!response[string.Format(".login .profile:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty() || !response[string.Format("#ctl00_TB_lblLogInStatus .MPLink:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty())
    //                        )
    //                    {
    //                        return true;
    //                    }
    //                }
    //                catch (Exception e)
    //                {
    //                    Gurock.SmartInspect.SiAuto.Main.LogException(e);
    //                }
    //            }
    //            return false;
    //        }
    //    }

    //    public WebSite()
    //    { }

    //    public WebSite(string userName)
    //    {
    //        this.UserName = userName;
    //    }
    //    private string UserName;
    }
}
