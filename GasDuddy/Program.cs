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
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            SiAuto.Si.Connections = "tcp()";
            SiAuto.Si.Enabled = true;
            SiAuto.Si.DefaultLevel = Level.Debug;
            SiAuto.Main.LogError("test error");
            ReportPrices();


            try
            {

                User user = UsersFunc.GetUser("gasbuddy6591");
                ContactInfo userContactInfo = UsersFunc.GetUserContactInfo(user.UserID);
                CommonAction.CheckAuthorization(ref user);
                CommonAction.ReportPrizeEntries(ref user, ref userContactInfo);
                UsersFunc.UpdateUser(user);
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
            try
            {
                List<User> usersToProcess = UsersFunc.GetUsers();

                if (!usersToProcess.IsNullOrEmpty())
                {
                    List<string> stations = CommonAction.GetStations("60641");

                    foreach (User user in usersToProcess)
                    {
                        try
                        {

                            User innerUser = user;
                            if (!CommonAction.isReadyToReportPrices(ref innerUser))
                                break;

                            foreach (string station in stations)
                            {
                                try
                                {
                                    if (user != null)
                                    {
                                        if (CommonAction.SuccessReportPriceMobile(station, user))
                                        {
                                            if (CommonAction.isReachedTodayMaxPoints(user))
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
                                                        SiAuto.Main.LogError("[{0}] Could not report prizes.", user.UserName);
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
                                    SiAuto.Main.LogException("Stations Loop, User " + user.UserName, e);
                                }
                            }

                            
                        }
                        catch (Exception usersLoop)
                        {
                            SiAuto.Main.LogException("Users Loop, User " + user.UserName, usersLoop);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
        }

    }
}
