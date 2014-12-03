using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace HelperFunctions
{
    public static class SiExtentions
    {
        public static void AddNoResponseMessage(HttpWebRequest response)
        {
            if (response != null)
                SiAuto.Main.LogWarning("No response from server [{0}]", response.RequestUri);
        }

        public static void AddErrorMessage(string errorText, string statusCode)
        {
            if (!errorText.IsNullOrWhiteSpace())
            {
                SiAuto.Main.LogWarning("Server returned error [Status Code = {1} , Message = {2}]", statusCode, errorText);
            }
        }
    }
}
