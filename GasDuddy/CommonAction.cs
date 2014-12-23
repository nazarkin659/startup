using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using GasBuddy.Model;
using CsQuery;
using WebSpider;
using Gurock.SmartInspect;
using System.Web;
using GasBuddy.Model.ComplexTypes;

namespace GasBuddy
{
    public class CommonAction
    {
        /// <summary>
        /// Get stations urls.
        /// </summary>
        /// <param name="zipCode">Area zip code to search.</param>
        /// <returns></returns>
        public static List<string> GetStations(string zipCode)
        {
            if (!zipCode.IsNullOrWhiteSpace())
            {
                string url = string.Format("https://m.gasbuddy.com/GasPriceSearch.aspx?t={0}", zipCode);

                CQ searchPage = SpiderUse.GetResponse(url, true);
                if (!searchPage.IsNullOrEmpty())
                {
                    string selector = "#tblPrices th.g>a";
                    if (!searchPage[selector].IsNullOrEmpty())
                    {
                        List<string> urls = searchPage[selector].GetMatchedAttrs("href").Select(u => StringExtentions.CorrectUrl("https://m.gasbuddy.com", u)).ToList();
                        return urls;
                    }
                    else
                        SiAuto.Main.LogError("GetStations Selector has been changed. [{0}]", url);
                }
            }
            return null;
        }

        /// <summary>
        /// Report prices on mobile website
        /// </summary>
        /// <param name="stationUrl"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool SuccessReportPriceMobile(string stationUrl, User user)
        {
            if (!stationUrl.IsNullOrWhiteSpace() && user != null)
            {
                CQ html = SpiderUse.GetResponse(stationUrl, true, user.Mobile.Cookies);
                Model.Common.Price price = GetPrevPrices(ref html);
                if (!html.IsNullOrEmpty() && price.IsValid())
                {
                    Common.ASPStats stats = new Common.ASPStats(html);
                    if (stats != null)
                    {
                        string reportPriceUrl = BuildReportPriceUrl(stationUrl);
                        if (!reportPriceUrl.IsNullOrWhiteSpace())
                        {
                            string postData = string.Format("__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}&ctl00$content$fvStation$hfId={3}&ctl00$content$fvStation$hfCountry={4}&ctl00$content$fvStation$txtRegular={5}&ctl00$content$fvStation$txtMidgrade={6}&ctl00$content$fvStation$txtPremium={7}&ctl00$content$fvStation$txtDiesel={8}&ctl00$content$fvStation$txtComments={9}&ctl00$content$fvStation$ddlTimeSpotted={10}&ctl00$content$fvStation$btnSubmitPrices={11}&ctl00$content$fvStation$hfName={12}&ctl00$content$fvStation$hfAddress={13}&ctl00$content$fvStation$hfArea={14}&ctl00$content$fvStation$hfTimeOffset={15}",
                                stats.ViewState,
                                stats.ViewStateGenerator,
                                stats.EventValidation,
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfId']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfCountry']"].Val()),
                                HttpUtility.UrlEncode(price.Regular),
                                HttpUtility.UrlEncode(price.Midgrade),
                                HttpUtility.UrlEncode(price.Premium),
                                HttpUtility.UrlEncode(price.Diesel),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$txtComments']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$ddlTimeSpotted']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$btnSubmitPrices']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfName']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfAddress']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfArea']"].Val()),
                                HttpUtility.UrlEncode(html["[name='ctl00$content$fvStation$hfTimeOffset']"].Val())
                                );

                            Spider spider = null;
                            CQ response = SpiderUse.GetResponse(reportPriceUrl, ref spider, false, user.Mobile.Cookies, postData);
                            if (!response.IsNullOrEmpty() && isReported(ref response, ref user))
                            {
                                return true;
                            }
                            else
                            {
                                //TO DO: Logs 
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static string BuildReportPriceUrl(string stationUrl)
        {
            if (!stationUrl.IsNullOrWhiteSpace())
            {
                if (stationUrl.Contains("ReportGasPrices.aspx?"))
                {
                    QueryStringBuilder qb = new QueryStringBuilder(new Uri(stationUrl));
                    string sValue = qb.GetValue("s");
                    if (!sValue.IsNullOrWhiteSpace())
                    {
                        string reportUrl = string.Format("https://m.gasbuddy.com/ReportGasPrices.aspx?s={0}", sValue);
                        return reportUrl;
                    }
                }
            }
            return null;
        }

        private static Model.Common.Price GetPrevPrices(ref CQ page)
        {
            if (!page.IsNullOrEmpty())
            {
                Model.Common.Price price = new Common.Price();

                string mainSelector = "table.bt tr:has(>td.price_fuel_prev)";
                price.Regular = page[mainSelector + ":has(label:contains('Regular')) .price_fuel_prev>div"].Text();
                price.Midgrade = page[mainSelector + ":has(label:contains('Midgrade')) .price_fuel_prev>div"].Text();
                price.Premium = page[mainSelector + ":has(label:contains('Premium')) .price_fuel_prev>div"].Text();
                price.Diesel = page[mainSelector + ":has(label:contains('Diesel')) .price_fuel_prev>div"].Text();

                return price;
            }
            return null;
        }

        private static bool isReported(ref CQ html, ref User user)
        {
            if (!html.IsNullOrEmpty()
                && !html[string.Format("a>:contains('{0}')", user.UserName)].IsNullOrEmpty()
                )
            {
                return true;
            }
            return false;
        }
    }
}
