using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using HelperFunctions;
using System.Net;
using GasBuddy.Model;
using GasBuddy.Infrastructure;
using GasBuddy.Infrastructure.Base;
using System.Threading;

namespace GasBuddy
{
    public class Program : BaseClass
    {
        static void Main(string[] args)
        {
            SiAuto.Si.Connections = "tcp()";
            SiAuto.Si.Enabled = true;
            SiAuto.Si.DefaultLevel = Level.Debug;
            SiAuto.Main.WatchString("name", "value");

            //PrepareService();
            User user = UserFunc.GetUser("member9905");
            UserAction.isReported(ref user);


            try
            {

                //List<User> users = UsersFunc.GetUsers();
                //foreach (var u in users)
                //{
                //    try
                //    {
                //        User user = u;
                //        Authorization.LoginMobile(ref user);
                //        Authorization.LoginWebsite(ref user);


                //    }
                //    catch (Exception e)
                //    { 

                //    }
                //}


                //User nazarkin659 = UsersFunc.GetUser("nazarkin659");
                //Authorization.LoginWebsite(ref nazarkin659);
                //UsersFunc.UpdateUser(nazarkin659);
            }
            catch (Exception e)
            {

            }





            ReportPrices();
            //User nazarkin659 = UsersFunc.GetUser("gasbuddy659");

            //Authorization.LoginMobile(ref nazarkin659);
            //UsersFunc.UpdateUser(nazarkin659);




            try
            {
                //User user = UsersFunc.GetUsers(1).FirstOrDefault();
                //if (user != null)
                //{

                //}

            }
            catch (Exception e)
            { }
        }

        public static void ReportPrices()
        {
            //try
            //{
            //    SiAuto.Main.EnterMethod("Program => ReportPrices");

            //    List<User> usersToProcess = UsersFunc.GetUsers();
            //    if (usersToProcess.IsNullOrEmpty())
            //    {
            //        SiAuto.Main.LogMessage("UsersToProcess = [{0}]", usersToProcess.Count);
            //    }
            //    else
            //    {
            //        List<int> zipcodes = Stations.GetZipcodes();
            //        if (zipcodes.IsNullOrEmpty())
            //        {
            //            SiAuto.Main.LogError("Program => ReportPrices: Can not get zipcodes.");
            //        }
            //        else
            //        {
            //            int zipcodeToReport = zipcodes.Random();
            //            List<ZipcodeStations> stations = GasBuddy.Infrastructure.Stations.GetStations(zipcodeToReport);
            //            if (stations.IsNullOrEmpty())
            //            {
            //                SiAuto.Main.LogMessage("No stations found. Zipcode [{0}]", "60641");
            //            }
            //            else
            //            {
            //                foreach (User user in usersToProcess)
            //                {
            //                    try
            //                    {
            //                        User innerUser = user;
            //                        if (!CommonAction.isReadyToReportPrices(ref innerUser))
            //                            continue;

            //                        foreach (var station in stations)
            //                        {
            //                            try
            //                            {
            //                                if (user != null)
            //                                {
            //                                    if (CommonAction.SuccessReportPriceMobile(station.StationsURL, ref user))
            //                                    {
            //                                        if (CommonAction.isReachedTodayMaxPoints(user))
            //                                        {
            //                                            ContactInfo userContactInfo = UsersFunc.GetUserContactInfo(innerUser.UserID);
            //                                            if (userContactInfo != null)
            //                                            {
            //                                                SiAuto.Main.LogMessage("[{0}] going to place prizes, wait 10sec before.", innerUser.UserName);
            //                                                Thread.Sleep(10000);
            //                                                if (CommonAction.ReportPrizeEntries(ref innerUser, ref userContactInfo))
            //                                                {
            //                                                    SiAuto.Main.LogMessage("[{0}] Successfully reported prizes, Count =[{1}]", innerUser.UserName, innerUser.PrizeEntriesReported);
            //                                                    UsersFunc.UpdateUser(innerUser);
            //                                                    break;
            //                                                }
            //                                                else
            //                                                {
            //                                                    SiAuto.Main.LogError("[{0}] Could not report prizes.", user.UserName);
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        //TODO: Logs ???
            //                                    }
            //                                }
            //                            }
            //                            catch (Exception e)
            //                            {
            //                                SiAuto.Main.LogException("Stations Loop, User " + user.UserName, e);
            //                            }
            //                        }
            //                    }
            //                    catch (Exception usersLoop)
            //                    {
            //                        SiAuto.Main.LogException("Users Loop, User " + user.UserName, usersLoop);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    SiAuto.Main.LogException(e);
            //}
            //finally
            //{
            //    SiAuto.Main.LeaveMethod("Program => ReportPrices");
            //}
        }

        public static bool SaveStations()
        {
            List<string> zipCodes = new List<string>
                    {
                        "60654",
                        "60641",
                        "60635",
                        "60612",
                        "60614",
                        "60615",
                        "60610"
                    };

            foreach (string zipCode in zipCodes)
            {
                List<string> stations = CommonAction.GetStationUrls(zipCode);
                foreach (string station in stations)
                {
                    Stations.UpdateStations(int.Parse(zipCode), stations);
                }
            }

            return true;
        }

        public static bool PrepareService()
        {
            try
            {
                PrepareUsers();

                if (!ProcessQueueF.ArchiveData())
                    throw new Exception("Failed to archive data");
                if (!InsertUsersInQueue())
                    throw new Exception("InsertUsersInQueue failed");

                return true;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
            return false;
        }
        /// <summary>
        /// Replicating Users to ProcessQueue table.
        /// Applies only for users where PrizesToReport is not 0.
        /// </summary>
        /// <returns></returns>
        private static bool InsertUsersInQueue()
        {
            List<User> users = UserFunc.GetUsers(500);
            if (users.IsNullOrEmpty())
                throw new Exception("Service1 => InsertUsersInQueue: Can't get users.");
            else
            {
                List<ProcessQueue> queues = new List<ProcessQueue>();
                foreach (var user in users.Where(u => u.PrizesToReport != 0))
                {
                    ProcessQueue queue = new ProcessQueue();
                    queue.UserID = user.UserID;
                    queue.Successful = false;
                    queue.RetryCount = 3;
                    queue.Priority = 0;
                    queue.FailCount = 0;
                    queue.Processing = false;

                    queues.Add(queue);
                }

                ProcessQueueF.AddRecord(queues);
                return true;
            }
        }

        /// <summary>
        /// Setting values for PrizesToReport, PrizeEntriesReported, TodayPointsReceived.
        /// </summary>
        /// <param name="userName"></param>
        private static void PrepareUsers(string userName = null)
        {
            using (var dbConnection = new GasBuddy.Infrastructure.Base.Db())
            {
                if (string.IsNullOrWhiteSpace(userName))
                    dbConnection.Database.ExecuteSqlCommand("exec [PrepareUsers]");
                else
                    dbConnection.Database.ExecuteSqlCommand("exec [PrepareUsers] @UserName={0}", userName);
            }
        }
    }
}
