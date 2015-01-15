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

namespace GasBuddy
{
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            //SiAuto.Si.Connections = "file(filename=\"c:\\log.txtxt\", level=\"error\")";
            //SiAuto.Si.Enabled = true;
            //SiAuto.Main.LogError("test error");


            try
            {

                User user = UsersFunc.GetUser("gasbuddy659");
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
                        User innerUser = user;
                        if (!innerUser.Mobile.isLoggedIn)
                        {
                            if (!Authorization.LoginMobile(ref innerUser))
                            {
                                SiAuto.Main.LogError("Can't Log In Mobile, User = {0}", user.UserName);
                                break;
                                //TODO: Improve
                            }
                        }
                        if (!innerUser.Website.isLoggedIn)
                        {
                            if (!Authorization.LoginWebsite(ref innerUser))
                            {
                                SiAuto.Main.LogError("Can't Log In WebSite, User = {0}", user.UserName);
                                break;
                                //TODO: Improve
                            }
                        }

                        foreach (string station in stations)
                        {
                            try
                            {
                                if (user != null)
                                {
                                    if (CommonAction.SuccessReportPriceMobile(station, user))
                                    {
                                        if (CommonAction.isReachedTodayMaxPoints(user))
                                            break; //this user is done
                                    }
                                    else
                                    {
                                        //TODO: Logs ???
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                SiAuto.Main.LogException("Users loop", e);
                            }
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
