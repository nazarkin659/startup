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

namespace GasBuddy
{
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            ReportPrices();
            try
            {
                User user = UsersFunc.GetUsers(1).FirstOrDefault();
                if (user != null)
                {
                    
                }
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
                    foreach (string station in stations)
                    {
                        foreach (User user in usersToProcess)
                        {
                            try
                            {
                                if (user != null)
                                {
                                    CommonAction.SuccessReportPriceMobile(station, user);
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
