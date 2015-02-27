using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using System.Net;
using System.Threading;
using System.Web;
using HelperFunctions;
using WebSpider;
using GasBuddy.Model;
using GasBuddy.Infrastructure;
using Gurock.SmartInspect;

namespace GasBuddy
{
    public class Authorization : BaseClass
    {
        private User _user;
        public User User { get { return this._user; } }
        public Authorization(User user)
        {
            _user = user;
        }
        /// <summary>
        /// Trying to login into website.  
        /// Should be used for any login attempt.
        /// </summary>
        /// <param name="count">How many times trying to login if fails.</param>
        /// <returns></returns>
        public bool ProceedMobileLogin(int count = 3)
        {
            if (_user != null)
            {
                while (!_user.Mobile.isLoggedIn && count >= 0)
                {
                    if (!LoginMobile())
                    {
                        SiAuto.Main.LogError("CommonAction => ProceedMobileLogin: Failed to Login, attempt [{0}], Mobile.Response [{1}]", count, _user.Mobile.Response);
                    }
                    count++;
                }

            }
            return _user.Mobile.isLoggedIn;
        }
        /// <summary>
        /// Trying to login into mobile website.
        /// </summary>
        /// <param name="count">How many times trying to login if fails.</param>
        /// <returns></returns>
        public bool ProceedWebSiteLogin(int count = 3)
        {
            if (_user != null)
            {
                while (!_user.Website.isLoggedIn && count >= 3)
                {
                    if (!LoginWebsite())
                    {
                        SiAuto.Main.LogError("CommonAction => ProceedWebSiteLogin: Failed to Login, attempt [{0}], Website.Response [{1}]", count, _user.Website.Response);
                    }
                    count++;
                }

            }
            return _user.Website.isLoggedIn;
        }



        private bool LoginMobile()
        {
            string loginUrl = "https://m.gasbuddy.com/Mem_Log_In.aspx";
            List<string> postKeys = new List<string>
            {
                "ctl00$content$txtUsername",
                "ctl00$content$txtPassword",
                "__VIEWSTATE",
                "__VIEWSTATEGENERATOR",
                "__EVENTVALIDATION",
                "ctl00$content$chkRememberMe",
                "ctl00$content$btnLogin",
                "ctl00$content$hfRedirect"
            };

            return Login(loginUrl, postKeys, _user.Mobile);
        }
        private bool LoginWebsite()
        {
            string loginUrl = "https://secure.gasbuddy.com/login.aspx?site=Chicago&returnURL=http%3a%2f%2fwww.chicagogasprices.com%2fmem_log_in.aspx%3fredirect%3dhttp%253a%252f%252fwww.chicagogasprices.com%252f";
            List<string> postKeys = new List<string>
            {
                "ctl00$main$txtMember_nm",
                "ctl00$main$txtPassword",
                "__EVENTTARGET",
                "__EVENTARGUMENT",
                "__VIEWSTATE",
                "__VIEWSTATEGENERATOR",
                "__EVENTVALIDATION",
                "ctl00$main$hfRedirectUrl",
                "ctl00$main$txtaccesstoken",
                "ctl00$main$txtnetworkid",
                "ctl00$main$txtAddress",
                "ctl00$main$chkSavePwd",
                "ctl00$main$btnSignIN"
            };

            return Login(loginUrl, postKeys, _user.Website);
        }
        private bool Login(string logInUrl, List<string> postKeys, dynamic loginType)
        {
            if (_user != null || !_user.Password.IsNullOrWhiteSpace() || !_user.UserName.IsNullOrWhiteSpace())
            {
                if (!string.IsNullOrWhiteSpace(_user.UserName) && !string.IsNullOrWhiteSpace(_user.Password))
                {
                    string requestUrl = logInUrl;

                    string postData = string.Empty;
                    CookieCollection userCookies = new CookieCollection();

                    CQ html = SpiderUse.GetResponse(requestUrl, ref userCookies, true);
                    if (!html.IsNullOrEmpty())
                    {
                        for (int i = 0; i < postKeys.Count(); i++)
                        {
                            if (i != 0)
                                postData += "&";

                            switch (postKeys[i])
                            {
                                case "ctl00$main$txtMember_nm": //webiste username
                                    postData += string.Format("ctl00$main$txtMember_nm={0}", HttpUtility.UrlEncode(_user.UserName));
                                    break;
                                case "ctl00$main$txtPassword": //website psw
                                    postData += string.Format("ctl00$main$txtPassword={0}", HttpUtility.UrlEncode(_user.Password));
                                    break;
                                case "ctl00$content$txtUsername": //old mobile username
                                    postData += string.Format("ctl00$content$txtUsername={0}", HttpUtility.UrlEncode(_user.UserName));
                                    break;
                                case "ctl00$content$txtPassword": //old mobile psw
                                    postData += string.Format("ctl00$content$txtPassword={0}", HttpUtility.UrlEncode(_user.Password));
                                    break;
                                default:
                                    postData += string.Format("{0}=", postKeys[i]) + HttpUtility.UrlEncode(html[string.Format("[name='{0}']", postKeys[i])].Val());
                                    break;
                            }
                        }

                        Spider spider = null;
                        string responseLogedIn = SpiderUse.GetResponse(requestUrl, ref spider, true, userCookies, postData);
                        loginType.CheckLoginURL = spider.ResponseObject.ResponseUri.ToString();
                        loginType.Cookies = SpiderUse.Cookies;
                        loginType.Response = responseLogedIn;


                        CQ responseCQ = responseLogedIn;
                        if (spider.ResponseObject != null && spider.ResponseObject.StatusCode == HttpStatusCode.OK && loginType.isLoggedIn)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
