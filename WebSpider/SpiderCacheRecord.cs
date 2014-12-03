using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpider
{

    public class SpiderCacheRecord
    {

        private string url;//key
        public string URL
        {
            get { return url; }
            set { url = value.ToLower(); }
        }

        public DateTime LastModified { get; set; }
        public string ETag { get; set; }
        public string ResponseHash { get; set; }
    }
}
