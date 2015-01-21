using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using Gurock.SmartInspect;
using GasBuddy.Model;
using GasBuddy.Infrastructure;
using System.Threading;

namespace GasBuddy
{
    public class UserAction : BaseClass
    {
        public static bool isReported(ref Model.User user)
        {
            try
            {
                SiAuto.Main.EnterMethod("UserAction => isReported");


                if (user == null)
                    throw new Exception("User == null");
                else
                {
                    if (!user.Mobile.isLoggedIn || !user.Website.isLoggedIn)
                        throw new Exception("User is not logged in.");

                    List<int> zipcodes = Stations.GetZipcodes();
                    if (zipcodes.IsNullOrEmpty())
                        throw new Exception("Can not get list of zipcodes");
                    else
                    {
                        int zipcodeToReport = zipcodes.Random();
                        List<ZipcodeStations> stations = GasBuddy.Infrastructure.Stations.GetStations(zipcodeToReport);
                        if (stations.IsNullOrEmpty())
                        {
                            SiAuto.Main.LogMessage("No stations found. Zipcode [{0}]", zipcodeToReport.ToString());
                        }
                        else
                        {
                            int maxFailCount = 3;
                            foreach (var station in stations)
                            {
                                try
                                {
                                    if (!CommonAction.SuccessReportPriceMobile(station.StationsURL, user))
                                    {
                                        maxFailCount--;
                                        SiAuto.Main.LogError("Failed to report prices. User [{0}] StationURL [{1}] RemainFailCount [{2}]", user.UserName, station.StationsURL, maxFailCount);
                                        if (maxFailCount == 0)
                                        {
                                            SiAuto.Main.LogError("Stopped.");
                                            return false;
                                        }
                                        else
                                            SiAuto.Main.LogError("Trying another station.");

                                        Thread.Sleep(10000);
                                    }
                                    else
                                    {
                                        if (CommonAction.isReachedTodayMaxPoints(user))
                                        {
                                            ContactInfo userContactInfo = UsersFunc.GetUserContactInfo(user.UserID);
                                            if (userContactInfo != null)
                                            {
                                                SiAuto.Main.LogMessage("[{0}] going to place prizes, wait 10sec before.", user.UserName);
                                                Thread.Sleep(10000);
                                                if (CommonAction.ReportPrizeEntries(ref user, ref userContactInfo))
                                                {
                                                    UsersFunc.UpdateUser(user);
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    SiAuto.Main.LogException("Stations Loop, User " + user.UserName, e);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(string.Format("UserAction => isReported: User [{0}]", user.UserName), e);
            }
            finally
            {
                SiAuto.Main.LeaveMethod("UserAction => isReported");
            }

            return false;
        }
    }
}
