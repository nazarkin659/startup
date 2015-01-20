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
using GasBuddy.Model;
using GasBuddy.Infrastructure;

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

        public static void TestProcessQueue()
        {
            try
            {
                ProcessQueue queue = new ProcessQueue();
                queue.FailCount = 1;
                queue.Successful = false;
                queue.UserID = 10;

                ProcessQueueF.AddRecord(queue);

                ProcessQueueF.RemoveRecord(queue);

                ProcessQueueF.AddRecord(queue);

                ProcessQueueF.UpdateRecordInQueue(queue);
            }
            catch (Exception e)
            { 
            }
        }
    }
}
