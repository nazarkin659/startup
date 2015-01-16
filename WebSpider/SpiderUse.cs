using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using System.Threading;
using Gurock.SmartInspect;

namespace WebSpider
{
    public class SpiderUse
    {
        public static CookieCollection Cookies;
        private static readonly object _cookiesLocker = new object();

        static SpiderUse()
        {
            Cookies = new CookieCollection();
            MaxReddirectsAllowed = 5;
        }


        public static int MaxReddirectsAllowed { get; set; }
        /// <summary>
        /// Get response from the server.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trackCookies">Track cookies between requests.</param>
        /// <param name="cookies">Add cookies to request object.</param>
        /// <param name="postVars"> Add POST values, if so, will use POST method for request.</param>
        /// <returns>Server Response</returns>
        private static string GetResponse(string url, ref Spider spider, bool trackCookies = false)
        {
            int requestCount = 0;

            spider.FollowRedirect = true;
            spider.headerCookieFix = true;

            string response = string.Empty;
            string reddirectedUrl = string.Empty;
            try
            {
                spider.SendRequest();

                while (requestCount <= MaxReddirectsAllowed)
                {
                    if (ValidResponse(ref spider))
                        break;
                    else if (isReddirectedOrMoved(ref spider, out reddirectedUrl))
                    {
                        spider.Url = reddirectedUrl;
                    }
                    else if (spider.ErrorResponseObject != null)
                    {
                        SiAuto.Main.LogError("{0}, {0}",
                            spider.ErrorResponseObject.StatusCode,
                            spider.Url);

                        response = spider.ErrorResponseText;
                        return response;
                    }
                    else
                    {
                        //network issue
                        Thread.Sleep(new TimeSpan(0, 0, 0, 1));
                    }

                    spider.SendRequest();
                    requestCount++;
                }

                if (spider.ResponseText.IsNullOrWhiteSpace())
                {
                    response = spider.ResponseText ?? spider.ErrorResponseText;
                }

                response = ReturnResponse(ref spider);
                return response;
            }
            catch (WebException e)
            {
                HelperFunctions.SiExtentions.AddErrorMessage(e.Message, e.Status.ToString());
            }

            return null;
        }

        public static string GetResponse(string url, ref CookieCollection outCookies, bool trackCookies = false, CookieCollection cookies = default(CookieCollection), string postVars = default(string))
        {
            Spider spider = new Spider();
            SetSpider(url, ref spider, trackCookies, cookies, postVars);

            string response = GetResponse(url, ref spider, trackCookies);
            outCookies = spider.Cookies;
            return response;
        }

        public static string GetResponse(string url, bool trackCookies = false, CookieCollection cookies = default(CookieCollection), string postVars = default(string))
        {
            Spider spider = new Spider();
            SetSpider(url, ref spider, trackCookies, cookies, postVars);
            return GetResponse(url, ref spider, trackCookies);
        }

        public static string GetResponse(string url, ref Spider spider, bool trackCookies, CookieCollection cookies, string postVars)
        {
            spider = new Spider();
            SetSpider(url, ref spider, trackCookies, cookies, postVars);
            return GetResponse(url, ref spider, trackCookies);
        }

        private static void SetSpider(string url, ref Spider spider, bool trackCookies = false, CookieCollection cookies = default(CookieCollection), string postVars = default(string))
        {
            Thread.Sleep(1000);
            SiAuto.Main.LogMessage("Spider Sleeps...[{0}]", url);
            spider.Url = url;
            if (!postVars.IsNullOrWhiteSpace())
            {
                spider.Type = HTTPRequestType.Post;
                spider.PostVars = postVars;
            }
            else
                spider.Type = HTTPRequestType.Get;


            if (trackCookies)
            {
                spider.PersistCookies = true;
                spider.CookieBushnellFix = true;
                if (cookies == null)
                    spider.Cookies = new CookieCollection();
                else
                    spider.Cookies = cookies;
            }
            else if (cookies != null && cookies.Count > 0)
                spider.Cookies = cookies;
            else
                spider.Cookies = new CookieCollection();
        }

        /// <summary>
        /// Valid if servers status code is OK and there is no reddirect.
        /// </summary>
        /// <param name="spider">Current instance of spider</param>
        /// <returns></returns>
        private static bool ValidResponse(ref Spider spider)
        {
            if (spider != null && spider.ResponseObject != null && spider.ResponseObject.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        private static bool isReddirectedOrMoved(ref Spider spider, out string reddirectUrl)
        {
            reddirectUrl = string.Empty;

            if (spider != null)
            {
                if (spider.ResponseObject != null)
                {
                    if (spider.ResponseObject.StatusCode == HttpStatusCode.Redirect ||
                    spider.ResponseObject.StatusCode == HttpStatusCode.RedirectMethod ||
                    spider.ResponseObject.StatusCode == HttpStatusCode.TemporaryRedirect ||
                    spider.ResponseObject.StatusCode == HttpStatusCode.Moved ||
                    spider.ResponseObject.StatusCode == HttpStatusCode.MovedPermanently)
                    {
                        reddirectUrl = GetReddirectedURL(ref spider);
                    }

                }
                else if (spider.ErrorResponseObject != null)
                {
                    if (spider.ErrorResponseObject.StatusCode == HttpStatusCode.Moved ||
                            spider.ErrorResponseObject.StatusCode == HttpStatusCode.MovedPermanently ||
                            spider.ErrorResponseObject.StatusCode == HttpStatusCode.Redirect ||
                            spider.ErrorResponseObject.StatusCode == HttpStatusCode.RedirectMethod ||
                            spider.ErrorResponseObject.StatusCode == HttpStatusCode.TemporaryRedirect)
                    {
                        reddirectUrl = GetReddirectedURL(ref spider);
                    }
                }

                if (!reddirectUrl.IsNullOrWhiteSpace())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Use for returning any response.
        /// </summary>
        /// <param name="spider"></param>
        /// <returns></returns>
        private static string ReturnResponse(ref Spider spider)
        {
            if (spider != null)
            {
                lock (_cookiesLocker)
                {
                    Cookies = spider.Cookies; //TO DO: Filter cookies
                }
                return spider.ResponseText;
            }
            return null;
        }

        public static string GetReddirectedURL(ref Spider spider)
        {
            if (spider != null)
            {
                string location = string.Empty;
                if (spider.ResponseObject != null && spider.ResponseObject.Headers != null && spider.ResponseObject.Headers.Keys != null)
                {
                    if (spider.ResponseObject.Headers.AllKeys.Contains("Location"))
                        location = spider.ResponseObject.Headers["Location"];
                }
                else if (spider.ErrorResponseObject != null && spider.ErrorResponseObject.Headers != null && spider.ErrorResponseObject.Headers.Keys != null)
                {
                    if (spider.ErrorResponseObject.Headers.AllKeys.Contains("Location"))
                        location = spider.ErrorResponseObject.Headers["Location"];
                }

                return location;
            }
            return null;
        }

        public static string GetResponseURL(ref Spider spider)
        {
            if (spider != null)
            {
                string location = string.Empty;
                if (spider.ResponseObject != null && spider.ResponseObject.ResponseUri != null)
                    return location = spider.ResponseObject.ResponseUri.ToString();
            }
            return null;
        }
    }
}
