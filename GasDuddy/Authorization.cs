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

namespace GasDuddy
{
    internal class Authorization : BaseClass
    {
        private int RetryCount = 0;
        private string UserName = "nazarkin659";
        private string Password = "Hakers659";

        public Authorization()
        {

        }

        public bool Login()
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                string requestUrl = string.Format("https://secure.gasbuddy.com/login.aspx?site=Chicago&returnURL=http%3a%2f%2fwww.chicagogasprices.com%2fmem_log_in.aspx%3fredirect%3dhttp%253a%252f%252fwww.chicagogasprices.com%252f");

                string postData = string.Empty;

                //Dictionary<string, string> viewStates = GetViewStates();

                CQ html = GetResponse(requestUrl, true);
                if (!html.IsNullOrEmpty())
                {
                    postData += string.Format(@"__EVENTTARGET={0}&__EVENTARGUMENT={1}&__VIEWSTATE={2}&__VIEWSTATEGENERATOR={3}&__EVENTVALIDATION={4}&ctl00$main$hfRedirectUrl={5}&ctl00$main$txtaccesstoken={6}&ctl00$main$txtnetworkid={7}&ctl00$main$txtMember_nm={8}&ctl00$main$txtPassword={9}&ctl00$main$txtAddress={10}&ctl00$main$chkSavePwd={11}&ctl00$main$btnSignIN={12}",
                        HttpUtility.UrlEncode(html["[name='__EVENTTARGET']"].Val()),
                        HttpUtility.UrlEncode(html["[name='__EVENTARGUMENT']"].Val()),
                        HttpUtility.UrlEncode(html["[name='__VIEWSTATE']"].Val()),
                        HttpUtility.UrlEncode(html["[name='__VIEWSTATEGENERATOR']"].Val()),
                        HttpUtility.UrlEncode(html["[name='__EVENTVALIDATION']"].Val()),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$hfRedirectUrl']"].Val()),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$txtaccesstoken']"].Val()),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$txtnetworkid']"].Val()),
                        HttpUtility.UrlEncode(this.UserName),
                        HttpUtility.UrlEncode(this.Password),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$txtAddress']"].Val()),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$chkSavePwd']"].Val()),
                        HttpUtility.UrlEncode(html["[name='ctl00$main$btnSignIN']"].Val())
                        );


                    

                    string responseLogedIn = GetResponse(requestUrl, true, base.Cookies, postData);

                    //spider.Url = requestPostUrl;
                    //spider.Type = HTTPRequestType.Post;
                    //spider.PostVars = postData;

                    //spider.SendRequest();
                    try
                    {

                    }
                    catch (WebException e)
                    {
                        SiExtentions.AddErrorMessage(e.Message, e.Status.ToString());
                    }

                    HttpWebResponse response = null;
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        //TO DO: Additional succsessful login varification.
                        base.Cookies = response.Cookies;
                        return true;
                    }
                }
            }
            return false;
        }

        private Dictionary<string, string> GetViewStates(string url = "")
        {
            //if (RetryCount > 5)
            //    return null;


            string requestURL = url;
            if (string.IsNullOrWhiteSpace(requestURL))
                requestURL = "https://secure.gasbuddy.com/login.aspx?site=Illinois&returnURL=http://www.illinoisgasprices.com/";

            CQ response = null;
            try
            {
                response = GetResponse(requestURL, true);
            }
            catch (HttpException wEx)
            {
                //TO DO: Exeption handling
                switch (wEx.WebEventCode)
                {
                    case 404:

                        break;
                    default:
                        break;
                }

                //logs are here
                //sleep 15 minutes
                //Thread.Sleep(new TimeSpan(0, 15, 0));
                //RetryCount++;
                //GetViewState(requestURL);
            }

            if (!response.IsNullOrEmpty())
            {
                string viewStageGenerator = response["#__VIEWSTATEGENERATOR"].Val();
                string eventValidation = response["#__EVENTVALIDATION"].Val();

                if (!viewStageGenerator.IsNullOrWhiteSpace() || !eventValidation.IsNullOrWhiteSpace())
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("viewStateGenerator", viewStageGenerator);
                    result.Add("eventValidation", eventValidation);

                    return result;
                }
            }

            return null;
        }
    }
}
