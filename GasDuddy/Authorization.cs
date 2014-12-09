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
using GasDuddy.Model;

namespace GasDuddy
{
    internal class Authorization
    {
        public Authorization()
        {
        }

        public static bool Login(ref User user)
        {
            if (user != null || !user.Password.IsNullOrWhiteSpace() || !user.UserName.IsNullOrWhiteSpace())
            {
                if (!string.IsNullOrWhiteSpace(user.UserName) && !string.IsNullOrWhiteSpace(user.Password))
                {
                    string requestUrl = string.Format("https://secure.gasbuddy.com/login.aspx?site=Chicago&returnURL=http%3a%2f%2fwww.chicagogasprices.com%2fmem_log_in.aspx%3fredirect%3dhttp%253a%252f%252fwww.chicagogasprices.com%252f");

                    string postData = string.Empty;
                    CookieCollection userCookies = new CookieCollection();

                    CQ html = SpiderUse.GetResponse(requestUrl, ref userCookies, true);
                    if (!html.IsNullOrEmpty())
                    {
                        Common.ASPStats stats = new Common.ASPStats(html);

                        postData += string.Format(@"__EVENTTARGET={0}&__EVENTARGUMENT={1}&__VIEWSTATE={2}&__VIEWSTATEGENERATOR={3}&__EVENTVALIDATION={4}&ctl00$main$hfRedirectUrl={5}&ctl00$main$txtaccesstoken={6}&ctl00$main$txtnetworkid={7}&ctl00$main$txtMember_nm={8}&ctl00$main$txtPassword={9}&ctl00$main$txtAddress={10}&ctl00$main$chkSavePwd={11}&ctl00$main$btnSignIN={12}",
                            stats.EventTarget,
                            stats.EventArgument,
                            stats.ViewState,
                            stats.ViewStateGenerator,
                            stats.EventValidation,
                            HttpUtility.UrlEncode(html["[name='ctl00$main$hfRedirectUrl']"].Val()),
                            HttpUtility.UrlEncode(html["[name='ctl00$main$txtaccesstoken']"].Val()),
                            HttpUtility.UrlEncode(html["[name='ctl00$main$txtnetworkid']"].Val()),
                            HttpUtility.UrlEncode(user.UserName),
                            HttpUtility.UrlEncode(user.Password),
                            HttpUtility.UrlEncode(html["[name='ctl00$main$txtAddress']"].Val()),
                            HttpUtility.UrlEncode(html["[name='ctl00$main$chkSavePwd']"].Val()),
                            HttpUtility.UrlEncode(html["[name='ctl00$main$btnSignIN']"].Val())
                            );



                        Spider spider = null;
                        string responseLogedIn = SpiderUse.GetResponse(requestUrl, ref spider, true, userCookies, postData);
                        if (spider.ResponseObject != null && spider.ResponseObject.StatusCode == HttpStatusCode.OK)
                        {
                            //TO DO: Additional succsessful login varification.
                            user.isLoggedIn = true;
                            user.Cookies = SpiderUse.Cookies;
                            user.Response = responseLogedIn;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
