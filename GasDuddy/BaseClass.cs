using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSpider;

namespace GasDuddy
{
    internal class BaseClass : WebSpider.SpiderUse
    {
        internal HttpCookieCollection Cookies { get; set; }

    }
}
