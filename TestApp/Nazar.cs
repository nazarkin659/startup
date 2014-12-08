using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using WebSpider;
using GasDuddy;
using Blablabla;
using System.Net;

namespace TestApp
{
    public class Nazar : SpiderUse
    {
        public void TestingSpider()
        {
            try
            {
                string requestURL = "https://secure.gasbuddy.com/login.aspx?site=Illinois";

                string response = GetResponse(requestURL);
            }
            catch (WebException ex)
            {
            }
            catch (Exception e)
            {
            }
        }
    }
}
