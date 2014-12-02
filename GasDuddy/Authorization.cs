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

namespace GasDuddy
{
    internal class Authorization
    {
        private HttpCookieCollection cookies;
        private int RetryCount = 0;
        private string UserName = "nazarkin659";
        private string Password = "Hakers659";

        public Authorization()
        {
            cookies = new HttpCookieCollection();
        }

        public bool Login()
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                string url = string.Format("https://secure.gasbuddy.com/login.aspx?site=Illinois");

                string postData = string.Empty;

                Dictionary<string, string> viewStates = GetViewStates();
                if (!viewStates.IsNullOrEmpty())
                {
                    postData += string.Format(@"__VIEWSTATEGENERATOR={0}&__EVENTVALIDATION={1}ctl00$main$chkSavePwd={2}&
                        ctl00$main$btnSignIN={3}&ctl00$main$hfRedirectUrl={4}&ctl00$main$txtnetworkid={5}&
                        ctl00$main$txtAddress={6}&
                        ctl00$main$txtMember_nm={7}&
                        ctl00$main$txtPassword={8}",
                        viewStates["viewStateGenerator"],
                        viewStates["eventValidation"],
                        "on",
                        "Log In",
                        "",
                        "",
                        "",
                        UserName,
                        Password
                        );

                    //Send POST request

                    HttpResponse response = null;
                    if (response.StatusCode == 200 && response.RedirectLocation!=null && !response.RedirectLocation.Contains("/error.aspx"))
                    {
                        //TO DO: Additional succsessful login varification.
                        cookies = response.Cookies;
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
                response = CQ.CreateFromUrl(requestURL);
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
                    result.Add("eventvalidation", eventValidation);

                    return result;
                }
            }

            return null;
        }
    }
}
