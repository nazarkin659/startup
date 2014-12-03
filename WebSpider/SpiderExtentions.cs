using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using HelperFunctions;

namespace WebSpider
{
    public class SpiderExtentions
    {
        private static int CurrentTry = 0;
        private static int MaxWebRetry = 5;

        private static WebSpider.Spider Spidering(ref WebSpider.Spider spd)
        {
            if (spd == null)
                return spd;

            if (spd.ResponseText.IsNullOrWhiteSpace())
            {
                spd.SendRequest();

                try
                {
                    if (spd.ResponseObject == null && CurrentTry <= MaxWebRetry)
                    {
                        if (spd.ErrorResponseObject != null) //we got error
                        {
                            if (!spd.ErrorResponseText.IsNullOrWhiteSpace())
                            {
                                SiExtentions.AddErrorMessage(spd.ErrorResponseText, spd.StatusCode.ToString());
                            }
                            else
                                throw new Exception("It's should not happend...");

                            return spd;
                        }

                        if (spd.ResponseObject != null) //everything is fine
                        {
                            if (!spd.ResponseText.IsNullOrWhiteSpace())
                                return spd;

                            if (spd.ResponseObject.Headers != null) //looking for reddirect
                            {
                                if (spd.ResponseObject.StatusDescription.ToLowerSafely().Contains("moved")
                                    || !spd.ResponseObject.Headers["Location"].IsNullOrWhiteSpace()
                                    )
                                {
                                    string reddirectedURL = spd.ResponseObject.Headers["Location"];
                                    if (reddirectedURL.IsNullOrWhiteSpace())
                                        throw new Exception("Check when it's happend");
                                    else
                                    {
                                        spd.Url = reddirectedURL;
                                        spd.SendRequest();
                                    }
                                }
                            }

                            return spd;
                        }

                    }
                    else if (spd.ResponseObject == null)
                    {
                        SiExtentions.AddNoResponseMessage(spd.RequestObject);
                    }
                }
                catch (Exception e)
                {
                    SiAuto.Main.LogException("SpiderExtentions", e);
                }
                finally
                {
                    CurrentTry++;
                }
            }

            return spd;
        }


        public static WebSpider.Spider GetResponseObj(ref WebSpider.Spider spider, int maxWebRetry = 5)
        {
            if (spider == null)
                return null;

            CurrentTry = 0;

            string responseText = "";


            while (responseText.IsNullOrWhiteSpace() && CurrentTry <= MaxWebRetry)
            {
                spider = Spidering(ref spider);
                responseText = spider.ResponseText;
            }

            return spider;
        }

        public static string GetResponseText(ref WebSpider.Spider spider, int maxWebRetry = 5)
        {
            if (spider == null)
                return null;

            CurrentTry = 0;

            string responseText = "";


            while (responseText.IsNullOrWhiteSpace() && CurrentTry <= 5)
            {
                spider = Spidering(ref spider);
                responseText = spider.ResponseText;
            }

            return responseText;
        }
    }
}