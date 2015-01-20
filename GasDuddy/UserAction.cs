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
        public static bool Start(Model.User user)
        {
            try
            {
                SiAuto.Main.EnterMethod("UserAction => ReportPrices");

                User userToProcess = UsersFunc.GetUser(user.UserName);
                List<User> usersToProcess = new List<User> { userToProcess };


                if (usersToProcess.IsNullOrEmpty())
                {
                    SiAuto.Main.LogMessage("UsersToProcess = [{0}]", usersToProcess.Count);
                }
                else
                {
                    List<int> zipcodes = Stations.GetZipcodes();
                    if (zipcodes.IsNullOrEmpty())
                    {
                        SiAuto.Main.LogError("Program => ReportPrices: Can not get zipcodes.");
                    }
                    else
                    {
                        int zipcodeToReport = zipcodes.Random();
                        List<ZipcodeStations> stations = GasBuddy.Infrastructure.Stations.GetStations(zipcodeToReport);
                        if (stations.IsNullOrEmpty())
                        {
                            SiAuto.Main.LogMessage("No stations found. Zipcode [{0}]", "60641");
                        }
                        else
                        {
                            foreach (User userLoop in usersToProcess)
                            {
                                try
                                {
                                    User innerUser = userLoop;
                                    if (!CommonAction.isReadyToReportPrices(ref innerUser))
                                        continue;

                                    foreach (var station in stations)
                                    {
                                        try
                                        {
                                            if (userLoop != null)
                                            {
                                                if (CommonAction.SuccessReportPriceMobile(station.StationsURL, userLoop))
                                                {
                                                    if (CommonAction.isReachedTodayMaxPoints(userLoop))
                                                    {
                                                        ContactInfo userContactInfo = UsersFunc.GetUserContactInfo(innerUser.UserID);
                                                        if (userContactInfo != null)
                                                        {
                                                            SiAuto.Main.LogMessage("[{0}] going to place prizes, wait 10sec before.", innerUser.UserName);
                                                            Thread.Sleep(10000);
                                                            if (CommonAction.ReportPrizeEntries(ref innerUser, ref userContactInfo))
                                                            {
                                                                SiAuto.Main.LogMessage("[{0}] Successfully reported prizes, Count =[{1}]", innerUser.UserName, innerUser.PrizeEntriesReported);
                                                                UsersFunc.UpdateUser(innerUser);
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                SiAuto.Main.LogError("[{0}] Could not report prizes.", userLoop.UserName);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //TODO: Logs ???
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            SiAuto.Main.LogException("Stations Loop, User " + userLoop.UserName, e);
                                        }
                                    }
                                }
                                catch (Exception usersLoop)
                                {
                                    SiAuto.Main.LogException("Users Loop, User " + userLoop.UserName, usersLoop);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
            finally
            {
                SiAuto.Main.LeaveMethod("UserAction => ReportPrices");
            }

            return false;
        }
    }
}
