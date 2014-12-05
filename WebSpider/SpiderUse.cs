using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpider
{
    public class SpiderUse : Spider
    {
        protected Spider Spider;

        public SpiderUse()
        {
            Spider = new Spider();
        }


        public virtual string GetResponse()
        {
            return null;
        }
    }
}
