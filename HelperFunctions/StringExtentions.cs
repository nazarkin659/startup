using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public static class StringExtentions
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return true;

            return false;
        }

        public static string AppendString(this string source, string append)
        {
            return string.Format("{0}{1}", source, append);
        }

        public static string ToLowerSafely(this string source)
        {
            return source ?? "";
        }

        public static string ReplaceSafely(this string source, string replace, string replaceTo)
        {
            if (source == null)
                return "";

            return source.Replace(replace, replaceTo);
        }

        public static bool ContainsSafely(this string source, string value)
        {
            if (!source.IsNullOrWhiteSpace() && source.Contains(value))
                return true;

            return false;
        }

        public static string CorrectUrl(string baseUri, string url, bool isHttps = false)
        {
            if (baseUri.IsNullOrWhiteSpace() || url.IsNullOrWhiteSpace())
                throw new Exception("Domain and url can't be null");

            string newUrl = url;
            if (baseUri.EndsWith("/"))
                baseUri = baseUri.Remove(baseUri.Length - 1, 1);

            if (!newUrl.StartsWith("/"))
                newUrl = "/" + newUrl;

            //local path
            if (url.StartsWith(@"\\"))
            {
                newUrl = newUrl.Remove(0, 2);
                string schemaAndDomain = newUrl.ReplaceSafely(newUrl.Split('/')[0], ""); //remove domain
                newUrl = string.Format("{0}{1}", baseUri, newUrl);
            }

            if (!newUrl.StartsWith(baseUri))
                newUrl = baseUri + newUrl;

            return newUrl;
        }
    }
}
