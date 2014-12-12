using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace HelperFunctions
{
    public static class CsQueryExtentions
    {
        public static List<string> GetMatchedAttrs(this CQ cq, string attrName)
        {
            List<string> attrs = new List<string>();
            if (!cq.IsNullOrEmpty())
            {
                cq.Each((index, dom) =>
                {
                    string attrValue = dom.GetAttribute(attrName);
                    if (!attrValue.IsNullOrWhiteSpace())
                        attrs.Add(attrValue);
                });
            }

            return attrs;
        }

        public static List<string> GetMatchedAttrs(this CQ cq, string selector, string attrName)
        {
            List<string> attrs = new List<string>();
            if (!cq.IsNullOrEmpty())
            {
                cq.Each((index, dom) =>
                {
                    string value = dom.Cq().Find(selector).Attr(attrName);
                    if (!value.IsNullOrWhiteSpace())
                        attrs.Add(value);
                });
            }

            return attrs;
        }
    }
}
