using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using WebSpider;
using GasBuddy;
using Blablabla;
using System.Net;
using Newtonsoft.Json;
using System.Web;

namespace TestApp
{
    public class Nazar
    {
        public void TestingSpider()
        {
            try
            {
                string requestURL = "https://secure.gasbuddy.com/login.aspx?site=Illinois";

                string response = SpiderUse.GetResponse(requestURL);
            }
            catch (WebException ex)
            {
            }
            catch (Exception e)
            {
            }
        }

        public static void NewtonJson()
        {
            try
            {

                CookieCollection cookies = new CookieCollection() { new Cookie("testname", "testvalue", "somepath", "somedamain") };

                string serialized = JSONHelper.ToJSON(cookies, typeof(CookieCollection));
                cookies = new CookieCollection();

                cookies = JSONHelper.FromJSON<CookieCollection>(serialized);

            }
            catch (Exception e)
            {
            }
        }
    }
}
