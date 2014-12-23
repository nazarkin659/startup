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

namespace GasBuddy
{
    internal class Authorization : BaseClass
    {
        public Authorization()
        {
        }

        public static bool LoginMobile(ref User user)
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

            if (Login(ref user, loginUrl, postKeys, user.Mobile))
                return true;

            return false;
        }

        public static bool LoginWebsite(ref User user)
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

            if (Login(ref user, loginUrl, postKeys, user.Mobile))
                return true;

            return false;
        }

        private static bool Login(ref User user, string logInUrl, List<string> postKeys, GasBuddy.Model.ComplexTypes.LoginType loginType)
        {
            if (user != null || !user.Password.IsNullOrWhiteSpace() || !user.UserName.IsNullOrWhiteSpace())
            {
                if (!string.IsNullOrWhiteSpace(user.UserName) && !string.IsNullOrWhiteSpace(user.Password))
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
                                    postData += string.Format("ctl00$main$txtMember_nm={0}", HttpUtility.UrlEncode(user.UserName));
                                    break;
                                case "ctl00$main$txtPassword": //website psw
                                    postData += string.Format("ctl00$main$txtPassword={0}", HttpUtility.UrlEncode(user.Password));
                                    break;
                                case "ctl00$content$txtUsername": //old mobile username
                                    postData += string.Format("ctl00$content$txtUsername={0}", HttpUtility.UrlEncode(user.UserName));
                                    break;
                                case "ctl00$content$txtPassword": //old mobile psw
                                    postData += string.Format("ctl00$content$txtPassword={0}", HttpUtility.UrlEncode(user.Password));
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

                        CQ responseCQ = responseLogedIn;
                        if (spider.ResponseObject != null && spider.ResponseObject.StatusCode == HttpStatusCode.OK && loginType.isLoggedIn)
                        {
                            loginType.Response = responseLogedIn;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
