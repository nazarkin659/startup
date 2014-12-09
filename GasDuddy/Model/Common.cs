using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using System.Web;

namespace GasDuddy.Model
{
    public class Common
    {
        public class ASPStats
        {
            public string EventTarget = "";
            public string EventArgument = "";
            public string ViewState = "";
            public string ViewStateGenerator = "";
            public string PreviousPage = "";
            public string EventValidation = "";

            public ASPStats(CsQuery.CQ page)
            {
                if (!page.IsNullOrEmpty())
                {
                    EventTarget = HttpUtility.UrlEncode(page["[name='__EVENTTARGET']"].Val());
                    EventArgument = HttpUtility.UrlEncode(page["[name='__EVENTARGUMENT']"].Val());
                    ViewState = HttpUtility.UrlEncode(page["[name='__VIEWSTATE']"].Val());
                    ViewStateGenerator = HttpUtility.UrlEncode(page["[name='__VIEWSTATEGENERATOR']"].Val());
                    PreviousPage = HttpUtility.UrlEncode(page["[name='__PREVIOUSPAGE']"].Val());
                    EventValidation = HttpUtility.UrlEncode(page["[name='__EVENTVALIDATION']"].Val());
                }
            }
        }

    }
}
