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
    partial class Service2 : ServiceBase
    {
        public Service2()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                SiAuto.Main.LogMessage("GasBuddy.Prepare Service has been started.");
                if (PrepareService())
                    this.Stop();
                else
                    throw new Exception("Could not stop the service,");
                // TODO: Add code here to start your service.
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
                this.Stop();
            }
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
        


        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
