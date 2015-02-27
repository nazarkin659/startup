using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GasBuddy.Model;
using GasBuddy.Infrastructure;
using HelperFunctions;
using Gurock.SmartInspect;

namespace GasBuddy.UnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        private Random random = new Random((int)DateTime.Now.Ticks);

        [TestInitialize]
        public void InitializeLogger()
        {
            SiAuto.Si.Connections = "tcp()";
            SiAuto.Si.DefaultLevel = Level.Debug;
            SiAuto.Si.Enabled = true;
        }

        [TestMethod]
        public double GetNextRunTime()
        {
            DateTime datetime = DateTime.Now;
            if (datetime != null)
            {
                double r = (24 * 60 * 60) - (datetime.TimeOfDay.TotalSeconds);

                int runBetweenStartSeconds = 0;
                int runBetweenStopSeconds = 4 * 60 * 60;
                double toAdd = random.Next(runBetweenStartSeconds, runBetweenStopSeconds); //actual runtime

                double actualStartTime = r + toAdd;

                var test = new TimeSpan(0, 0, (int)(DateTime.Now.TimeOfDay.TotalSeconds + actualStartTime));
                //var t = datetime.TimeOfDay.TotalSeconds + (new DateTime(new TimeSpan(0, 0, 0, r))).TimeOfDay;

                var a = datetime.Subtract(new TimeSpan(0, 0, (int)actualStartTime)).Hour;

                return actualStartTime;
            }

            return 0;
        }

        [TestMethod]
        public void RunReport()
        {
            GasBuddy.Model.User user;
            user = UserFunc.GetUser("nazarkin659");

            Authorization auth = new Authorization(user);
            auth.ProceedMobileLogin();
            auth.ProceedWebSiteLogin();

            var condition = UserAction.isReported(ref user);

            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void LoginUsers()
        {
            GasBuddy.Model.User user;
            user = UserFunc.GetUser("nazarkin659");

            var authorization = new GasBuddy.Authorization(user);
            authorization.ProceedMobileLogin();
            authorization.ProceedWebSiteLogin();

            var result = authorization.User;

            Assert.IsTrue(result.Mobile.isLoggedIn);
            Assert.IsTrue(result.Website.isLoggedIn);
        }
    }
}
