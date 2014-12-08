using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSpider;

namespace GasDuddy
{
    internal class BaseClass : WebSpider.SpiderUse
    {
        internal CookieCollection Cookies
        {
            get
            {
                return base.Cookies;
            }
            set
            {
                base.Cookies = value;
            }
        }

        public BaseClass()
        {
            Cookies = new CookieCollection();
        }

    }
}
