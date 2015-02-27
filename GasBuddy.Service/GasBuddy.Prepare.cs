using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using GasBuddy.Infrastructure;
using GasBuddy.Model;
using HelperFunctions;

namespace GasBuddy.Service
{
    partial class Service2 : BaseService
    {
        public Service2()
        {
            InitializeLogger();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                SiAuto.Main.EnterMethod("GasBuddy.Prepare => OnStart");


                SiAuto.Main.LogMessage("GasBuddy.Prepare Service has been started.");
                if (PrepareService())
                    this.Stop();
                else
                    throw new Exception("Could not stop the service,");
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException("GasBuddy.Prepare => OnStart", e);
                this.Stop();
            }
            finally
            {
                SiAuto.Main.LeaveMethod("GasBuddy.Prepare => OnStart");
            }
        }
        public bool PrepareService()
        {
            try
            {
                SiAuto.Main.EnterMethod("GasBuddy.Prepare => PrepareService");

                PrepareUsers();

                if (!ProcessQueueF.ArchiveData())
                    throw new Exception("Failed to archive data");
                if (!InsertUsersInQueue())
                    throw new Exception("InsertUsersInQueue failed");

                return true;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException("GasBuddy.Prepare => PrepareService", e);
            }
            finally
            {
                SiAuto.Main.LeaveMethod("GasBuddy.Prepare => PrepareService");
            }
            return false;
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
