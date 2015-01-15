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

        public static int? GetTodaysPoints(ref User user)
        {
            if (user != null && user.Website != null && !string.IsNullOrWhiteSpace(user.Website.URL))
            {
                if (!user.Website.isLoggedIn && (!Authorization.LoginWebsite(ref user)))
                {
                    //TODO: Logs here
                }
                else
                {
                    CQ html = SpiderUse.GetResponse(user.Website.URL, false, user.Website.Cookies);
                    if (!html.IsNullOrEmpty())
                    {
                        string tdPoints = html["th:contains('s Points')+td"].Text().Trim();

                        if (!string.IsNullOrWhiteSpace(tdPoints))
                        {
                            decimal todayPoints;
                            if (decimal.TryParse(tdPoints.Trim(), out todayPoints))
                                return Convert.ToInt32(todayPoints);
                        }
                    }
                }
            }

            return null;
        }

        public static bool isReachedTodayMaxPoints(User user)
        {
            if (user != null && user.Website != null)
            {
                int? todayPoints = CommonAction.GetTodaysPoints(ref user);
                if (todayPoints != null && todayPoints == user.TodayPointsReceived)
                    return true;
                else
                {
                    SiAuto.Main.LogMessage("{0} Today's Points == {1}, continue...", todayPoints);
                }
            }

            return false;
        }

        /// <summary>
        /// Place Prize Entries. 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Updated User Object</returns>
        public static bool ReportPrizeEntries(ref User user, ref ContactInfo userContacInto)
        {
            if (user != null && userContacInto != null)
            {
                string prizeReportURL = "https://m.gasbuddy.com/Prize.aspx";
                CQ html = SpiderUse.GetResponse(prizeReportURL, false, user.Mobile.Cookies);
                if (!html.IsNullOrEmpty())
                {
                    int? prizes = GetAvailablePrizes(ref html);
                    if (prizes == null)
                        SiAuto.Main.LogError("Can't get available prizes to report. User {0}", user.UserName);
                    else if (prizes == 0)
                    {
                        SiAuto.Main.LogMessage("No Available Prizes, User {0}", user.UserName);
                    }
                    else
                    {
                        int prizesToReport = 0;

                        #region Set Prizes To Report
                        switch (user.PrizesToReport)
                        {
                            case 0: //do not report prizes
                                return true;

                            case -1: //report all prizes available
                                prizesToReport = (int)prizes;
                                break;

                            default:
                                prizesToReport = user.PrizesToReport;
                                break;
                        }
                        #endregion Set Prizes To Report

                        Common.ASPStats stats = new Common.ASPStats(html);
                        if (stats != null)
                        {
                            string postData = string.Format("__EVENTVALIDATION={0}&__VIEWSTATE={1}&__VIEWSTATEGENERATOR={2}&ctl00$content$txtTickets={3}&ctl00$content$btnGetTickets={4}",
                                    stats.EventValidation,
                                    stats.ViewState,
                                    stats.ViewStateGenerator,
                                    HttpUtility.UrlEncode(prizesToReport.ToString()),
                                    HttpUtility.UrlEncode(html["[name='ctl00$content$btnGetTickets']"].Val())
                                );

                            Spider spider = null;
                            CQ prizesContactInfoResponse = SpiderUse.GetResponse(prizeReportURL, ref spider, true, user.Mobile.Cookies, postData);
                            string contactInfoUrl = SpiderUse.GetResponseURL(ref spider);
                            if (!prizesContactInfoResponse.IsNullOrEmpty() &&
                                !string.IsNullOrWhiteSpace(contactInfoUrl) &&
                                !prizesContactInfoResponse[string.Format(":contains('You have entered {0} ticket for this prize draw')", prizesToReport.ToString())].IsNullOrEmpty())
                            {
                                stats = new Common.ASPStats(prizesContactInfoResponse);
                                if (stats != null)
                                {
                                    string contactInfoPagePostData = string.Format("__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}&ctl00$content$fvContactInfo$hfTickets={3}&ctl00$content$fvContactInfo$txtFirstName={4}&ctl00$content$fvContactInfo$txtLastName={5}&ctl00$content$fvContactInfo$txtAddress={6}&ctl00$content$fvContactInfo$txtAddress2={7}&ctl00$content$fvContactInfo$txtCity={8}&ctl00$content$fvContactInfo$ddlState={9}&ctl00$content$fvContactInfo$txtZipCode={10}&ctl00$content$fvContactInfo$txtEmail={11}&ctl00$content$fvContactInfo$txtSubmit={12}",
                                        HttpUtility.HtmlDecode(stats.ViewState),
                                        HttpUtility.HtmlDecode(stats.ViewStateGenerator),
                                        HttpUtility.HtmlDecode(stats.EventValidation),
                                        HttpUtility.HtmlDecode(prizesToReport.ToString()),
                                        HttpUtility.HtmlDecode(userContacInto.FirstName),
                                        HttpUtility.HtmlDecode(userContacInto.LastName),
                                        HttpUtility.HtmlDecode(userContacInto.Address),
                                        HttpUtility.HtmlDecode(userContacInto.Unit),
                                        HttpUtility.HtmlDecode(userContacInto.City),
                                        HttpUtility.HtmlDecode(userContacInto.State),
                                        HttpUtility.HtmlDecode(userContacInto.ZipCode.ToString()),
                                        HttpUtility.HtmlDecode(userContacInto.Email),
                                        HttpUtility.HtmlDecode(prizesContactInfoResponse["[name='ctl00$content$fvContactInfo$txtSubmit']"].Val())
                                        );

                                    CQ finalResponse = SpiderUse.GetResponse(contactInfoUrl, false, user.Mobile.Cookies, contactInfoPagePostData);
                                    if (finalResponse.IsNullOrEmpty() || !ValidatePrizeReport(ref user, ref finalResponse, prizes, prizesToReport))
                                        SiAuto.Main.LogError("Report Failed: User {0}", user.UserName);
                                    else
                                    {
                                        SiAuto.Main.LogMessage("Reported Succsessfully, User {0}", user.UserName);
                                        int? entriesReported = GetPrizeEntriesReported(ref finalResponse);
                                        if (entriesReported == null)
                                        {
                                            SiAuto.Main.LogError("Entries Reported  == null, User {0}", user.UserName);
                                        }
                                        else if (entriesReported == 0)
                                        {
                                            SiAuto.Main.LogError("Should not happen. Entries Reported == 0. User {0}", user.UserName);
                                        }
                                        else
                                        {
                                            user.PrizeEntriesReported = entriesReported;
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SiAuto.Main.LogError("Can't report prizes, User {0}", user.UserName);
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool ValidatePrizeReport(ref User user, ref CQ prizeReportHtml, int? oldPrizesValue, int? prizesToReport)
        {
            if (user != null && !prizeReportHtml.IsNullOrEmpty() && oldPrizesValue != null && prizesToReport != null)
            {
                int? prizesAvailable = GetAvailablePrizes(ref prizeReportHtml);
                if (prizesAvailable != null && (prizesAvailable == oldPrizesValue - prizesToReport))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get max available prizes to report.
        /// </summary>
        /// <param name="prizesHtml">https://m.gasbuddy.com/Prize.aspx</param>
        /// <returns></returns>
        public static int? GetAvailablePrizes(ref CQ prizesHtml)
        {
            if (!prizesHtml.IsNullOrEmpty())
            {
                string text = prizesHtml[":contains('You can get up to'):last"].Text();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string ticketsText = text.Substring("up to", "tickets");
                    if (!string.IsNullOrWhiteSpace(ticketsText))
                    {
                        int tickets;
                        if (int.TryParse(ticketsText.Trim(), out tickets))
                            return tickets;
                    }
                }
            }

            return null;
        }

        public static void CheckAuthorization(ref User user)
        {
            if (user != null && user.Mobile != null && user.Website != null)
            {
                if (!user.Mobile.isLoggedIn)
                    Authorization.LoginMobile(ref user);

                if (!user.Website.isLoggedIn)
                    Authorization.LoginWebsite(ref user);
            }
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
            //if (!html.IsNullOrEmpty()
            //    && html[string.Format("a>:contains('{0}')", user.UserName)].IsNullOrEmpty()
            //    )
            int? todayPoints = GetTodaysPoints(ref user);
            if (todayPoints != null && todayPoints > 0)
            {
                return true;
            }

            return false;
        }

        private static int? GetPrizeEntriesReported(ref CQ prizeEntriesHtml)
        {
            if (!prizeEntriesHtml.IsNullOrEmpty())
            {
                string text = prizeEntriesHtml[":contains('for this prize draw.  Good luck!'):last"].Text();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string entriesText = text.Substring("have", "entr");
                    if (!string.IsNullOrWhiteSpace(entriesText))
                    {
                        int entries;
                        if (int.TryParse(entriesText.Trim(), out entries))
                            return entries;
                    }
                }
            }
            return null;
        }
    }
}
