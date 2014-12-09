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
        public CookieCollection Cookies;
        private string _reddirectURL;
        private int _maxReddirectsAllowed = 5;
        private int _requestCount;


        public SpiderUse()
        {
            Cookies = new CookieCollection();
            _reddirectURL = string.Empty;
            _requestCount = 0;
        }

        public int MaxReddirectsAllowed
        {
            set { this._maxReddirectsAllowed = value; }
            get { return this._maxReddirectsAllowed; }
        }
        /// <summary>
        /// Get latest reddirected URL.
        /// If it's empty the wasn't reddirect.
        /// </summary>
        public string ReddirectedURL
        {
            private set { this._reddirectURL = value; }
            get { return _reddirectURL; }
        }
        /// <summary>
        /// Get response from the server.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trackCookies">Track cookies between requests.</param>
        /// <param name="cookies">Add cookies to request object.</param>
        /// <param name="postValues"> Add POST values, if so, will use POST method for request.</param>
        /// <returns>Server Response</returns>
        public virtual string GetResponse(string url, bool trackCookies, CookieCollection cookies, string postValues)
        {
            Spider spider = new Spider();
            spider.Url = url;
            spider.FollowRedirect = true;
            spider.headerCookieFix = true;

            if (!postValues.IsNullOrWhiteSpace())
            {
                spider.Type = HTTPRequestType.Post;
                spider.PostVars = postValues;
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

            string response = string.Empty;
            string reddirectedUrl = string.Empty;
            try
            {
                spider.SendRequest();

                while (this._requestCount <= this._maxReddirectsAllowed)
                {
                    if (ValidResponse(ref spider))
                    {
                        response = ReturnResponse(ref spider);
                        break;
                    }

                    if (isReddirectedOrMoved(ref spider, out reddirectedUrl))
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
                    this._requestCount++;
                }

                if (response.IsNullOrWhiteSpace())
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

        /// <summary>
        /// Valid if servers status code is OK and there is no reddirect.
        /// </summary>
        /// <param name="spider">Current instance of spider</param>
        /// <returns></returns>
        private bool ValidResponse(ref Spider spider)
        {
            if (spider != null && spider.ResponseObject != null && spider.ResponseObject.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        private bool isReddirectedOrMoved(ref Spider spider, out string reddirectUrl)
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
        private string ReturnResponse(ref Spider spider)
        {
            if (spider != null)
            {
                this.Cookies = spider.Cookies; //TO DO: Filter cookies
                return spider.ResponseText;
            }
            return null;
        }

        private string GetReddirectedURL(ref Spider spider)
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

                this._reddirectURL = location;
                return location;
            }
            return null;
        }

        /// <summary>
        /// 'GET' response.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual string GetResponse(string url)
        {
            return this.GetResponse(url, false, null, null);
        }

        public virtual string GetResponse(string url, bool trackCookies)
        {
            return this.GetResponse(url, trackCookies, null, null);
        }

        public virtual string GetResponse(string url, bool trackCookies, CookieCollection cookies)
        {
            return this.GetResponse(url, trackCookies, cookies, null);
        }
    }
}
